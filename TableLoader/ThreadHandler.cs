using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Threading;
using System.Data.SqlClient;
using TableLoader.Log;

namespace TableLoader
{
    /// <summary>
    /// Handles bulk copy and database threads
    /// </summary>
    class ThreadHandler
    {   
        /// <summary>
        /// Number of finished bulk copy threads
        /// </summary>
        private int _finishedBulkCopyThreads = 0;
        /// <summary>
        /// Number of created bulk copy threads
        /// </summary>
        private int _createdBulkCopyThreads = 0;
        /// <summary>
        /// Maximum number of concurrent threads
        /// </summary>
        private long _maxAllowdThreads;
        /// <summary>
        /// List of bulk copy threads
        /// </summary>
        private List<ThreadBulkCopy> _bulkCopyThreads = new List<ThreadBulkCopy>();
        /// <summary>
        /// SSIS input
        /// </summary>
        private IDTSInput100 _input;
        /// <summary>
        /// components custom properties
        /// </summary>
        private IsagCustomProperties _isagCustomProperties;
        /// <summary>
        /// database timeout in seconds
        /// </summary>
        private int _timeoutDb;
        /// <summary>
        /// number of reattempts for database command
        /// </summary>
        private int _reattempts;
        /// <summary>
        /// connectionstring
        /// </summary>
        private string _cstr;
        /// <summary>
        /// Database command thread
        /// </summary>
        private ThreadDbCommand _dbCmdThread = null;
        /// <summary>
        /// Siag events
        /// </summary>
        private IsagEvents _events;
        /// <summary>
        /// Sql connection
        /// </summary>
        private SqlConnection _conn;
        /// <summary>
        /// Status
        /// </summary>
        private Status _status;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="input">SSIS input</param>
        /// <param name="isagCustomProperties">Components custom properties</param>
        /// <param name="cstr">Conectionststring</param>
        /// <param name="conn">Sql connection</param>
        /// <param name="events">Isag events</param>
        /// <param name="dbCommandEventType">Database command event type</param>
        /// <param name="dbCommandTemplate">Database command template</param>
        /// <param name="status"></param>
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

        /// <summary>
        /// Add a bulk copy thread
        /// </summary>
        /// <param name="tempTableName">temporary tablename</param>
        /// <param name="dt">Datatable (buffer with rows to write to the temporary table)</param>
        /// <param name="useTableLock">Use tablock?</param>
        public void AddBulkCopyThread(string tempTableName, DataTable dt, bool useTableLock)
        {
            //Logging.Log(IsagEvents.IsagEventType.BulkInsert, string.Format("DataTable {0} created with {1} rows.", (_createdBulkCopyThreads + 1).ToString(), dt.Rows.Count.ToString()));
            _status.AddStatus(_createdBulkCopyThreads + 1, dt.Rows.Count, Status.StatusType.dataTableCreated, IsagEvents.IsagEventType.Status);
            UpdateStatus();

            string templateCreateTempTable = SqlCreator.GetCreateTempTable(_isagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
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
                Logging.Log(IsagEvents.IsagEventType.BulkInsert, string.Format("BulkCopyThread Status: {0} finished, {1} created", 
                    _finishedBulkCopyThreads.ToString(), _createdBulkCopyThreads.ToString()));

            }
        }

        /// <summary>
        /// Updates status
        /// 
        /// Threads gather events/status messages, ThreadHandler fires those events
        /// </summary>
        public void UpdateStatus()
        {
            //Fire database command thread events & messages
            if (_dbCmdThread != null) _dbCmdThread.FireMessages(_events, _status);
            
            ///Iterate over bulk copy threads
            for (int i = _bulkCopyThreads.Count-1; i >= 0; i--)
            {
                //Fire bulk copy thread events & messages
                _bulkCopyThreads[i].FireMessages(_events, _status);
                
                //Remove finished threads and incerease count for finished threads
                if (_bulkCopyThreads[i].Status == ThreadBulkCopy.StatusType.Finished)
                {
                    _bulkCopyThreads.Remove(_bulkCopyThreads[i]);
                    _finishedBulkCopyThreads++;
                }
            }

        }

        /// <summary>
        /// If number of concurrent threads is limited, creating a new bulk copy thread is possible only if maximum number of threads has not
        /// been reached. Determines if it is necessarry to wait for a free bulk copy thread slot.
        /// </summary>
        /// <returns>Is it necessarry to wait for a free bulk copy thread slot?</returns>
        private bool WaitForFreeBulkCopyThread()
        {
            return _maxAllowdThreads != 0 && (!HasError() && _bulkCopyThreads.Count >= _maxAllowdThreads);
        }

        /// <summary>
        /// Does the database command thread or a bulk copy thread has an error?
        /// </summary>
        /// <returns>Does the database command thread or a bulk copy thread has an error?</returns>
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

        /// <summary>
        /// Log statistic for all threads
        /// (log number of bulk copy/database command threads created/finished)
        /// </summary>
        public void LogThreadStatistic()
        {
            if (_dbCmdThread != null)
            _events.Fire(IsagEvents.IsagEventType.BulkInsert, string.Format("{0}/{1} BulkCopy Threads finished, {2}/{3} DB Command Threads finished. ({4})",
                         _finishedBulkCopyThreads, _createdBulkCopyThreads, _dbCmdThread.FinishedDbCommands, _dbCmdThread.CreatedDbCommands, DateTime.Now.ToString()));
            else
                _events.Fire(IsagEvents.IsagEventType.BulkInsert, string.Format("{0}/{1} BulkCopy Threads finished. ({2})",
                         _finishedBulkCopyThreads, _createdBulkCopyThreads, DateTime.Now.ToString()));
        }


        /// <summary>
        /// Waits for bulk copy threads
        /// 
        /// In post execute phase all bulk copy threads have been created and maybe started but usually not finished.
        /// </summary>
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

        /// <summary>
        /// Waits for database command threads
        /// 
        /// In post execute phase all database threads have been created but usually not started/finished.
        /// </summary>
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
