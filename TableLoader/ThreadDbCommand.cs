using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;

namespace TableLoader
{
    /// <summary>
    /// Database command thread
    /// 
    /// Executes database command for each finished bulk copy thread.
    /// Database command execution is not concurrent.
    /// </summary>
    public class ThreadDbCommand : BackgroundWorker
    {
        /// <summary>
        /// Has Error?
        /// </summary>
        private bool _hasError = false;

        /// <summary>
        /// Has Error?
        /// </summary>
        public bool HasError
        {
            get
            {
                lock (this) { return _hasError; }
            }
        }

        /// <summary>
        /// Set error = true
        /// </summary>
        private void SetError()
        {
            lock (this) { _hasError = true; }
        }

        /// <summary>
        /// Number of finished database commands
        /// </summary>
        private int _finishedDbCommands = 0;

        /// <summary>
        /// Number of finished database commands
        /// </summary>
        public int FinishedDbCommands { get { lock (this) { return _finishedDbCommands; } } }

        /// <summary>
        /// Increase number of finished database commands by one
        /// </summary>
        private void IncreaseFinishedDbCommands()
        {
            lock (this)
            {
                _finishedDbCommands++;
            }
        }

        /// <summary>
        /// Get number of created database commands
        /// </summary>
        public int CreatedDbCommands
        {
            get
            {
                lock (this)
                {
                    return _finishedDbCommands + _threadList.Count;
                }
            }
        }

        /// <summary>
        /// Get number of finished database commands
        /// </summary>
        public bool Finished
        {
            get
            {
                lock (this)
                {
                    return !_waitForBulkCopy && _threadList.Count == 0;
                }
            }
        }

        #region member

        /// <summary>
        /// List of event messages
        /// </summary>
        private EventMessageList _messageList = new EventMessageList();

        /// <summary>
        /// List of status events
        /// </summary>
        private StatusEventList _statusList = new StatusEventList();

        /// <summary>
        /// List of error messages
        /// </summary>
        private List<string> _errorMessages = new List<string>(); 

        /// <summary>
        /// Get array of all error messages
        /// </summary>
        /// <returns>array of all error messages</returns>
        public string[] GetErrorMessages()
        {
            return _errorMessages.ToArray();
        }

        /// <summary>
        /// Sql command type
        /// </summary>
        private IsagEvents.IsagEventType _sqlCommandType;

        /// <summary>
        /// Database timout
        /// </summary>
        private int _dbTimeout;

        /// <summary>
        /// Number of reattempts if database command failed
        /// </summary>
        private int _reattempts;

        /// <summary>
        /// Sql connection
        /// </summary>
        private SqlConnection _connection;

        /// <summary>
        /// Sql command templates
        /// </summary>
        private string[] _templateSql;

        /// <summary>
        /// List of database command thread definitions
        /// </summary>
        private List<ThreadDefiniton> _threadList = new List<ThreadDefiniton>();

        /// <summary>
        /// Have to wait for bulk copy thread?
        /// (true unless all bulk copy threads have been finished)
        /// </summary>
        private bool _waitForBulkCopy = true;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connection">Sql connection</param>
        /// <param name="sqlCommandType">Sql command type</param>
        /// <param name="dbTimeout">Database timout</param>
        /// <param name="reattempts">Number of reattempts if database command failed</param>
        /// <param name="templateSql">Sql command templates</param>
        public ThreadDbCommand(SqlConnection connection, IsagEvents.IsagEventType sqlCommandType, int dbTimeout, int reattempts, string[] templateSql)
        {
            _connection = connection;
            _sqlCommandType = sqlCommandType;
            _dbTimeout = dbTimeout;
            _reattempts = reattempts;
            _templateSql = templateSql;

            this.DoWork += new DoWorkEventHandler(ThreadDbCommand_DoWork);
            this.RunWorkerAsync();
        }

        #region Interface

