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
        /// <summary>
        /// Status types of thread
        /// </summary>
        public enum StatusType { Waiting, Working, Finished, Error }

        /// <summary>
        /// Status type (default: Waiting)
        /// </summary>
        private StatusType _status = StatusType.Waiting;

        /// <summary>
        /// Status type (default: Waiting)
        /// </summary>
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

        /// <summary>
        /// Database command thread
        /// </summary>
        private ThreadDbCommand _dbCmdThread;

        /// <summary>
        /// Event message list
        /// </summary>
        private EventMessageList _messageList = new EventMessageList();

        /// <summary>
        /// Status list
        /// </summary>
        private StatusEventList _statusList = new StatusEventList();

        /// <summary>
        ///  Error message string (used for all threads)
        /// </summary>
        private string _errorMessage = "No Error.";

        /// <summary>
        /// Temporary table name
        /// </summary>
        private string _tempTableName;

        /// <summary>
        /// Sql statement for creating a new temporary table
        /// </summary>
        private string _sqlCreateTempTable;

        /// <summary>
        /// Database timeout
        /// </summary>
        private int _timeoutDb;

        /// <summary>
        /// Reattempts for failed bulk copys
        /// </summary>
        private int _reattempts;

        /// <summary>
        /// Thread number of this thread
        /// </summary>
        private string _threadNr;

        /// <summary>
        /// Datatable (buffer rows) to write to the temporary table
        /// </summary>
        private DataTable _dt;

        /// <summary>
        /// connectionstring
        /// </summary>
        private string _cstr;

        /// <summary>
        /// Sql connection (used for create/truncate temporary table)
        /// </summary>
        private SqlConnection _conn; 

        /// <summary>
        /// Use tablock?
        /// </summary>
        private bool _useTableLock;

        /// <summary>
        /// Use bulk insert?
        /// </summary>
        private bool _useBulkInsert;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dbCmdThread">Database command thread</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="dt">Datatable (buffer rows) to write to the temporary table</param>
        /// <param name="sqlTemplateCreateTempTable">Template for creating temprrary table</param>
        /// <param name="timeOutDb">Database timout</param>
        /// <param name="reattempts">Reattempts for failed bulk copys</param>
        /// <param name="threadNr">Thread nummber</param>
        /// <param name="cstr">Connectionstring</param>
        /// <param name="conn">Sql connection</param>
        /// <param name="useTableLock">Use tablock?</param>
        /// <param name="useBulkInsert">Use bulk insert?</param>
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

        /// <summary>
        /// Starts asynchronous worker job
        /// </summary>
        public void Start()
        {
            this.RunWorkerAsync();
        }

        /// <summary>
        /// Fires event/statistic/error messages
        /// </summary>
        /// <param name="events">Isag events</param>
        /// <param name="status">Status</param>
        public void FireMessages(IsagEvents events, Status status)
        {
            _messageList.FireEvents(events);
            _statusList.LogStatusEvents(status);
            if (Status == StatusType.Error)
                events.FireError(new string[] { "BulkCopy", "Thread " + _threadNr.ToString(), _errorMessage });
        }


        #endregion

        #region Execute

        /// <summary>
        /// React to worker job completed
        /// 
        /// - Adds job to database command thread
        /// - Adds event and status messages
        /// - Sets Status to finished (if status != error)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void ThreadBulkCopy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            if (_dbCmdThread != null && Status != StatusType.Error)
            {
                _dbCmdThread.AddThread(_tempTableName, _threadNr, _conn);

                _messageList.AddMessage(string.Format("DB Command Thread {0} created [{1}].", _threadNr, DateTime.Now.ToString()),
                                                      IsagEvents.IsagEventType.BulkInsert);

                _statusList.AddStatusEvent(Int32.Parse(_threadNr), -1, global::TableLoader.Status.StatusType.dbJobQueued, IsagEvents.IsagEventType.Status);
            }

            if (Status != StatusType.Error) Status = StatusType.Finished;
        }

        /// <summary>
        /// React on DoWork event:
        /// Write data to temporary table
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
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

                            _statusList.AddStatusEvent(Int32.Parse(_threadNr), _dt.Rows.Count, global::TableLoader.Status.StatusType.bulkCopyFinished, _tempTableName, IsagEvents.IsagEventType.Status);
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

                    _dt.Dispose();

                    _messageList.AddMessage(string.Format("BulkCopy Thread {0} ended [{1}].", _threadNr, DateTime.Now.ToString()),
                                                      IsagEvents.IsagEventType.BulkInsert);

                    _statusList.AddStatusEvent(Int32.Parse(_threadNr), -1, global::TableLoader.Status.StatusType.bulkCopyThreadFinished, IsagEvents.IsagEventType.Status);
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

        /// <summary>
        /// Executes a sql command
        /// </summary>
        /// <param name="sql">Sql statement</param>
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

        /// <summary>
        /// Creates temporary table
        /// </summary>
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

        /// <summary>
        /// Truncates temporary table
        /// </summary>
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
            }
        }

        #endregion
    }
}
