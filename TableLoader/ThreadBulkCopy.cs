 using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace TableLoader
{
    class ThreadBulkCopy : BackgroundWorker
    {

        public enum StatusType { Waiting, Working, Finished, Error }

        private StatusType _status = StatusType.Waiting;
        public StatusType Status
        {
            get
            {
                lock (this) { return _status; }
            }
            set
            {
                lock (this) { _status = value; }
            }
        }

        #region members

        private ThreadDbCommand _dbCmdThread;
        private EventMessageList _messageList = new EventMessageList();
        private StatusEventList _statusList = new StatusEventList();
        private string _errorMessage = "No Error.";
        private string _tempTableName;
        private string _sqlCreateTempTable;
        private int _timeoutDb;
        private int _reattempts;
        private string _threadNr;
        private DataTable _dt;
        private string _cstr;
        private SqlConnection _conn; //für Create/Truncate TempTable
        private bool _useTableLock;
        private bool _useBulkInsert;

        #endregion

        public ThreadBulkCopy(ThreadDbCommand dbCmdThread, string tempTableName, DataTable dt, string sqlTemplateCreateTempTable,
                              int timeOutDb, int reattempts, string threadNr, string cstr, SqlConnection conn, bool useTableLock, bool useBulkInsert)
        {
            _conn = new SqlConnection(cstr);

            _dbCmdThread = dbCmdThread;
            _tempTableName = tempTableName;
            _dt = dt;
            _sqlCreateTempTable = sqlTemplateCreateTempTable.Replace(Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, tempTableName);
            _timeoutDb = timeOutDb;
            _reattempts = reattempts;
            _threadNr = threadNr;
            _cstr = cstr;
            _useTableLock = useTableLock;
            _useBulkInsert = useBulkInsert;

            this.DoWork += new DoWorkEventHandler(ThreadBulkCopy_DoWork);
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ThreadBulkCopy_RunWorkerCompleted);
        }

        #region Interface

        public void Start()
        {
            this.RunWorkerAsync();
        }

        public void FireMessages(IsagEvents events, Status status)
        {
            _messageList.FireEvents(events);
            _statusList.LogStatusEvents(status);
            if (Status == StatusType.Error)
                events.FireError(new string[] { "BulkCopy", "Thread " + _threadNr.ToString(), _errorMessage });
        }


        #endregion

        #region Execute

        private void ThreadBulkCopy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if (_dbCmdThread != null && Status != StatusType.Error)
            {
                _dbCmdThread.AddThread(_tempTableName, _threadNr, _conn);

                _messageList.AddMessage(string.Format("DB Command Thread {0} created [{1}].", _threadNr, DateTime.Now.ToString()),
                                                      IsagEvents.IsagEventType.BulkInsert);

                _statusList.AddStatusEvent(Int32.Parse(_threadNr), -1, global::TableLoader.Status.StatusType.dbJobQueued, IsagEvents.IsagEventType.Status);

                //_messageListNew.AddMessage(string.Format("Bulk Copy Thread {0} in DB Command Queue eingetragen [{1}]", _threadNr,
                //     DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString()), IsagEvents.IsagEventType.BulkInsert);

            }

            if (Status != StatusType.Error) Status = StatusType.Finished;

        }

        private void ThreadBulkCopy_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Status = StatusType.Working;

                if (_dbCmdThread != null) CreateTempTable();

                SqlBulkCopyOptions options = _useTableLock ? SqlBulkCopyOptions.TableLock : SqlBulkCopyOptions.Default;

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_cstr, options))
                {
                    bulkCopy.DestinationTableName = _tempTableName;
                    bulkCopy.BulkCopyTimeout = _timeoutDb;

                    if (_useBulkInsert)
                    {
                        bulkCopy.ColumnMappings.Clear();
                        foreach (DataColumn column in _dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }
                    }

                    bool executeBulkCopy = true;
                    int attempt = 1;

                    while (executeBulkCopy)
                    {
                        try
                        {

                            bulkCopy.WriteToServer(_dt);
                            executeBulkCopy = false;
                            string message = string.Format("{0} Rows written by the BulkCopy Thread {1} [{2}].", _dt.Rows.Count.ToString(), _threadNr.ToString(), DateTime.Now.ToString());                            
                            _messageList.AddMessage(message, new string[] {_dt.Rows.Count.ToString()}, IsagEvents.IsagEventType.BulkInsert);

                            //message = string.Format("Bulk Copy Thread {0}: {1} rows written to tempTable {2} [{3}]", _threadNr.ToString(), _dt.Rows.Count.ToString(), _tempTableName,
                            //    DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                            //StatusEvent statusEvent = new StatusEvent(Int32.Parse(_threadNr), _dt.Rows.Count, global::TableLoader.Status.StatusType.bulkCopyFinished);
                            //statusEvent.Param1 = _tempTableName;
                            _statusList.AddStatusEvent(Int32.Parse(_threadNr), _dt.Rows.Count, global::TableLoader.Status.StatusType.bulkCopyFinished, _tempTableName, IsagEvents.IsagEventType.Status);
                            //_messageListNew.AddMessage(message, new string[] { _dt.Rows.Count.ToString() }, IsagEvents.IsagEventType.BulkInsert);
                        }
                        catch (Exception ex)
                        {
                            if ((!ex.Message.Contains("Timeout") || (attempt > _reattempts && _reattempts != 0)) || _useBulkInsert)
                            {
                                executeBulkCopy = false;
                                Status = StatusType.Error;
                                _errorMessage = string.Format("BulkCopy into {0} failed. [{1}]", _tempTableName, DateTime.Now.ToString()) + ex.ToString();

                                throw ex;
                            }
                            else
                            {
                                attempt++;
                                _messageList.AddMessage(string.Format("BulkCopy Thread {0}: Timeout...trying again... [{1}]", _threadNr.ToString(), DateTime.Now.ToString()),
                                                        IsagEvents.IsagEventType.BulkInsert);

                                TruncateTempTable();
                            }
                        }
                    }

                    //_dt.Rows.Clear();
                    
                    _dt.Dispose();

                    _messageList.AddMessage(string.Format("BulkCopy Thread {0} ended [{1}].", _threadNr, DateTime.Now.ToString()),
                                                      IsagEvents.IsagEventType.BulkInsert);

                    _statusList.AddStatusEvent(Int32.Parse(_threadNr), -1, global::TableLoader.Status.StatusType.bulkCopyThreadFinished, IsagEvents.IsagEventType.Status);
                    //_messageListNew.AddMessage(string.Format("BulkCopy Thread {0} finished [{1}].", _threadNr, DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString()),
                    //                                  IsagEvents.IsagEventType.BulkInsert);
                }
            }
            catch (Exception ex)
            {
                if (Status != StatusType.Error)
                {
                    Status = StatusType.Error;
                    _errorMessage = string.Format("BulkCopy Thread failed. [{1}]", DateTime.Now.ToString()) + ex.ToString();
                }

            }
        }

        private void ExecSql(string sql)
        {
            SqlConnection con = _conn;
            if (con.State != ConnectionState.Open)
                con.Open();
            SqlCommand comm = con.CreateCommand();
            comm.CommandText = sql;

            comm.CommandTimeout = _timeoutDb;

            comm.ExecuteNonQuery();
        }


        private void CreateTempTable()
        {
            try
            {
                ExecSql(_sqlCreateTempTable);
                _messageList.AddMessage(string.Format("BulkCopy Thread {0}: Temporary table {1} has been created [{2}].", _threadNr, _tempTableName, DateTime.Now.ToString()),
                                                     IsagEvents.IsagEventType.BulkInsert);
            }
            catch (Exception ex)
            {
                Status = StatusType.Error;
                _errorMessage = string.Format("Cannot create temporary table {0} [{1}]. ", _tempTableName, DateTime.Now.ToString()) + ex.ToString();
                throw ex;
            }


        }

        private void TruncateTempTable()
        {
            try
            {
                ExecSql("truncate table " + _tempTableName);
                _messageList.AddMessage(string.Format("BulkCopy Thread {0}: Temporary table {1} has been truncated [{2}].", _threadNr, _tempTableName, DateTime.Now.ToString()),
                                                     IsagEvents.IsagEventType.BulkInsert);
            }
            catch (Exception ex)
            {

                Status = StatusType.Error;
                _errorMessage = string.Format("Cannot truncate tempory table {0} [{1}]. ", _tempTableName, DateTime.Now.ToString()) + ex.ToString();
                throw ex;
                throw;
            }


        }


        #endregion


    }
}