        /// <summary>
        /// Adds a database command thread
        /// </summary>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="threadNr">Thread number</param>
        /// <param name="conn">Sql connection</param>
        public void AddThread(string tempTableName, string threadNr, SqlConnection conn)
        {
            lock (_threadList)
            {
                _threadList.Add(new ThreadDefiniton() { TempTableName = tempTableName, ThreadNr = threadNr, Conn = conn });

                _statusList.AddStatusEvent(FinishedDbCommands, CreatedDbCommands, Status.StatusType.dbThreadStatistic, IsagEvents.IsagEventType.Status);
            }
        }

        /// <summary>
        /// "WaitForBulkCopy" is set to false.
        /// This means a bulk copy has finished and a temporary table is ready for the database command
        /// </summary>
        public void SetBulkCopyFinished()
        {
            lock (this)
            {
                _waitForBulkCopy = false;
            }
        }

        /// <summary>
        /// Fire even, status and error messages
        /// </summary>
        /// <param name="events">Isag events</param>
        /// <param name="status">Status</param>
        public void FireMessages(IsagEvents events, Status status)
        {
            _messageList.FireEvents(events);
            _statusList.LogStatusEvents(status);      

            if (HasError)
            {
                foreach (string errorMessage in _errorMessages)
                {
                    events.FireError(new string[] { "DbCommand", errorMessage });
                }

                if (_errorMessages.Count == 0) events.FireError(new string[] { "DbCommand", "Fatal error: No Error Message available" });
            }
        }
        #endregion

        /// <summary>
        /// Removes a thread definition from the thread list (-> thread has finished)
        /// </summary>
        /// <param name="threadDef"></param>
        private void RemoveThread(ThreadDefiniton threadDef)
        {
            lock (_threadList)
            {
                _threadList.Remove(threadDef);
            }
        }

        #region Execute

        /// <summary>
        /// Reacton DoWork event:
        /// 
        /// While waiting for bulk copys or thread definition list is not empty (and no error occured) this method will not be left.
        /// If thread definition list is not empty a new database command will be executed
        /// </summary>
        /// <param name="sender">evetn sender</param>
        /// <param name="e"></param>
        private void ThreadDbCommand_DoWork(object sender, DoWorkEventArgs e)
        {
            while ((_waitForBulkCopy || _threadList.Count > 0) && !HasError)
            {
                if (_threadList.Count > 0)
                {
                    ThreadDefiniton currentThread = _threadList[0];
                    ExecDbCommand(currentThread);
                    RemoveThread(currentThread);
                    IncreaseFinishedDbCommands();
                }
                else Thread.Sleep(180);
            }
        }

