using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;

namespace TableLoader
{
    public class ThreadDbCommand : BackgroundWorker
    {
        private bool _hasError = false;
        public bool HasError
        {
            get
            {
                lock (this) { return _hasError; }
            }
        }
        private void SetError()
        {
            lock (this) { _hasError = true; }
        }

        private int _finishedDbCommands = 0;
        public int FinishedDbCommands { get { lock (this) { return _finishedDbCommands; } } }
        private void IncreaseFinishedDbCommands()
        {
            lock (this)
            {
                _finishedDbCommands++;
            }
        }

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

        private EventMessageList _messageList = new EventMessageList();
        private StatusEventList _statusList = new StatusEventList();

        private List<string> _errorMessages = new List<string>(); //"No Error.";

        public string[] GetErrorMessages()
        {
            return _errorMessages.ToArray();
        }

        private IsagEvents.IsagEventType _sqlCommandType;
        private int _dbTimeout;
        private int _reattempts;
        private SqlConnection _connection;
        private string[] _templateSql;
        private List<ThreadDefiniton> _threadList = new List<ThreadDefiniton>();

        private bool _waitForBulkCopy = true;

        #endregion

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

        public void AddThread(string tempTableName, string threadNr, SqlConnection conn)
        {
            lock (_threadList)
            {
                _threadList.Add(new ThreadDefiniton() { TempTableName = tempTableName, ThreadNr = threadNr, Conn = conn });

                _statusList.AddStatusEvent(FinishedDbCommands, CreatedDbCommands, Status.StatusType.dbThreadStatistic, IsagEvents.IsagEventType.Status);
                //_messageListNew.AddMessage(string.Format("{0}/{1} DB Commands finished [{2}]", FinishedDbCommands.ToString(),
                //    CreatedDbCommands.ToString(), DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString()),
                //    IsagEvents.IsagEventType.MergeBegin);
            }
        }

        public void SetBulkCopyFinished()
        {
            lock (this)
            {
                _waitForBulkCopy = false;
            }
        }

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



        private void RemoveThread(ThreadDefiniton threadDef)
        {
            lock (_threadList)
            {
                _threadList.Remove(threadDef);
            }
        }


        #region Execute

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

        private void ExecDbCommand(ThreadDefiniton threadDef)
        {

           
            try
            {
                _statusList.AddStatusEvent(Int32.Parse(threadDef.ThreadNr),-1 , Status.StatusType.dbThreadProcessingDataTable, threadDef.TempTableName, IsagEvents.IsagEventType.Status);
               // _messageListNew.AddMessage(string.Format("DB Command Thread: Processing TempTable {0} from BulkCopy Thread {1} [{2}]",
               //threadDef.TempTableName, threadDef.ThreadNr, DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString()),
               //IsagEvents.IsagEventType.MergeBegin);

                SqlCommand comm1 = threadDef.Conn.CreateCommand();
                comm1.CommandText = "SELECT COUNT(*) FROM " + threadDef.TempTableName;
                object result = comm1.ExecuteScalar();

                _statusList.AddStatusEvent(Int32.Parse(threadDef.ThreadNr), (int)result, Status.StatusType.dbThreadCountTempTable, threadDef.TempTableName, IsagEvents.IsagEventType.Status);
                //_messageListNew.AddMessage(string.Format("TempTable {0} contains {1} rows. [{2}]",
                //     threadDef.TempTableName, result.ToString(), DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString()),
                //     IsagEvents.IsagEventType.MergeBegin);



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
     //               _messageListNew.AddMessage(string.Format("DB Command for TempTable {0} of BulkCopy Thread {1} finished, {2} rows have been affected [{3}]",
     //threadDef.TempTableName, threadDef.ThreadNr, rowsAffected.ToString(), DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString()),
     //IsagEvents.IsagEventType.Sql);

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

        private class ThreadDefiniton
        {
            public string ThreadNr { get; set; }
            public string TempTableName { get; set; }
            public SqlConnection Conn { get; set; }
        }
    }


}
