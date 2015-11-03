using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using TableLoader.Log;

namespace TableLoader
{
    class TxAll
    {
        /// <summary>
        /// Sql transaction
        /// </summary>
        private SqlTransaction _dbTransaction = null;

        /// <summary>
        /// Isag events
        /// </summary>
        private IsagEvents _events;

        /// <summary>
        /// Main sql connection
        /// </summary>
        private SqlConnection _conn;

        /// <summary>
        /// Sql bulk connection
        /// </summary>
        private SqlConnection _bulkConn;

        /// <summary>
        /// components custom properties
        /// </summary>
        private IsagCustomProperties _IsagCustomProperties;

        /// <summary>
        /// TableLoader database command type
        /// </summary>
        private TlDbCommand _dbCommand;

        /// <summary>
        /// SSIS component metadata
        /// </summary>
        IDTSComponentMetaData100 _componentMetaData;

        /// <summary>
        /// Buffer for rows that will be written to he sel table
        /// </summary>
        public DataTable DtBuffer { get; set; }

        /// <summary>
        /// Sql transaction
        /// </summary>
        public SqlTransaction Transaction { get { return _dbTransaction; } }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="events">Isag events</param>
        /// <param name="conn">Main sql connection</param>
        /// <param name="isagCustomProperties">components custom properties</param>
        /// <param name="dbCommand">Database command type</param>
        /// <param name="bulkConn">Bulk csql connection</param>
        /// <param name="componentMetaData">SSIS component metadata</param>
        public TxAll(IsagEvents events, SqlConnection conn,
                     IsagCustomProperties isagCustomProperties, TlDbCommand dbCommand, SqlConnection bulkConn,
                     IDTSComponentMetaData100 componentMetaData)
        {
            _events = events;
            _conn = conn;
            _IsagCustomProperties = isagCustomProperties;
            _dbCommand = dbCommand;
            _bulkConn = bulkConn;
            _componentMetaData = componentMetaData;
        }


        /// <summary>
        /// Executes the database command 
        /// (data is written from temporary table to destination table)
        /// </summary>
        /// <param name="tempTableName">temporary table</param>
        public void ExecuteDbCommand(string tempTableName)
        {
            try
            {
                int rowsAffected = 0;

                IsagEvents.IsagEventType eventType = IsagEvents.IsagEventType.MergeBegin;
                string[] sqlTemplate = null;
                _dbCommand.GetDbCommandDefinition(out eventType, out sqlTemplate);

                if (sqlTemplate.Length > 0)
                {
                    string sql;

                    SqlCommand comm = _conn.CreateCommand();
                    comm.CommandTimeout = _IsagCustomProperties.TimeOutDb;
                    if (_dbTransaction != null) comm.Transaction = _dbTransaction;

                    for (int i = 0; i < sqlTemplate.Length; i++)
                    {
                        sql = sqlTemplate[i].Replace(Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, tempTableName)
                                             .Replace(Constants.TEMP_TABLE_PLACEHOLDER, tempTableName);

                        try
                        {
                            comm.CommandText = sql;
                            rowsAffected = comm.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {

                            _events.FireError(new string[] {string.Format("DbCommand failed. [{0}]: {1}",
                                                                    DateTime.Now.ToString(), sql) + ex.ToString() });
                            throw ex;
                        }
                    }

                }

                _events.Fire(eventType, string.Format("[Exec DbCommand: {0}]: {1} rows were affected by the Sql Command. ({2})",
                                                      eventType.ToString(), rowsAffected.ToString(), DateTime.Now.ToString()));
            }

            catch (Exception ex)
            {
                _events.FireError(new string[] {string.Format("DbCommand failed. [{0}]",
                                                DateTime.Now.ToString()) + ex.ToString() });
                throw;
            }
        }

        /// <summary>
        /// Executes bulk copy
        /// Data is written from the buffer (Datatable) to the temporary or destination table 
        /// </summary>
        /// <param name="tempTableName"></param>
        public void ExecuteBulkCopy(string tempTableName)
        {
            try
            {
                string[] logParam = new string[] { DtBuffer.Rows.Count.ToString() };

                SqlTransaction transaction;
                if (_dbTransaction != null) transaction = _dbTransaction;
                else transaction = _bulkConn.BeginTransaction();

                SqlBulkCopyOptions options = !_IsagCustomProperties.DisableTablock ? SqlBulkCopyOptions.TableLock : SqlBulkCopyOptions.Default;

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_bulkConn, options, transaction))
                {

                    bulkCopy.DestinationTableName = tempTableName;
                    bulkCopy.BulkCopyTimeout = _IsagCustomProperties.TimeOutDb;

                    if (_IsagCustomProperties.UseBulkInsert)
                    {
                        bulkCopy.ColumnMappings.Clear();
                        foreach (DataColumn column in DtBuffer.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }
                    }

                    try
                    {
                        bulkCopy.WriteToServer(DtBuffer);
                        _events.Fire(IsagEvents.IsagEventType.BulkInsert,
                                     string.Format("{0} Rows written by the BulkCopy [{1}].", DtBuffer.Rows.Count.ToString(), DateTime.Now.ToString()),
                                     logParam);

                        if (_dbTransaction == null) transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _events.FireError(new string[] { string.Format("BulkCopy into {0} failed. [{1}]´", tempTableName, DateTime.Now.ToString()) + ex.ToString() });

                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { string.Format("BulkCopy failed. [{0}]. ", DateTime.Now.ToString()) + ex.ToString() });
                throw ex;
            }
        }