        /// <summary>
        /// Executes database command (using a thread deinition)
        /// </summary>
        /// <param name="threadDef">thread deinition</param>
        private void ExecDbCommand(ThreadDefiniton threadDef)
        {
            try
            {
                _statusList.AddStatusEvent(Int32.Parse(threadDef.ThreadNr),-1 , Status.StatusType.dbThreadProcessingDataTable, threadDef.TempTableName, IsagEvents.IsagEventType.Status);

                SqlCommand comm1 = threadDef.Conn.CreateCommand();
                comm1.CommandText = "SELECT COUNT(*) FROM " + threadDef.TempTableName;
                object result = comm1.ExecuteScalar();

                _statusList.AddStatusEvent(Int32.Parse(threadDef.ThreadNr), (int)result, Status.StatusType.dbThreadCountTempTable, threadDef.TempTableName, IsagEvents.IsagEventType.Status);

                int rowsAffected = 0;
                string sql;

                if (_templateSql.Length > 0)
                {
                    if (threadDef.Conn.State != System.Data.ConnectionState.Open)
                        threadDef.Conn.Open();

                    SqlCommand comm = threadDef.Conn.CreateCommand();
                    //comm.CommandText = sql;
                    comm.CommandTimeout = _dbTimeout;


                    for (int i = 0; i < _templateSql.Length; i++)
                    {
                        sql = _templateSql[i].Replace("#SCD_" + Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, "[#SCD_" + threadDef.TempTableName.Substring(3))
                                             .Replace("SCD_" + Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, "SCD_" + threadDef.TempTableName.Substring(3, threadDef.TempTableName.Length - 4))
                                             .Replace(Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, threadDef.TempTableName)
                                             .Replace(Constants.TEMP_TABLE_PLACEHOLDER, threadDef.TempTableName);

                        bool executeDbCommand = true;
                        int attempt = 1;
                        while (executeDbCommand)
                        {
                            try
                            {
                                comm.CommandText = sql;
                                rowsAffected = comm.ExecuteNonQuery();

                                executeDbCommand = false;
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.Contains("Timeout") || (attempt > _reattempts && _reattempts != 0))
                                {
                                    executeDbCommand = false;
                                    SetError();
                                    _errorMessages.Add(string.Format("DbCommand {0} failed. [{1}]: {2}", threadDef.ThreadNr,
                                                                  DateTime.Now.ToString(), sql) + ex.ToString());
                                    throw ex;
                                }
                                else
                                {
                                    attempt++;
                                    _messageList.AddMessage(string.Format("DbCommand Thread {0}: Timeout...trying again... [{1}]", threadDef.ThreadNr, DateTime.Now.ToString()),
                                                            IsagEvents.IsagEventType.Sql);
                                }
                            }
                        }

                    }

                    string message = string.Format("[Exec DbCommand Thread {0} : {1}]: {2} rows were affected by the Sql Command. ({3})",
                                                   threadDef.ThreadNr, _sqlCommandType.ToString(), rowsAffected.ToString(), DateTime.Now.ToString());
                    _messageList.AddMessage(message, IsagEvents.IsagEventType.Sql);

                    _statusList.AddStatusEvent(Int32.Parse(threadDef.ThreadNr), rowsAffected, Status.StatusType.dbJobFinished, threadDef.TempTableName, IsagEvents.IsagEventType.Status);

                    DropTemporaryTable(comm, threadDef);

                    threadDef.Conn.Close();
                    threadDef.Conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (!HasError)
                {
                    SetError();

                    _errorMessages.Add(string.Format("DbCommand Thread {0} failed. [{1}] : {2}", threadDef.ThreadNr, DateTime.Now.ToString(), _templateSql) + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Drops temporary table after database command has finished
        /// </summary>
        /// <param name="comm">Sql command</param>
        /// <param name="threadDef">Thread definition</param>
        private void DropTemporaryTable(SqlCommand comm, ThreadDefiniton threadDef)
        {
            try
            {
                comm.CommandText = "drop table " + threadDef.TempTableName;
                comm.ExecuteNonQuery();

                string message = string.Format("[Exec DbCommand Thread {0} : {1}]: {2} has been dropped. ({3})",
                                               threadDef.ThreadNr, _sqlCommandType.ToString(), threadDef.TempTableName, DateTime.Now.ToString());
                _messageList.AddMessage(message, IsagEvents.IsagEventType.Sql);
            }
            catch (Exception ex)
            {
                SetError();
                _errorMessages.Add(string.Format("DbCommand {0} failed, unable to drop {1}. [{2}]", threadDef.ThreadNr,
                                              threadDef.TempTableName, DateTime.Now.ToString()) + ex.ToString());
                throw ex;
            }
        }

        #endregion


        /// <summary>
        /// Database command thread definition
        /// </summary>
        private class ThreadDefiniton
        {
            /// <summary>
            /// Thread number
            /// </summary>
            public string ThreadNr { get; set; }

            /// <summary>
            /// Temporary table name
            /// </summary>
            public string TempTableName { get; set; }

            /// <summary>
            /// Sql connection
            /// </summary>
            public SqlConnection Conn { get; set; }
        }
    }


}
