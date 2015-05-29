using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Threading;
using System.Data.SqlClient;

namespace TableLoader
{
    class ThreadHandler
    {
    


        private int _finishedBulkCopyThreads = 0;
        private int _createdBulkCopyThreads = 0;
        private long _maxAllowdThreads;
        private List<ThreadBulkCopy> _bulkCopyThreads = new List<ThreadBulkCopy>();
        private IDTSInput100 _input;
        private IsagCustomProperties _isagCustomProperties;
        private int _timeoutDb;
        private int _reattempts;
        private string _cstr;
        private ThreadDbCommand _dbCmdThread = null;
        private IsagEvents _events;
        private SqlConnection _conn;
        private Status _status;

        public ThreadHandler(IDTSInput100 input, IsagCustomProperties isagCustomProperties,
                             string cstr, SqlConnection conn, IsagEvents events,
                             IsagEvents.IsagEventType dbCommandEventType, string[] dbCommandTemplate, Status status)
        {
            _conn = conn;

            if (dbCommandTemplate != null) _dbCmdThread = new ThreadDbCommand(conn, dbCommandEventType, isagCustomProperties.TimeOutDb, isagCustomProperties.Reattempts, dbCommandTemplate);

            _maxAllowdThreads = isagCustomProperties.MaxThreadCount;
            _input = input;
            _isagCustomProperties = isagCustomProperties;
            _timeoutDb = isagCustomProperties.TimeOutDb;
            _reattempts = isagCustomProperties.Reattempts;
            _cstr = cstr;
            _events = events;
            _status = status;
        }

