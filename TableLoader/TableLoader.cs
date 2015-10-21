using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Data.SqlClient;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Data;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections;
using System.Data.Common;
using System.Data.OleDb;
using System.ComponentModel;
using System.Threading;
using TableLoader.Framework.Mapping;

namespace TableLoader {


#if     (SQL2008)
   [DtsPipelineComponent(DisplayName = "TableLoader 3",
        ComponentType = ComponentType.DestinationAdapter,
        CurrentVersion = 1,
        IconResource = "TableLoader.Resources.TableLoader.ico",
        UITypeName = "TableLoader.TableLoaderUI, TableLoader3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1bfbf132955f2db6")]
#elif   (SQL2012)
     [DtsPipelineComponent(DisplayName = "TableLoader 3",
        ComponentType = ComponentType.DestinationAdapter,
        CurrentVersion = 0,
        IconResource = "TableLoader.Resources.TableLoader.ico",
        UITypeName = "TableLoader.TableLoaderUI, TableLoader4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=bb59c1475df39544")]
#elif   (SQL2014)
    [DtsPipelineComponent(DisplayName = "TableLoader 3",
      ComponentType = ComponentType.DestinationAdapter,
      CurrentVersion = 1,
      IconResource = "TableLoader.Resources.TableLoader.ico",
      UITypeName = "TableLoader.TableLoaderUI, TableLoader5, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d6e3dd235db59be7")]
#else
     [DtsPipelineComponent(DisplayName = "TableLoader 3",
        ComponentType = ComponentType.DestinationAdapter,
        CurrentVersion = 1,
        IconResource = "TableLoader.Resources.TableLoader.ico",
        UITypeName = "TableLoader.TableLoaderUI, TableLoader3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1bfbf132955f2db6")]
#endif

    /// <summary>
    /// the pipeline component TableLoader
    /// </summary>
    public class TableLoader: PipelineComponent {

        #region Members & Properties

        //Runtime & Designtime

        /// <summary>
        /// Main sql connection
        /// </summary>
        private SqlConnection _mainConn = null;

        /// <summary>
        /// Main sql connection
        /// </summary>
        private SqlConnection Conn
        {
            get
            {
                if (_mainConn != null && _mainConn.State == System.Data.ConnectionState.Closed)
                    _mainConn.Open();
                return _mainConn;
            }
        }

        /// <summary>
        /// Bulk sql connection
        /// </summary>
        private SqlConnection _bulkConn = null;

        /// <summary>
        /// Bulk sql connection
        /// </summary>
        private SqlConnection BulkConn
        {
            get
            {
                InitProperties(true);

                if (!_IsagCustomProperties.UseExternalTransaction)
                    return Conn;

                if (_bulkConn != null && _bulkConn.State == System.Data.ConnectionState.Closed)
                    _bulkConn.Open();
                return _bulkConn;
            }
        }

        /// <summary>
        /// custom properties of this component
        /// </summary>
        private IsagCustomProperties _IsagCustomProperties;

        //Runtime
        /// <summary>
        /// Database command type
        /// </summary>
        private TlDbCommand _dbCommand;

        /// <summary>
        /// TableLoader type
        /// </summary>
        private TxAll _txAll;

        /// <summary>
        /// Thread handler
        /// </summary>
        private ThreadHandler _threadHandler;

        /// <summary>
        /// Isag events
        /// </summary>
        private IsagEvents _events;

        /// <summary>
        /// Column infos 
        /// (filled in pre sql to increase performance in process input phase
        /// </summary>
        private List<ColumnInfo> _columnInfos = null;

        /// <summary>
        /// Column name (input and output) mapping used for bulk coppy 
        /// </summary>
        private Dictionary<string, string> _columnMapping;

        /// <summary>
        /// Buffer of rows that is written to the temporary table
        /// </summary>
        private DataTable _dtBuffer = null;

        /// <summary>
        /// Number of rows written to the buffer (_dtBuffer)
        /// </summary>
        private long _chunkCounterBulk;