        /// <summary>
        /// Creates temporary table
        /// </summary>
        /// <param name="tempTableName">temporary table name</param>
        public void CreateTempTable(string tempTableName)
        {
            IDTSInput100 input = _componentMetaData.InputCollection[Constants.INPUT_NAME];
            string templateCreateTempTable = SqlCreator.GetCreateTempTable(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
            string sqlCreateTempTable = templateCreateTempTable.Replace(Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, tempTableName);

            if (!_IsagCustomProperties.UseBulkInsert)
                SqlExecutor.ExecSql(sqlCreateTempTable, _bulkConn, _IsagCustomProperties.TimeOutDb, _dbTransaction);
        }

        /// <summary>
        /// Truncates table
        /// </summary>
        /// <param name="tempTableName"></param>
        public void TruncateTempTable(string tempTableName)
        {
            try
            {
                SqlExecutor.TruncateTable(tempTableName, _bulkConn, _IsagCustomProperties.TimeOutDb, _dbTransaction);

                _events.Fire(IsagEvents.IsagEventType.TempTableTruncate,
                             string.Format("Temporary Table [{0}] has been truncated [{1}].", tempTableName, DateTime.Now.ToString()));
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { string.Format("Cannot truncate tempory table {0} [{1}]. ", tempTableName, DateTime.Now.ToString()) + ex.ToString() });

                throw ex;
            }
        }

        /// <summary>
        /// Drops temporary table
        /// </summary>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="eventType">Isag event type</param>
        public void DropTempTable(string tempTableName, IsagEvents.IsagEventType eventType)
        {
            try
            {
                SqlExecutor.DropTable(tempTableName, _bulkConn, _IsagCustomProperties.TimeOutDb, _dbTransaction);

                _events.Fire(eventType, string.Format("[Exec DbCommand: {0}]: Temporary Table has been dropped. ({1})",
                                                      eventType.ToString(), DateTime.Now.ToString()));
            }
            catch (Exception ex)
            {
                _events.Fire(eventType, string.Format("[Exec DbCommand: {0}]: Unable to drop temporary table. ({1})",
                                                  eventType.ToString(), DateTime.Now.ToString()));

                throw ex;
            }
        }

        /// <summary>
        /// Commits database transaction
        /// </summary>
        public void Commit()
        {
            if (_dbTransaction != null)
            {
                try
                {
                    _dbTransaction.Commit();
                    _events.Fire(IsagEvents.IsagEventType.TransactionCommit, string.Format("Transaction committed. [{0}]", DateTime.Now.ToString()));
                }
                catch (Exception ex1)
                {
                    _events.FireError(new string[] { "Unable to Commit transaction!", ex1.Message });
                    throw ex1;
                }

            }
        }

        /// <summary>
        /// Rollback database transaction
        /// </summary>
        public void Rollback()
        {
            if (_dbTransaction != null)
            {
                try
                {
                    _dbTransaction.Rollback();
                    _events.Fire(IsagEvents.IsagEventType.TransactionCommit, string.Format("Transaction rolled back. [{0}]", DateTime.Now.ToString()));
                }
                catch (Exception ex1)
                {
                    _events.FireError(new string[] { "Unable to Rollback transaction!", ex1.Message });
                }
            }
        }

        /// <summary>
        /// Creates TableLoader internal connection if not disabled
        /// </summary>
        public void CreateTransaction()
        {

            if (_IsagCustomProperties.UseInternalTransaction)
            {
                string transactionName = Constants.DB_TRANSACTION_NAME + "_" + DateTime.Now.ToLongTimeString();
                _dbTransaction = _conn.BeginTransaction(transactionName);
                _events.Fire(IsagEvents.IsagEventType.TransactionBegin, "TableLoader internal Transaction [{0}] has been started.", transactionName);
            }
        }

        /// <summary>
        /// Gets temporary table name
        /// (destination table name if database command BulkInsert is used)
        /// </summary>
        /// <returns>Table name for bulk insert</returns>
        public string GetTempTableName()
        {

            if (_IsagCustomProperties.UseBulkInsert)
                return _IsagCustomProperties.DestinationTable;
            else
                return _IsagCustomProperties.CreateTempTableName();
        }
    }
}