        public void AddBulkCopyThread(string tempTableName, DataTable dt, bool useTableLock)
        {
            //Logging.Log(IsagEvents.IsagEventType.BulkInsert, string.Format("DataTable {0} created with {1} rows.", (_createdBulkCopyThreads + 1).ToString(), dt.Rows.Count.ToString()));
            _status.AddStatus(_createdBulkCopyThreads + 1, dt.Rows.Count, Status.StatusType.dataTableCreated, IsagEvents.IsagEventType.Status);
            UpdateStatus();

            string templateCreateTempTable = SqlCreator.GetCreateTempTable(_input, _isagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
            ThreadBulkCopy thread = new ThreadBulkCopy(_dbCmdThread, tempTableName, dt, templateCreateTempTable, _timeoutDb, _reattempts, (_createdBulkCopyThreads + 1).ToString(), 
                                                       _cstr, _conn, useTableLock, _isagCustomProperties.UseBulkInsert);
            bool showWaitMessage = true;
            while (WaitForFreeBulkCopyThread())
            {
                if (showWaitMessage)
                    _events.Fire(IsagEvents.IsagEventType.BulkInsert, "Waiting ... Max Threat Count for BulkCopys has been reached. " + DateTime.Now.ToString());
                showWaitMessage = false;
                Thread.Sleep(180); //TODO: Timeout?
                UpdateStatus();
            }

            if (!showWaitMessage)
                _events.Fire(IsagEvents.IsagEventType.BulkInsert, "Waiting for free Bulkcopy Thread finished. " + DateTime.Now.ToString());

            if (!HasError())
            {
                _bulkCopyThreads.Add(thread);
                _createdBulkCopyThreads++;

                _status.AddStatus(_createdBulkCopyThreads, dt.Rows.Count, Status.StatusType.bulkCopyThreadCreated, IsagEvents.IsagEventType.Status);
                //Logging.Log(IsagEvents.IsagEventType.BulkInsert, string.Format("BulkCopy Thread {0} created [Datatable {1}: {2} rows]", 
                //    _createdBulkCopyThreads.ToString(), _createdBulkCopyThreads.ToString(), dt.Rows.Count.ToString()));

                thread.Start();
                 ///TODO StatusLog
                Logging.Log(IsagEvents.IsagEventType.BulkInsert, string.Format("BulkCopyThread Status: {0} finished, {1} created", 
                    _finishedBulkCopyThreads.ToString(), _createdBulkCopyThreads.ToString()));

            }
        }

        public void UpdateStatus()
        {
            if (_dbCmdThread != null) _dbCmdThread.FireMessages(_events, _status);
            
            for (int i = _bulkCopyThreads.Count-1; i >= 0; i--)
            {
                _bulkCopyThreads[i].FireMessages(_events, _status);
                
                if (_bulkCopyThreads[i].Status == ThreadBulkCopy.StatusType.Finished)
                {
                    _bulkCopyThreads.Remove(_bulkCopyThreads[i]);
                    _finishedBulkCopyThreads++;
                }
            }

        }

        private bool WaitForFreeBulkCopyThread()
        {
            return _maxAllowdThreads != 0 && (!HasError() && _bulkCopyThreads.Count >= _maxAllowdThreads);
        }

        private bool HasError()
        {
            
            if (_dbCmdThread != null && _dbCmdThread.HasError)
            {
                _events.FireError(_dbCmdThread.GetErrorMessages());
                _events.FireError(new string[] { "Error in DB Thread." });
                throw new Exception("Error in DB Thread.");
            }

            foreach (ThreadBulkCopy thread in _bulkCopyThreads)
            {
                if (thread.Status == ThreadBulkCopy.StatusType.Error)
                {
                    _events.FireError(new string[] { "Error in BulkCopy Thread." });
                    throw new Exception("Error in BulkCopy Thread.");
                }
                
                
            }

            return false;
        }

        public void LogThreadStatistic()
        {
            if (_dbCmdThread != null)
            _events.Fire(IsagEvents.IsagEventType.BulkInsert, string.Format("{0}/{1} BulkCopy Threads finished, {2}/{3} DB Command Threads finished. ({4})",
                         _finishedBulkCopyThreads, _createdBulkCopyThreads, _dbCmdThread.FinishedDbCommands, _dbCmdThread.CreatedDbCommands, DateTime.Now.ToString()));
            else
                _events.Fire(IsagEvents.IsagEventType.BulkInsert, string.Format("{0}/{1} BulkCopy Threads finished. ({2})",
                         _finishedBulkCopyThreads, _createdBulkCopyThreads, DateTime.Now.ToString()));
        }


  
        public void WaitForBulkCopyThreads()
        {
            bool showMessage = false;
            if (!HasError() && _bulkCopyThreads.Count != 0)
            {
                _events.Fire(IsagEvents.IsagEventType.BulkInsert, "PostExecute: Waiting for BulkCopy Threads... " + DateTime.Now.ToString());
                UpdateStatus();
                showMessage = true;
            }

            while (!HasError() && _bulkCopyThreads.Count != 0)
            {
                Thread.Sleep(180);
                UpdateStatus();
            }

            UpdateStatus();

            if (showMessage)
            {
                _events.Fire(IsagEvents.IsagEventType.BulkInsert, "PostExecute: Waiting for Bulkcopy Threads finished. " + DateTime.Now.ToString());
            }

            if (_dbCmdThread != null) _dbCmdThread.SetBulkCopyFinished();
        }

        public void WaitForDbCommands()
        {
            if (_dbCmdThread != null)
            {
                UpdateStatus();

                bool waited = false;
                if (!_dbCmdThread.Finished)
                {
                    _events.Fire(IsagEvents.IsagEventType.Sql, "PostExecute: Waiting for DbCommand Thread... " + DateTime.Now.ToString());
                    waited = true;
                }

                bool abort = false;
                while (!_dbCmdThread.Finished && !abort)
                {
                    Thread.Sleep(180);
                    abort = HasError();
                    UpdateStatus();
                    //else if waited Then UpdateStatus
                }

                UpdateStatus();

                if (waited)
                {
                    _events.Fire(IsagEvents.IsagEventType.BulkInsert, "PostExecute: Waiting for DbCommand Thread finished. " + DateTime.Now.ToString());
                }
            }
        }
    }
}