        /// <summary>
        /// Maximum number of rows that are used for a single database command (i.e. merge)
        /// </summary>
        private long _chunkCounterDbCommand;

        /// <summary>
        /// Temporary table name
        /// </summary>
        string _tempTableName = "";

        /// <summary>
        /// Status
        /// </summary>
        private Status _status;

        /// <summary>
        /// Is pre execute database command executed and finished?
        /// </summary>
        private bool _PreSqlFinished = false;
        #endregion

        #region DesignTime

        /// <summary>
        /// Provides the component properties
        /// </summary>
        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();

            _IsagCustomProperties = new IsagCustomProperties();
            _IsagCustomProperties.SetDefaultValues();
            ComponentMetaDataTools.UpdateVersion(this, ComponentMetaData);

            //Set metadata version to DLL-Version
            DtsPipelineComponentAttribute componentAttr =
                 (DtsPipelineComponentAttribute) Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
            int binaryVersion = componentAttr.CurrentVersion;
            ComponentMetaData.Version = binaryVersion;

            //Clear out base implmentation
            this.ComponentMetaData.RuntimeConnectionCollection.RemoveAll();
            this.ComponentMetaData.InputCollection.RemoveAll();
            this.ComponentMetaData.OutputCollection.RemoveAll();

            //Input
            IDTSInput100 input = this.ComponentMetaData.InputCollection.New();
            input.Name = Constants.INPUT_NAME;
            input.Description = Constants.INPUT_NAME;
            input.HasSideEffects = true;

            //New connection managers
            IDTSRuntimeConnection100 conn = this.ComponentMetaData.RuntimeConnectionCollection.New();
            conn.Name = Constants.CONNECTION_MANAGER_NAME_MAIN;
            conn.Description = "Main Connection to SQL Server";

            //Custom Properties hinzufügen
            IDTSCustomProperty100 prop = ComponentMetaData.CustomPropertyCollection.New();
            prop.Name = Constants.PROP_CONFIG;

            _IsagCustomProperties.Save(ComponentMetaData);
        }



        /// <summary>
        /// Reiniitalized the components metadata
        /// </summary>
        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();
            this.ComponentMetaData.RemoveInvalidInputColumns();
            InitProperties(false);
            Mapping.UpdateInputIdProperties(ComponentMetaData, _IsagCustomProperties);

