using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Windows.Forms;
using System.Data;
using TableLoader.ComponentFramework;


namespace TableLoader
{
    public class IsagCustomProperties
    {
        #region Properties

        [BrowsableAttribute(false), XmlIgnore]
        public string Version
        {
            get
            {
                return GetVersion();
            }
        }

        SortableBindingList<ColumnConfig> _columnConfigList;
        public SortableBindingList<ColumnConfig> ColumnConfigList
        {
            get
            {
                if (_columnConfigList == null) _columnConfigList = new SortableBindingList<ColumnConfig>();
                return _columnConfigList;
            }
            set { _columnConfigList = value; }
        }

        public BindingList<ColumnConfig> BulkCopyColumnConfigLIst
        {
            get
            {
                BindingList<ColumnConfig> result = new BindingList<ColumnConfig>();

                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Insert || config.Update || config.Key || config.IsScdValidFrom)
                        result.Add(config);
                }

                return result;
            }

        }


        public string DestinationTable { get; set; }
        public long ChunckSizeBulk { get; set; }
        public long ChunkSizeDbCommand { get; set; }

        public long MaxThreadCount { get; set; }
        public int TimeOutDb { get; set; }
        public int Reattempts { get; set; }

        private string _prefixInput;
        public string PrefixInput
        {
            get
            {
                if (_prefixInput == null) _prefixInput = "";
                return _prefixInput;
            }
            set
            {
                if (value != null) _prefixInput = value;
                else _prefixInput = "";
            }
        }

        private string _prefixOutput;
        public string PrefixOutput
        {
            get
            {
                if (_prefixOutput == null) _prefixOutput = "";
                return _prefixOutput;
            }
            set
            {
                if (value != null) _prefixOutput = value;
                else _prefixOutput = "";
            }
        }

        public string PostSql { get; set; }
        public string PreSql { get; set; }
        public bool ExcludePreSqlFromTransaction { get; set; }

        public TableLoaderType TlType { get; set; }

        DbCommandType _dbCommand;
        public DbCommandType DbCommand
        {
            get { return _dbCommand; }
            set
            {
                _dbCommand = value;
                if ((_dbCommand == DbCommandType.BulkInsert || _dbCommand == DbCommandType.BulkInsertRowLock) && _transaction == TransactionType.External)
                    _transaction = TransactionType.Internal;
            }
        }

        public bool DisableTablock { get; set; }

        TransactionType _transaction;
        public TransactionType Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }

        private string _customMergeCommand;
        public string CustomMergeCommand
        {
            get { return _customMergeCommand; }
            set { _customMergeCommand = value; }
        }

        public bool UseCustomMergeCommand { get; set; }


        private string _standarConfiguration;
        public string StandarConfiguration
        {
            get
            {
                if (_standarConfiguration == null) _standarConfiguration = "";
                return _standarConfiguration;
            }
            set
            {
                if (value == "") AutoUpdateStandardConfiguration = false;
                _standarConfiguration = value;
            }
        }

        public bool AutoUpdateStandardConfiguration { get; set; }

        [BrowsableAttribute(false), XmlIgnore]
        public bool NoAutoUpdateStandardConfiguration { get { return !AutoUpdateStandardConfiguration; } }

        public string CustumLoggingTemplate { get; set; }

        private int _logLevel;
        public int LogLevel
        {
            get
            {
                if (_logLevel == 0) _logLevel = 3; //Default 
                return _logLevel;
            }
            set
            {
                _logLevel = value;
                if (_logLevel < 1 || _logLevel > 3) _logLevel = 3; //Default          
            }
        }

        [BrowsableAttribute(false), XmlIgnore]
        public bool HasScd
        {
            get
            {
                if (DbCommand != DbCommandType.Merge) return false;

                foreach (ColumnConfig config in _columnConfigList)
                {
                    if (config.HasScd) return true;
                }

                return false;
            }
        }

        #region Binding Helper

        [XmlIgnore]
        public bool UseMerge
        {
            get { return (DbCommand == DbCommandType.Merge || DbCommand == DbCommandType.Merge2005); }
        }

        [XmlIgnore]
        public bool UseMultiThreading
        {
            get { return (TlType == TableLoaderType.FastLoad); }
        }

        [XmlIgnore]
        public bool IsTransactionAvailable
        {
            get { return (TlType == TableLoaderType.TxAll); }
        }

        [XmlIgnore]
        public bool IsTransactionUsed
        {
            get { return (Transaction == TransactionType.Internal || Transaction == TransactionType.External); }
        }

        [XmlIgnore]
        public bool CanUseCustomCommand
        {
            get { return (DbCommand == DbCommandType.Merge || DbCommand == DbCommandType.Merge2005 || DbCommand == DbCommandType.Insert); }
        }

        [XmlIgnore]
        public bool UseExternalTransaction
        {
            get { return Transaction == TransactionType.External && TlType == TableLoaderType.TxAll; }
        }

        [XmlIgnore]
        public bool UseInternalTransaction
        {
            get { return Transaction == TransactionType.Internal; }
        }

        [XmlIgnore]
        public bool UseChunkSizeDbCommand
        {
            get { return DbCommand != DbCommandType.BulkInsert && DbCommand != DbCommandType.BulkInsertRowLock; }
        }

        [XmlIgnore]
        public bool HasDestinationTable
        {
            get { return (DestinationTable != null && DestinationTable != ""); }
        }

        [XmlIgnore]
        public bool UseTempTable
        {
            get { return (DbCommand != DbCommandType.BulkInsert && DbCommand != DbCommandType.BulkInsertRowLock); }
        }

        [XmlIgnore]
        public bool UseBulkInsert
        {
            get { return (DbCommand == DbCommandType.BulkInsert || DbCommand == DbCommandType.BulkInsertRowLock); }
        }

        [XmlIgnore]
        public bool HasPreSql
        {
            get { return (PreSql != null && PreSql != ""); }
        }

        [XmlIgnore]
        public bool HasPostSql
        {
            get { return (PostSql != null && PostSql != ""); }
        }

        [XmlIgnore]
        public bool HasMaxThreadCount
        {
            get { return MaxThreadCount != 0; }
        }


        #endregion


        #endregion

        #region Save & Load

        /// <summary>
        /// Speichert den XML String der Instanz dieser Klasse in den CustomProperties
        /// </summary>
        /// <param name="componentMetaData"></param>
        public void Save(IDTSComponentMetaData100 componentMetaData)
        {
            try
            {
                componentMetaData.CustomPropertyCollection[Constants.PROP_CONFIG].Value = SaveToXml();
            }
            catch (Exception)
            {
                IDTSCustomProperty100 prop = componentMetaData.CustomPropertyCollection.New();
                prop.Name = Constants.PROP_CONFIG;
                componentMetaData.CustomPropertyCollection[Constants.PROP_CONFIG].Value = SaveToXml();
            }

        }

        /// <summary>
        /// Erzeugt eine Instanz dieser Klasse anhand eines XML-Strings aus den CutsomProperties
        /// und lädt ggfs. eine Standardkonfiguration nach.
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <param name="variableDispenser"></param>
        /// <returns></returns>
        public static IsagCustomProperties Load(IDTSComponentMetaData100 componentMetaData, bool needsStandardConfiguration)
        {
            IsagCustomProperties properties;

            try
            {
                properties = LoadFromXml(componentMetaData.CustomPropertyCollection[Constants.PROP_CONFIG].Value.ToString());

            }
            catch (Exception ex)
            {
                if (!needsStandardConfiguration)
                {
                    properties = new IsagCustomProperties();
                    properties.SetDefaultValues();
                }
                else
                {
                    throw new Exception("Cannot load the Configuration: " + ex.Message);
                }
            }

            try
            {
                StandardConfiguration stdConfiguration = new StandardConfiguration(componentMetaData.RuntimeConnectionCollection, properties.AutoUpdateStandardConfiguration);
                stdConfiguration.SetStandardConfiguration(ref properties);
            }
            catch (Exception ex)
            {
                if (needsStandardConfiguration) throw new Exception("Cannot load Standard Configuration: " + ex.Message);
            }


            return properties;

        }

        /// <summary>
        /// Serialisiert die Instanz dieser Klasse in einen XML-String
        /// </summary>
        /// <returns></returns>
        public string SaveToXml()
        {
            StringBuilder builder;
            XmlSerializer serializer;
            XmlWriter writer;
            XmlSerializerNamespaces namespaces;

            builder = new StringBuilder();
            serializer = new XmlSerializer(this.GetType());
            writer = XmlWriter.Create(builder);
            namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            serializer.Serialize(writer, this, namespaces);

            return builder.ToString();
        }

        /// <summary>
        /// Erzeugt anhand eines XML-Strings eine Instanz dieser Klasse
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IsagCustomProperties LoadFromXml(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IsagCustomProperties));

            StringReader reader = new StringReader(xml);
            IsagCustomProperties result = (IsagCustomProperties)serializer.Deserialize(reader);

            return result;
        }



        #endregion

        #region Enums

        public enum TableLoaderType
        {
            FastLoad = 0,
            TxAll = 1
        }

        public enum TransactionType
        {
            Internal = 0,
            External = 1,
            None = 2
        }

        public enum DbCommandType
        {
            Merge = 0,
            Merge2005 = 1,
            UpdateTblInsertRow = 2,
            UpdateRowInsertRow = 3,
            BulkInsert = 4,
            Insert = 5,
            BulkInsertRowLock = 6
        }


        public static string[] DB_COMMAND_MERGE_STRING_VALUES = {"Merge (table based)", "Merge (table based - SQL Server 2005)",
                                                                 "Update (table based) - Insert(row based)","Update (row based) - Insert(row based)",
                                                                  "Bulk Insert", "Insert", "Bulk Insert (rowlock)"};
        #endregion

        #region ColumnConfig

        public ColumnConfig AddColumnConfig(SqlColumnList sqlColumns)
        {
            ColumnConfig result = new ColumnConfig();
            result.SetSqlColumnDefinitions(sqlColumns);
            result.Default = "";
            result.Function = "";
            this.ColumnConfigList.Add(result);

            return result;
        }

        public SqlColumnList AddSqlColumnDefinitions(SqlConnection con, string destinationTableName)
        {
            SqlColumnList result = new SqlColumnList();

            try
            {
                if (con.State != ConnectionState.Open) con.Open();

                DataTable schema;
                SqlConnection sqlConnnection = con;
                SqlCommand sqlCom = sqlConnnection.CreateCommand();


                sqlCom.CommandText = "select TOP 0 * from " + SqlCreator.Brackets(destinationTableName);
                SqlDataReader reader = sqlCom.ExecuteReader(CommandBehavior.KeyInfo);
                schema = reader.GetSchemaTable();


                foreach (DataRow row in schema.Rows)
                {

                    result.Add(row["ColumnName"].ToString(), row["DataTypeName"].ToString(), row["DataType"].ToString(),
                                    Int32.Parse(row["ColumnSize"].ToString()), Int32.Parse(row["NumericPrecision"].ToString()),
                                    Int32.Parse(row["NumericScale"].ToString()),
                                    (row["IsKey"].ToString() == "True"), (row["IsAutoIncrement"].ToString() == "True"), (row["AllowDBNull"].ToString() == "True"));
                }

                sqlConnnection.Close();

                foreach (ColumnConfig config in this.ColumnConfigList)
                {
                    config.SetSqlColumnDefinitions(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public void AddSqlColumnDefinitions(SqlColumnList sqlColumns)
        {
            foreach (ColumnConfig config in this.ColumnConfigList)
            {
                config.SetSqlColumnDefinitions(sqlColumns);
            }
        }

        public void AutoMap()
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                config.AutoMap(PrefixInput, PrefixOutput);
            }
        }

        public void AutoMap(ColumnConfig config)
        {
            config.AutoMap(PrefixInput, PrefixOutput);
        }

        public void RemoveOutput()
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                config.RemoveOutput();
            }
        }

        #endregion

        /// <summary>
        /// Baut das Mapping neu auf und korrigiert so ggfs. Fehler
        /// </summary>
        /// <param name="componentMetaData"></param>
        public void RebuildMappings(IDTSComponentMetaData100 componentMetaData, IsagEvents events)
        {
            IDTSInput100 input = componentMetaData.InputCollection[Constants.INPUT_NAME];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            Dictionary<int, ColumnConfig> mappingsInput = new Dictionary<int, ColumnConfig>();
            List<ColumnConfig> mappingsWithoutInputs = new List<ColumnConfig>();
            List<ColumnConfig> newMappings = new List<ColumnConfig>();

            if (this.ContainsWrongUsageType(vInput.VirtualInputColumnCollection, events)) ComponentMetaDataTools.SetUsageTypeReadOnly(vInput);

            //Speichern der bisherigen Mappings in 2 Listen (1x mit Input-Mapping, 1x ohne)
            foreach (ColumnConfig config in this.ColumnConfigList)
            {

                if (config.HasInput)
                    mappingsInput.Add(config.InputColumnId, config);
                else mappingsWithoutInputs.Add(config);

            }


            //Generieren von neuen MappingRows anhand der InputColumns
            foreach (IDTSInputColumn100 inputCol in input.InputColumnCollection)
            {
                ColumnConfig config;

                if (mappingsInput.ContainsKey(inputCol.ID))
                {
                    config = mappingsInput[inputCol.ID];
                    config.InputColumnName = inputCol.Name;
                    config.DataTypeInput = SqlCreator.GetSQLServerDataTypeFromInput(inputCol);
                }
                else
                {
                    config = new ColumnConfig(inputCol.Name, SqlCreator.GetSQLServerDataTypeFromInput(inputCol), inputCol);
                }

                newMappings.Add(config);
            }

            //Aufbauen der neuen Mapping Properties
            ColumnConfigList.Clear();

            foreach (ColumnConfig config in newMappings) ColumnConfigList.Add(config);
            foreach (ColumnConfig config in mappingsWithoutInputs) ColumnConfigList.Add(config);

            this.Save(componentMetaData);
        }

        #region Helpers

        public ColumnConfig GetColumnConfigByInputColumnName(string inputColumnName)
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.InputColumnName == inputColumnName) return config;
            }

            return null;
        }

        public string CreateTempTableName()
        {
            return "[##tempTable" + Guid.NewGuid().ToString() + "]";
        }

        public void SetDefaultValues()
        {
            TimeOutDb = Constants.DB_TIMEOUT_DEFAULT;
            Reattempts = Constants.REATTEMPTS_DEFAULT;
            ChunckSizeBulk = Constants.CHUNK_SIZE_BULK_DEFAULT;
            MaxThreadCount = Constants.MAX_THREAD_COUNT_DEFAULT;
            DbCommand = DbCommandType.Merge;
            Transaction = TransactionType.Internal;
            TlType = TableLoaderType.FastLoad;
        }

        public string[] GetInputColumns()
        {
            List<string> result = new List<string>();

            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.HasInput) result.Add(config.InputColumnName);
            }

            return result.ToArray();
        }

        public bool IsOutputColumnAssigned(string outputColumnName)
        {

            foreach (ColumnConfig config in _columnConfigList)
            {
                if (config.OutputColumnName == outputColumnName) return true;
            }

            return false;

        }

        public static string GetVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            return fvi.FileVersion;
        }

        public string GetCustomCommand()
        {
            string result = "";

            switch (DbCommand)
            {
                case IsagCustomProperties.DbCommandType.Merge:
                    result = SqlCreator.GetSqlMerge(this, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, true);
                    //.Replace("[" + Constants.TEMP_TABLE_PLACEHOLDER + "]", Constants.TEMP_TABLE_PLACEHOLDER);
                    break;
                case IsagCustomProperties.DbCommandType.Merge2005:
                    result = SqlCreator.GetSqlMerge2005(this, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, true);//
                    //.Replace("[" + Constants.TEMP_TABLE_PLACEHOLDER + "]", Constants.TEMP_TABLE_PLACEHOLDER);
                    break;
                case IsagCustomProperties.DbCommandType.UpdateTblInsertRow:
                    break;
                case IsagCustomProperties.DbCommandType.UpdateRowInsertRow:
                    break;
                case IsagCustomProperties.DbCommandType.BulkInsert:
                    break;
                case IsagCustomProperties.DbCommandType.BulkInsertRowLock:
                    break;
                case IsagCustomProperties.DbCommandType.Insert:
                    result = SqlCreator.GetSqlInsert(this, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, true);//;
                    //Replace("[" + Constants.TEMP_TABLE_PLACEHOLDER + "]", Constants.TEMP_TABLE_PLACEHOLDER);
                    break;
                default:
                    break;
            }

            return result;
        }

        public List<ColumnMapping> GetBulkColumnMapping()
        {
            List<ColumnMapping> result = null;

            if (UseBulkInsert)
            {
                result = new List<ColumnMapping>();
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Insert) result.Add(new ColumnMapping(config.InputColumnName, config.OutputColumnName));
                }
            }

            return result;
        }

        #endregion

        #region Validate

        /// <summary>
        /// Gibt Fehlermeldungen und Warnungen aus, sofern die Metadaten nicht konsistent sind.
        /// 
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <returns>Gibt es Fehler, die automatisch korrigiert werden könnten?</returns>
        public bool IsValid(IDTSComponentMetaData100 componentMetaData, IsagEvents events)
        {
            IDTSInput100 input = componentMetaData.InputCollection[Constants.INPUT_NAME];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            WarnIfScdIsNotValid(events);
            WarnIfMoreThanOneKeyIsSelected(events);
            ContainsUnusedColumns(events);
            IsKeyMissing(events);
            InsertOrUpdateAutoIdColumn(events);
            AreConnectionManagersValid(componentMetaData, events);
            ContainsDuplicateOutputColumn(events);

            return !ContainsWrongUsageType(vInput.VirtualInputColumnCollection, events) && AreColumnNamesAndDataTypesValid(input, events) &&
                   !ContainsInputWithoutColumnConfig(vInput, events) && !ContainsColumnConfigWithoutOutput(events);
        }

        private void WarnIfScdIsNotValid(IsagEvents events)
        {
            SCDList scdList = new SCDList(ColumnConfigList, DestinationTable);

            string message = "";
            if (!scdList.IsValid(ref message)) events.Fire(IsagEvents.IsagEventType.Warning, message);

            bool isValid = true;
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.IsScdColumn && config.IsScdValidFrom) isValid = false; 
            }
            if (!isValid) events.Fire(IsagEvents.IsagEventType.Warning, @"You have to choose ""SCD Column"" OR ""SCD ValidFrom"" for one column but not both!");

            isValid = true;
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (string.IsNullOrEmpty(config.ScdTable) && (config.IsScdColumn || config.IsScdValidFrom)) isValid = false;
            }
            if (!isValid) events.Fire(IsagEvents.IsagEventType.Warning, @"If choosing ""SCD Column"" or ""SCD ValidFrom"" you also have to fill out ""SCD Table"".");

            isValid = true;
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (!string.IsNullOrEmpty(config.ScdTable) && !config.IsScdColumn && !config.IsScdValidFrom) isValid = false;
            }
            if (!isValid) events.Fire(IsagEvents.IsagEventType.Warning, @"If filling out ""SCD Table"" you have to choose ""SCD Column"" or ""SCD ValidFrom"".");
        }

        private void WarnIfMoreThanOneKeyIsSelected(IsagEvents events)
        {
            if (DbCommand == DbCommandType.Merge || DbCommand == DbCommandType.Merge2005)
            {
                int keys = 0;
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Key) keys++;
                }

                if (keys > 1)
                {
                    events.Fire(IsagEvents.IsagEventType.Warning, "Es sind mehrere Keys ausgewählt.");
                }
            }
        }
        /// <summary>
        /// Existiert im Mapping eine Zeile, die kein Bezug (über die Input Column ID) zu einer Column des Inputs hat?
        /// Existiert im Mapping eine Zeile, die einen Bezug zu einer Input Column hat, bei der aber der Name abweicht?
        /// </summary>
        /// <param name="input"></param>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool AreColumnNamesAndDataTypesValid(IDTSInput100 input, IsagEvents events)
        {

            foreach (ColumnConfig config in this.ColumnConfigList)
            {


                if (config.HasInput)
                {
                    IDTSInputColumn100 inputColumn;

                    try
                    {
                        inputColumn = input.InputColumnCollection.GetObjectByID(config.InputColumnId);
                    }
                    catch (Exception)
                    {
                        events.Fire(IsagEvents.IsagEventType.Error, "The Mapping contains at least one column with a non existent, but assigned input column!");
                        return false;
                    }


                    if (inputColumn.Name != config.InputColumnName || config.DataTypeInput != SqlCreator.GetSQLServerDataTypeFromInput(inputColumn))
                    {
                        events.Fire(IsagEvents.IsagEventType.Error, "The Mapping contains at least one column with a name or datatype differing from the assigned input column!");

                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Existiert im Mapping eine Zeile, die kein Bezug zu einer Column des Inputs hat?
        /// </summary>
        /// <param name="vInput"></param>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool ContainsInputWithoutColumnConfig(IDTSVirtualInput100 vInput, IsagEvents events)
        {
            for (int i = 0; i < vInput.VirtualInputColumnCollection.Count; i++)
            {
                if (GetColumnConfigByInputColumnName(vInput.VirtualInputColumnCollection[i].Name) == null)
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "The input contains at least one column that is not assigned to a column of the Mapping!");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sofern Update oder Insert gesetzt ist, muss auch eine Zielspalte gewählt sein
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool ContainsColumnConfigWithoutOutput(IsagEvents events)
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (!config.HasOutput && (config.Insert || config.Update))
                {
                    events.Fire(IsagEvents.IsagEventType.Error, @"If ""Use (insert)"" oder Use ""(Update)"" is choosen a destination columns has to be selected!");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Alle UsageTypes müssen auf ReadOnly gesetzt sein
        /// </summary>
        /// <param name="vInputColumnCollection"></param>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool ContainsWrongUsageType(IDTSVirtualInputColumnCollection100 vInputColumnCollection, IsagEvents events)
        {
            for (int i = 0; i < vInputColumnCollection.Count; i++)
            {
                if (vInputColumnCollection[i].UsageType != DTSUsageType.UT_READONLY)
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "The UsageType of all input columns has to be ReadOnly!");
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Sofern es sich nicht um das DB Command "Bulk Insert" oder "Insert" handelt,
        /// muss mind. ein Key ausgewählt sein
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool IsKeyMissing(IsagEvents events)
        {
            if (DbCommand != DbCommandType.BulkInsert && DbCommand != DbCommandType.BulkInsertRowLock && DbCommand != DbCommandType.Insert)
            {
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Key) return false;
                }
            }
            else return false;

            events.Fire(IsagEvents.IsagEventType.Error, @"No Key has been selected!");

            return true;
        }

        /// <summary>
        /// Wenn eine Zielspalte eine AutoID ist, so darf weder Insert noch Update auf true gesetzt sein.
        /// (Ausnahme: Custom Command)
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool InsertOrUpdateAutoIdColumn(IsagEvents events)
        {
            if (!UseCustomMergeCommand)
            {
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.IsOutputAutoId && (config.Insert || config.Update))
                    {
                        events.Fire(IsagEvents.IsagEventType.Error, @"AutoID-Columns must not be marked as ""Use Insert"" or ""UseUpdate"".");
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Die ConnectionManager sind gültig, wenn
        /// - die Main Connection vorhanden ist
        /// 
        /// Sofern die externe Transaktion gewählt ist, 
        /// - muss die Bulk Connection vorhanden sein
        /// - muss mit der Main Connection auf die in der Bulk Connection erzeugte Tempräre Tabelle zugegriffen werden können
        /// - dürfen die beiden Connection Manager nicht identisch sien
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool AreConnectionManagersValid(IDTSComponentMetaData100 componentMetaData, IsagEvents events)
        {
            IDTSRuntimeConnection100 runtimeConn = null;
            SqlConnection mainConn = null;
            SqlConnection bulkConn = null;

            //Main
            try
            {
                runtimeConn = componentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN];
            }
            catch (Exception) { }

            if (runtimeConn == null || runtimeConn.ConnectionManager == null)
            {
                events.Fire(IsagEvents.IsagEventType.Error, "ADO.NET [Main] DB Connection Manager has not been initialized.");
                return false;
            }
            else
            {
                object tempConn = componentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN].ConnectionManager.AcquireConnection(null);

                if (tempConn is SqlConnection) mainConn = (SqlConnection)tempConn;
                else
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "Only ADO.NET SQL Server connections are supported for the ADO.NET [Main] Connection.");
                    return false;
                }
            }

            runtimeConn = null;

            //Bulk
            if (!UseExternalTransaction && mainConn != null) bulkConn = mainConn;
            else
            {
                try
                {
                    runtimeConn = componentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK];
                }
                catch (Exception) { }

                if (runtimeConn == null || runtimeConn.ConnectionManager == null)
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "ADO.NET [Bulk] Connection Manager has not been initialized.");
                    return false;
                }
                else
                {
                    object tempConn = componentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK].ConnectionManager.AcquireConnection(null);

                    if (tempConn is SqlConnection) bulkConn = (SqlConnection)tempConn;
                    else
                    {
                        events.Fire(IsagEvents.IsagEventType.Error, "Only ADO.NET SQL Server connections are supported for the ADO.NET [Bulk] Connection.");
                        return false;
                    }
                }
            }


            //External Transaction
            if (mainConn != null && bulkConn != null && UseExternalTransaction)
            {
                string mainConnectionServer = mainConn.DataSource;
                string bulkConnectionServer = bulkConn.DataSource;

                if (mainConnectionServer.StartsWith(".")) mainConnectionServer = "localhost" + mainConnectionServer.Substring(1);
                if (bulkConnectionServer.StartsWith(".")) bulkConnectionServer = "localhost" + bulkConnectionServer.Substring(1);

                // Die Main Connection muss Zugriff auf die Temporäre Tabelle der Bulk Connection haben
                if (mainConnectionServer != bulkConnectionServer)
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "Please make sure that the Main Connection can access the temporary table created with the bulk Connection");

                    return false;
                }


                // Die Main - und Bulk Connection dürfen nicht identsich sein
                bool areConnectionsIdentic =
                    componentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN].ConnectionManagerID ==
                        componentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK].ConnectionManagerID;

                if (areConnectionsIdentic)
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "Please make sure that the Main- and the Bulk Connection are not identical.");

                    return false;
                }
            }

            return true;
        }

        private bool ContainsDuplicateOutputColumn(IsagEvents events)
        {
            List<string> outputColumns = new List<string>();
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.OutputColumnName != "")
                {
                    if (outputColumns.Contains(config.OutputColumnName))
                    {
                        events.Fire(IsagEvents.IsagEventType.Error, "Please assign Outputcolumns only once.");
                        return true;
                    }
                    else outputColumns.Add(config.OutputColumnName);
                }
            }

            return false;
        }

        /// <summary>
        /// Gibt es im Mapping Columns mit Zielspalte und Quellspalte/Default/Function 
        /// ohne Häkchen bei Use (Insert) oder Use (Update)?
        /// </summary>
        /// <param name="componentMetaData"></param>
        /// <returns></returns>
        private bool ContainsUnusedColumns(IsagEvents events)
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.HasOutput && (config.HasInput || config.HasFunction || config.HasDefault) && (!config.Insert && !config.Update) && !config.IsInputColumnUsed)
                {
                    events.Fire(IsagEvents.IsagEventType.Warning,
                            @"The Mapping contains a column with an output column and an input column, function or default value, but is not marked as ""Use (Insert)"" or ""Use (Update)""");
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