            _IsagCustomProperties.RebuildMappings(ComponentMetaData, _events);
        }

        /// <summary>
        /// React if input path has been attached
        /// </summary>
        /// <param name="inputID">SSIS input ID</param>
        public override void OnInputPathAttached(int inputID)
        {
            base.OnInputPathAttached(inputID);
            InitProperties(false);

            //Initialize IsagCustomProperties if column config list is empty
            if (_IsagCustomProperties.ColumnConfigList.Count == 0)
            {
                IDTSInput100 input = ComponentMetaData.InputCollection.GetObjectByID(inputID);
                input.InputColumnCollection.RemoveAll();

                _IsagCustomProperties.RebuildMappings(ComponentMetaData, _events);
            }
        }

        #endregion

        /// <summary>
        /// Validates the component metadata
        /// </summary>
        /// <returns>Is component configuration valid?</returns>
        public override DTSValidationStatus Validate()
        {
            InitProperties(false);
            Mapping.UpdateInputIdProperties(ComponentMetaData, _IsagCustomProperties);

            DTSValidationStatus status = base.Validate();
            if (status != DTSValidationStatus.VS_ISVALID)
                return status;

            if (!_IsagCustomProperties.IsValid(ComponentMetaData, _events))
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;

            if (!this.ComponentMetaData.AreInputColumnsValid)
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;

            return DTSValidationStatus.VS_ISVALID;

        }

        #region Run Time

        #region Connection & Transaction
        /// <summary>
        /// Acquire connection from SSIS
        /// (initializes main sql connection and if neccesarry bulk connection
        /// </summary>
        /// <param name="transaction">transaction</param>
        public override void AcquireConnections(object transaction)
        {
            InitProperties(true);


            IDTSRuntimeConnection100 runtimeConn = null;

            //Main
            try
            {
                runtimeConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN];
            }
            catch (Exception) { }

            if (runtimeConn == null || runtimeConn.ConnectionManager == null)
            {
                _events.Fire(IsagEvents.IsagEventType.ErrorConnectionNotInitialized,
                             "ADO.NET [{0}] DB Connection Manager has not been initialized.",
                             Constants.CONNECTION_MANAGER_NAME_MAIN);
            }
            else
            {
                object tempConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN].ConnectionManager.AcquireConnection(transaction);

                if (tempConn is SqlConnection)
                    _mainConn = (SqlConnection) tempConn;
                else
                    _events.Fire(IsagEvents.IsagEventType.ErrorWrongConnection,
                    "Only ADO.NET SQL Server connections are supported for the ADO.NET [{0}] Connection.",
                    Constants.CONNECTION_MANAGER_NAME_MAIN);
            }

            runtimeConn = null;

            //Bulk
            if (!_IsagCustomProperties.UseExternalTransaction && _mainConn != null)
                _bulkConn = _mainConn;
            else
            {
                try
                {
                    runtimeConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK];
                }
                catch (Exception) { }

                if (runtimeConn == null || runtimeConn.ConnectionManager == null)
                {
                    _events.Fire(IsagEvents.IsagEventType.ErrorConnectionNotInitialized,
                    "ADO.NET [{0}] DB Connection Manager has not been initialized.",
                    Constants.CONNECTION_MANAGER_NAME_BULK);
                    //Events.Fire(ComponentMetaData, Events.Type.Error, "ADO.NET [Bulk] Connection Manager has not been initialized.");
                }
                else
                {
                    object tempConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK].ConnectionManager.AcquireConnection(transaction);

                    if (tempConn is SqlConnection)
                        _bulkConn = (SqlConnection) tempConn;
                    else
                        _events.Fire(IsagEvents.IsagEventType.ErrorWrongConnection,
                        "Only ADO.NET SQL Server connections are supported for the ADO.NET [{0}] Connection.",
                        Constants.CONNECTION_MANAGER_NAME_BULK);
                }
            }

        }

        /// <summary>
        /// Release connections
        /// </summary>
        public override void ReleaseConnections()
        {
            base.ReleaseConnections();
        }

        /// <summary>
        /// Close connections
        /// </summary>
        private void CloseConnections()
        {
            if (BulkConn != null && BulkConn.State != System.Data.ConnectionState.Closed)
                BulkConn.Close();
            if (!_IsagCustomProperties.UseExternalTransaction && Conn != null && Conn.State != System.Data.ConnectionState.Closed)
                Conn.Close();
        }

        #endregion

        #region Destination & Temporary Table

        /// <summary>
        /// Creates dataTable (buffer) for destination table
        /// </summary>
        /// <returns>DataTable (buffer) for destination table</returns>
        private DataTable CreateDataTableForDestinationTable()
        {
            return CreateDataTableForDestinationTable(null);
        }

        /// <summary>
        /// Creates dataTable (buffer) for destination table
        /// </summary>
        /// <returns>DataTable (buffer) for destination table</returns>
        private DataTable CreateDataTableForDestinationTable(SqlTransaction transaction)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "select TOP 0 * from " + _IsagCustomProperties.DestinationTable;
            if (transaction != null)
                cmd.Transaction = transaction;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);


            Dictionary<string, ColumnInfo> relevantColumnInfos = new Dictionary<string, ColumnInfo>();

            //Gather relevant columns (those that shall be inserted)
            foreach (ColumnInfo info in _columnInfos)
            {
                if (info.Insert)
                    relevantColumnInfos.Add(info.DestColumnName, info);
            }

            //Remove columns that will not be used
            for (int i = dt.Columns.Count - 1; i >= 0; i--)
            {
                if (!relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName) &&
                    !relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName.ToUpper()))
                {
                    dt.Columns.RemoveAt(i);
                }
            }

            //Sort ColumnInfos according to DataTable
            _columnInfos = new List<ColumnInfo>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName))
                    _columnInfos.Add(relevantColumnInfos[dt.Columns[i].ColumnName]);
                else if (relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName.ToUpper()))
                    _columnInfos.Add(relevantColumnInfos[dt.Columns[i].ColumnName.ToUpper()]);
            }

            if (_columnInfos.Count != relevantColumnInfos.Count)
                _events.Fire(IsagEvents.IsagEventType.Error, "Destination Table conatins less columns than configured!");

            if (dt.Columns.Count != _columnInfos.Count)
                _events.Fire(IsagEvents.IsagEventType.Error, "Column Count for Configuration and Datatable is not equal!");

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string inputColumnName = _columnInfos[i].DestColumnName;
                string outputColumnName = dt.Columns[i].ColumnName;

                if (inputColumnName != outputColumnName && inputColumnName.ToUpper() != outputColumnName.ToUpper())
                    _events.Fire(IsagEvents.IsagEventType.Error, "Mapping Error: Cannot map input column " + inputColumnName + " to output column " + outputColumnName);
            }           

            return dt;
        }

        /// <summary>
        /// Creates ataTable according to properties
        /// (for all database commands except BulkInsert)
        /// </summary>
        /// <param name="input">SSIS input</param>
        /// <returns>DataTable for bulk copy into temporary table</returns>
        private DataTable CreateDataTableForTempTable(IDTSInput100 input)
        {
            DataTable dt = new DataTable();

            foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
            {
                Type typeNet;
                if (config.HasInput)
                    typeNet = SqlCreator.GetNetDataType(input.InputColumnCollection.GetObjectByID(config.InputColumnId).DataType);
                else
                    typeNet = Type.GetType(config.DataTypeOutputNet);

                if (config.IsInputColumnUsed)
                    dt.Columns.Add(config.InputColumnName, typeNet);
            }

            return dt;
        }

        /// <summary>
        /// Add row from pipeline buffer to buffer (Datatable) for temporary table
        /// </summary>
        /// <param name="buffer">SSIS pipeline buffer</param>
        private void AddDataRowToTempDataTable(PipelineBuffer buffer)
        {
            DataRow row = _dtBuffer.NewRow();

            int destIndex = 0;
            for (int i = 0; i < _columnInfos.Count; i++)
            {
                ColumnInfo col = _columnInfos[i];

                if (col.IsUsed)
                {
                    object value = buffer[col.BufferIndex];
                    if (value != null)
                        row[destIndex] = value;
                    destIndex++;
                }
            }

            _dtBuffer.Rows.Add(row);
        }

        #endregion

        #region PreExecute

        /// <summary>
        /// PreExecute phase: Gather all needed informations
        /// </summary>
        public override void PreExecute()
        {
            _status = new Status(_events);
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.started);
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.preExecStarted);

            if (_IsagCustomProperties == null || _events == null)
                InitProperties(true);

            _chunkCounterBulk = 1;
            _dbCommand = new TlDbCommand(_IsagCustomProperties, _events, ComponentMetaData, VariableDispenser);

            if (_IsagCustomProperties.UseMultiThreading)
            {
                CreateThreadHandler();
            }
            else
            {
                _txAll = new TxAll(_events, Conn, _IsagCustomProperties, _dbCommand, BulkConn, ComponentMetaData);
                if (!_IsagCustomProperties.ExcludePreSqlFromTransaction)
                    _txAll.CreateTransaction();
                _chunkCounterDbCommand = 1;
                _tempTableName = _txAll.GetTempTableName();
                _txAll.CreateTempTable(_tempTableName);
            }

            CreateMapping();

            IDTSInput100 input = this.ComponentMetaData.InputCollection[Constants.INPUT_NAME];

            if (_IsagCustomProperties.UseTempTable)
                _dtBuffer = CreateDataTableForTempTable(input);
            else if (_IsagCustomProperties.UseMultiThreading)
                _dtBuffer = CreateDataTableForDestinationTable();
            else
                _dtBuffer = CreateDataTableForDestinationTable(_txAll.Transaction);

            if (!_IsagCustomProperties.UseMultiThreading)
                _txAll.DtBuffer = _dtBuffer;

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.preExecFinished);
        }

        /// <summary>
        /// Gather mapping informations in Pre execute phase
        /// </summary>
        private void CreateMapping()
        {
            _columnInfos = new List<ColumnInfo>(this.ComponentMetaData.InputCollection[0].InputColumnCollection.Count);
            IDTSInput100 input = this.ComponentMetaData.InputCollection[Constants.INPUT_NAME];
            foreach (IDTSInputColumn100 col in input.InputColumnCollection)
            {
                // Find the position in buffers that this column will take, and add it to the map.
                ColumnConfig config = _IsagCustomProperties.GetColumnConfigByInputColumnName(col.Name);
                _columnInfos.Add(new ColumnInfo(col.Name, col.DataType,
                    this.BufferManager.FindColumnByLineageID(input.Buffer, col.LineageID),
                    col.Length, col.Precision, col.Scale, config.IsInputColumnUsed, config.OutputColumnName, config.Insert));
            }

            if (_IsagCustomProperties.UseBulkInsert)
            {
                _columnMapping = new Dictionary<string, string>();
                foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
                {
                    if (config.Insert)
                        _columnMapping.Add(config.OutputColumnName, config.InputColumnName);
                }
            }
        }


        #endregion

        /// <summary>
        /// Processes Input Phase
        /// </summary>
        /// <param name="inputID">Input ID</param>
        /// <param name="buffer">Input buffer.</param>
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            if (_IsagCustomProperties == null || _events == null)
                InitProperties(true);

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.processInputStarted);

            try
            {
                base.ProcessInput(inputID, buffer);

                if (!_PreSqlFinished)
                {

                    ExecPreOrPostExecuteStatement(IsagEvents.IsagEventType.PreSql);
                    if (!_IsagCustomProperties.UseMultiThreading && _IsagCustomProperties.ExcludePreSqlFromTransaction)
                        _txAll.CreateTransaction();
                    _PreSqlFinished = true;
                }

                long chunkSize = _IsagCustomProperties.ChunckSizeBulk;

                while (buffer.NextRow())
                {
                    AddDataRowToTempDataTable(buffer);

                    //ChunkSizeDbCommand is only used if TableLoader type is TxAll and db command != BulkInsert 
                    if (_IsagCustomProperties.IsTransactionAvailable && !_IsagCustomProperties.UseBulkInsert &&
                        _chunkCounterDbCommand == _IsagCustomProperties.ChunkSizeDbCommand)
                    {
                        _chunkCounterBulk = 0;
                        _chunkCounterDbCommand = 1;

                        _txAll.ExecuteBulkCopy(_tempTableName);
                        _txAll.ExecuteDbCommand(_tempTableName);
                        _txAll.TruncateTempTable(_tempTableName);
                        _dtBuffer.Clear();
                    }
                    else
                        _chunkCounterDbCommand++;

                    if (_chunkCounterBulk == chunkSize)
                    {
                        _chunkCounterBulk = 1;

                        if (_IsagCustomProperties.UseMultiThreading)
                        {
                            StartBulkCopyThread();

                            _threadHandler.LogThreadStatistic();
                        }
                        else
                        {
                            _txAll.ExecuteBulkCopy(_tempTableName);
                            _dtBuffer.Clear();
                        }
                    }
                    else
                        _chunkCounterBulk++;
                }
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { "ProcessInput", ex.Message });
                throw ex;
            }

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.processInpitFinished);
        }

        #region PostExecute

        /// <summary>
        /// Post Execute Phase
        /// </summary>
        public override void PostExecute()
        {
            if (_IsagCustomProperties == null || _events == null)
                InitProperties(true);

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.postExecStarted);

            try
            {
                try
                {
                    base.PostExecute();

                    _PreSqlFinished = false;

                    // Finish process input phase
                    //(last data might not have been written because chunk size has not been reached)
                    if (_IsagCustomProperties.UseMultiThreading)
                    {
                        if (_dtBuffer.Rows.Count > 0)
                        {
                            StartBulkCopyThread();
                        }

                        _threadHandler.LogThreadStatistic();
                        _threadHandler.WaitForBulkCopyThreads();
                        _threadHandler.LogThreadStatistic();
                        _threadHandler.WaitForDbCommands();
                        _threadHandler.LogThreadStatistic();
                    }
                    else
                    {
                        if (_dtBuffer.Rows.Count > 0)
                        {
                            _txAll.ExecuteBulkCopy(_tempTableName);
                            _dtBuffer.Clear();
                        }

                        if (!_IsagCustomProperties.UseBulkInsert)
                        {
                            _txAll.ExecuteDbCommand(_tempTableName);
                            _txAll.DropTempTable(_tempTableName, IsagEvents.IsagEventType.TempTableDrop);
                        }
                    }

                    //execute post execute sql command
                    ExecPreOrPostExecuteStatement(IsagEvents.IsagEventType.PostSql);

                    //commit changes to database if multithreading is not used
                    if (!_IsagCustomProperties.UseMultiThreading)
                        _txAll.Commit();
                }
                catch (Exception ex)
                {
                    _events.FireError(new string[] { "PostExecute", ex.Message });
                    throw ex;
                }



            }
            catch (Exception ex)
            {
                if (!_IsagCustomProperties.UseMultiThreading)
                    _txAll.Rollback();

                _events.FireError(new string[] { "PostExecute", ex.Message });
                throw ex;
            }
            finally
            {
                CloseConnections();
            }

            _status.LogStatistic();
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.postExecFinished);
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.finished);


        }

        #endregion

        #endregion

        #region Threads

        /// <summary>
        /// Creates Thread handler
        /// </summary>
        private void CreateThreadHandler()
        {
            IsagEvents.IsagEventType eventType = IsagEvents.IsagEventType.MergeBegin;
            string[] sqlTemplate = null;
            _dbCommand.GetDbCommandDefinition(out eventType, out sqlTemplate);

            _threadHandler = new ThreadHandler(ComponentMetaData.InputCollection[Constants.INPUT_NAME], _IsagCustomProperties,
                                               GetConnectionStringForThread(), Conn, _events, eventType, sqlTemplate, _status);
        }

        /// <summary>
        /// Starts a bulk copy thread
        /// </summary>
        private void StartBulkCopyThread()
        {
            DataTable dt = _dtBuffer;
            _dtBuffer = dt.Clone();

            if (_IsagCustomProperties.DbCommand == IsagCustomProperties.DbCommandType.BulkInsert)
                _threadHandler.AddBulkCopyThread(_IsagCustomProperties.DestinationTable, dt, !_IsagCustomProperties.DisableTablock);
            else if (_IsagCustomProperties.DbCommand == IsagCustomProperties.DbCommandType.BulkInsertRowLock)
                _threadHandler.AddBulkCopyThread(_IsagCustomProperties.DestinationTable, dt, false);
            else
                _threadHandler.AddBulkCopyThread(_IsagCustomProperties.CreateTempTableName(), dt, !_IsagCustomProperties.DisableTablock);
        }

        /// <summary>
        /// Gets connection string for thread (integrated authentication is always used)
        /// </summary>
        /// <returns>connection string for thread</returns>
        private string GetConnectionStringForThread()
        {
            string cstr = Conn.ConnectionString;

            if (cstr.Contains("Integrated Security=True"))
                return cstr;
            else
            {
                int start = cstr.IndexOf("User ID=");
                int end = cstr.IndexOf(";", start);
                cstr = cstr.Substring(0, start) + "Integrated Security=True" + cstr.Substring(end);
            }

            return cstr;

        }

        #endregion

        #region Pre/Post Sql Statement

        /// <summary>
        /// Executes pre- oder post execute command
        /// </summary>
        /// <param name="cmdType"> Events.IsagEventType.PreSql oder Events.IsagEventType.PostSql </param>
        private void ExecPreOrPostExecuteStatement(IsagEvents.IsagEventType cmdType)
        {
            string sql = "";
            string cmdTypeName = "";

            try
            {
                if (cmdType == IsagEvents.IsagEventType.PreSql && _IsagCustomProperties.HasPreSql)
                {
                    sql = _dbCommand.GetExecuteStatementFromTemplate(_IsagCustomProperties.PreSql);
                    cmdTypeName = "PreSql";
                }
                else if (cmdType == IsagEvents.IsagEventType.PostSql && _IsagCustomProperties.HasPostSql)
                {
                    sql = _dbCommand.GetExecuteStatementFromTemplate(_IsagCustomProperties.PostSql);
                    cmdTypeName = "PostSql";
                }
                else if (cmdType != IsagEvents.IsagEventType.PreSql && cmdType != IsagEvents.IsagEventType.PostSql)
                    throw new Exception("Unknown CommandType in ExecPrePostExecuteStatement: " + cmdType);

                // execute pre- oder post execute command

                if (sql.Length > 0)
                {
                    SqlTransaction transaction = _IsagCustomProperties.UseMultiThreading ? null : _txAll.Transaction;
                    int rowsAffected = SqlExecutor.ExecSql(sql, Conn, _IsagCustomProperties.TimeOutDb, transaction);
                    _events.Fire(IsagEvents.IsagEventType.Sql,
                                              "[ExecSql:" + cmdType.ToString() + "]: {0} rows were affected by the Sql Command.",
                                              new string[] { rowsAffected.ToString(), ((int) cmdType).ToString() });
                    _events.Fire(cmdType, cmdTypeName + " Statement has been executed.");
                }
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { cmdTypeName, ex.Message });
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Initializes custom properties
        /// </summary>
        /// <param name="needsStandardConfiguration">Is standard configuration needed?</param>
        private void InitProperties(bool needsStandardConfiguration)
        {
            try
            {
                _IsagCustomProperties = IsagCustomProperties.Load(ComponentMetaData, needsStandardConfiguration);
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { "InitProperties", "Load", ex.Message });
            }


            _events = new IsagEvents(ComponentMetaData, VariableDispenser, _IsagCustomProperties.DestinationTable, _IsagCustomProperties.CustumLoggingTemplate, _IsagCustomProperties.LogLevel);
            Logging.Events = _events;
        }

        #region PerformUpgrade

        /// <summary>
        /// Upgrade from SSIS 2008 to 2012/2014
        /// </summary>
        /// <param name="pipelineVersion">components pipeline verion</param>
        public override void PerformUpgrade(int pipelineVersion)
        {
            try
            {
                if (Mapping.NeedsMapping())
                {
                    InitProperties(false);

                    foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
                    {
                        if (string.IsNullOrEmpty(config.CustomId))
                            config.CustomId = Guid.NewGuid().ToString();
                        AddInputColumnCustomProperty(config.InputColumnName, config.CustomId, Mapping.IdPropertyName);
                    }

                    Mapping.UpdateInputIdProperties(this.ComponentMetaData, _IsagCustomProperties);
                    _IsagCustomProperties.Save(this.ComponentMetaData);
                }

                DtsPipelineComponentAttribute attr =
                    (DtsPipelineComponentAttribute) Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
                ComponentMetaData.Version = attr.CurrentVersion;
            }
            catch (Exception ex)
            {
                bool cancel = false;
                this.ComponentMetaData.FireError(0, "DataConverter Upgrade", ex.ToString(), "", 0, out cancel);
                throw (ex);
            }
        }

        /// <summary>
        ///  Adds a custom property to an input column and sets the value
        ///  (has no effect if custom property already exists)
        /// </summary>
        /// <param name="colName">The name of the input column</param>
        /// <param name="value">the value of the custom property</param>
        /// <param name="propertyName">the name of the custom property</param>
        private void AddInputColumnCustomProperty(string colName, string value, string propertyName)
        {
            IDTSInputColumn100 inputCol = this.ComponentMetaData.InputCollection[0].InputColumnCollection[colName];
            AddCustomProperty(inputCol.CustomPropertyCollection, value, propertyName);
        }

        /// <summary>
        ///  Adds a custom property to a CustomPropertyCollection and sets the value
        ///  (has no effect if custom property already exists)
        /// </summary>
        /// <param name="propCollection">the CustomPropertyCollection</param>
        /// <param name="value">the value of the custom property</param>
        /// <param name="propertyName">the name of the custom property</param>
        private void AddCustomProperty(IDTSCustomPropertyCollection100 propCollection, string value, string propertyName)
        {
            IDTSCustomProperty100 prop = null;
            try
            {
                //do nothing if custom property exists:
                prop = propCollection[propertyName];
            }
            catch (Exception)
            {
                prop = propCollection.New();
                prop.Name = propertyName;
                prop.Value = value;
            }
        }
        #endregion



        /// <summary>
        /// Data that is gathering in pre execute phase and used in process input phase
        /// (SSIS buffer mapping, column properties,...)
        /// </summary>
        class ColumnInfo {
            /// <summary>
            /// Input column name
            /// </summary>
            private string _columnName = string.Empty;

            /// <summary>
            /// Datatype
            /// </summary>
            private DataType _dataType = DataType.DT_STR;

            /// <summary>
            /// SSIS column buffer index
            /// </summary>
            private int _bufferIndex = 0;

            /// <summary>
            /// Datatype precision
            /// </summary>
            private int _precision = 0;

            /// <summary>
            /// Datatype scale
            /// </summary>
            private int _scale = 0;

            /// <summary>
            /// Datatype length
            /// </summary>
            private int _length;

            /// <summary>
            /// Is column used?
            /// </summary>
            private bool _isUsed = false;

            /// <summary>
            /// Destination column name
            /// </summary>
            private string _destColumnName = string.Empty;

            /// <summary>
            /// Is column inserted?
            /// </summary>
            private bool _insert = false;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="columnName">Input column name</param>
            /// <param name="dataType">Datatype</param>
            /// <param name="bufferIndex">SSIS column buffer index</param>
            /// <param name="length">Datatype length</param>
            /// <param name="precision">Datatype precision</param>
            /// <param name="scale">Datatype scale</param>
            /// <param name="isUsed">Is column used?</param>
            /// <param name="destColumnName">Destination column name</param>
            /// <param name="insert">Is column inserted?</param>
            public ColumnInfo(string columnName, DataType dataType, int bufferIndex,
                              int length, int precision, int scale, bool isUsed, string destColumnName, bool insert)
            {
                _columnName = columnName;
                _dataType = dataType;
                _bufferIndex = bufferIndex;
                _precision = precision;
                _scale = scale;
                _length = length;
                _isUsed = isUsed;
                _destColumnName = destColumnName;
                _insert = insert;
            }


            /// <summary>
            /// SSIS column buffer index
            /// </summary>
            public int BufferIndex
            { get { return _bufferIndex; } }

            /// <summary>
            /// Datatype
            /// </summary>
            public DataType ColumnDataType
            { get { return _dataType; } }

            /// <summary>
            /// Input column name
            /// </summary>
            public string ColumnName
            { get { return _columnName; } }

            /// <summary>
            /// Datatype precision
            /// </summary>
            public int Precision
            { get { return _precision; } }

            /// <summary>
            /// Datatype length
            /// </summary>
            public int Length
            { get { return _length; } }

            /// <summary>
            /// Datatype scale
            /// </summary>
            public int Scale
            { get { return _scale; } }

            /// <summary>
            /// Is column used?
            /// </summary>
            public bool IsUsed
            { get { return _isUsed; } }

            /// <summary>
            /// Destination column name
            /// </summary>
            public string DestColumnName
            { get { return _destColumnName; } }

            /// <summary>
            /// Is column inserted?
            /// </summary>
            public bool Insert
            { get { return _insert; } }
        }
    }
}
