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
using TableLoader.Framework;
using TableLoader.SCD;
using TableLoader.Log;
using System.Linq;


namespace TableLoader
{
    /// <summary>
    /// custom properties for this component
    /// </summary>
    public class IsagCustomProperties : INotifyPropertyChanged
    {
        /// <summary>
        /// Property changed event
        /// (implements Interface of INotifyPropertyChanged)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Announces that a properties value has changed
        /// </summary>
        /// <param name="info">property name</param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #region Properties

        /// <summary>
        /// file version of the assembly
        /// </summary>
        [BrowsableAttribute(false), XmlIgnore]
        public string Version
        {
            get
            {
                return GetVersion();
            }
        }

        /// <summary>
        /// List of column configurations
        /// </summary>
        SortableBindingList<ColumnConfig> _columnConfigList;
        public SortableBindingList<ColumnConfig> ColumnConfigList
        {
            get
            {
                if (_columnConfigList == null)
                    _columnConfigList = new SortableBindingList<ColumnConfig>();
                return _columnConfigList;
            }
            set { _columnConfigList = value; }
        }

        /// <summary>
        /// List of column configurations for bulk copy
        /// </summary>
        public BindingList<ColumnConfig> BulkCopyColumnConfigLIst
        {
            get
            {
                BindingList<ColumnConfig> result = new BindingList<ColumnConfig>();
                List<string> referencedByFunction = GetNeededColumnsReferenceByFunction();

                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Insert || config.Update || config.Key || config.IsScdValidFrom || referencedByFunction.Contains(config.InputColumnName))
                        result.Add(config);
                }

                return result;
            }

        }

        /// <summary>
        /// destination table name
        /// </summary>
        public string DestinationTable { get; set; }

        /// <summary>
        /// Bulk chunk size (number of rows) written with each bulk copy
        /// </summary>
        public long ChunckSizeBulk { get; set; }
        /// <summary>
        /// Database command chunk size 
        /// (number of rows used for each execution of the db command (i.e. merge)
        /// </summary>
        public long ChunkSizeDbCommand { get; set; }

        /// <summary>
        /// Maximum number if concurrent threads
        /// </summary>
        public long MaxThreadCount { get; set; }

        /// <summary>
        /// database timeout in seconds
        /// </summary>
        public int TimeOutDb { get; set; }

        /// <summary>
        /// Number of reattampts for the database command
        /// </summary>
        public int Reattempts { get; set; }

        /// <summary>
        /// Input column prefix
        /// </summary>
        private string _prefixInput;

        /// <summary>
        /// Input column prefix
        /// </summary>
        public string PrefixInput
        {
            get
            {
                if (_prefixInput == null)
                    _prefixInput = "";
                return _prefixInput;
            }
            set
            {
                if (value != null)
                    _prefixInput = value;
                else
                    _prefixInput = "";
            }
        }

        /// <summary>
        /// Output column prefix
        /// </summary>
        private string _prefixOutput;

        /// <summary>
        /// Output column prefix
        /// </summary>
        public string PrefixOutput
        {
            get
            {
                if (_prefixOutput == null)
                    _prefixOutput = "";
                return _prefixOutput;
            }
            set
            {
                if (value != null)
                    _prefixOutput = value;
                else
                    _prefixOutput = "";
            }
        }

        /// <summary>
        /// Post sql database command
        /// </summary>
        public string PostSql { get; set; }

        /// <summary>
        /// Pre sql database command
        /// </summary>
        public string PreSql { get; set; }

        /// <summary>
        /// Exclude pre sql database command from transaction?
        /// </summary>
        public bool ExcludePreSqlFromTransaction { get; set; }

        /// <summary>
        /// TableLoader type 
        /// </summary>
        public TableLoaderType TlType { get; set; }

        /// <summary>
        /// database command type
        /// </summary>
        DbCommandType _dbCommand;

        /// <summary>
        /// database command type
        /// </summary>
        public DbCommandType DbCommand
        {
            get { return _dbCommand; }
            set
            {
                _dbCommand = value;
                if ((_dbCommand == DbCommandType.BulkInsert || _dbCommand == DbCommandType.BulkInsertRowLock) && _transaction == TransactionType.External)
                    _transaction = TransactionType.Internal;
                NotifyPropertyChanged("DbCommand");
            }
        }

        /// <summary>
        /// Disable Tablock?
        /// </summary>
        public bool DisableTablock { get; set; }

        private bool _azureCompatible;
        public bool AzureCompatible
        {
            get
            {
                return _azureCompatible;
            }

            set
            {
                _azureCompatible = value;
                NotifyPropertyChanged("AzureCompatible");
                NotifyPropertyChanged("IsTlTypeEditable");
                if (_azureCompatible)
                {
                    TlType = TableLoaderType.TxAll;
                    NotifyPropertyChanged("TlType");
                }
            }
        }
        /// <summary>
        /// Disable Nonculsterd index on SCD tables?
        /// </summary>
        public bool EnableIndexOnSCD { get; set; }

        /// <summary>
        /// Transaction type
        /// </summary>
        TransactionType _transaction;

        /// <summary>
        /// Transaction type
        /// </summary>
        public TransactionType Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }

        /// <summary>
        /// Custom database command (user can overide standard database commands)
        /// </summary>
        private string _customMergeCommand;
        public string CustomMergeCommand
        {
            get { return _customMergeCommand; }
            set { _customMergeCommand = value; }
        }

        /// <summary>
        /// Use custom database command?
        /// </summary>
        public bool UseCustomMergeCommand { get; set; }

        /// <summary>
        /// Standard configuration
        /// </summary>
        private string _standarConfiguration;

        /// <summary>
        /// Standard configuration
        /// </summary>
        public string StandarConfiguration
        {
            get
            {
                if (_standarConfiguration == null)
                    _standarConfiguration = "";
                return _standarConfiguration;
            }
            set
            {
                if (value == "")
                    AutoUpdateStandardConfiguration = false;
                _standarConfiguration = value;
            }
        }

        /// <summary>
        /// Use Standard configuration from database to update properties?
        /// </summary>
        public bool AutoUpdateStandardConfiguration { get; set; }

        [BrowsableAttribute(false), XmlIgnore]
        public bool NoAutoUpdateStandardConfiguration { get { return !AutoUpdateStandardConfiguration; } }

        /// <summary>
        /// Custom template for logging
        /// </summary>
        public string CustumLoggingTemplate { get; set; }

        /// <summary>
        /// Loglevel (1-3)
        /// </summary>
        private int _logLevel;

        /// <summary>
        /// Loglevel (1-3)
        /// </summary>
        public int LogLevel
        {
            get
            {
                if (_logLevel == 0)
                    _logLevel = 3; //Default 
                return _logLevel;
            }
            set
            {
                _logLevel = value;
                if (_logLevel < 1 || _logLevel > 3)
                    _logLevel = 3; //Default          
            }
        }

        /// <summary>
        /// Has configuration fro slowly changing dimension?
        /// </summary>
        [BrowsableAttribute(false), XmlIgnore]
        public bool HasScd
        {
            get
            {
                if (DbCommand != DbCommandType.Merge)
                    return false;

                foreach (ColumnConfig config in _columnConfigList)
                {
                    if (config.HasScd)
                        return true;
                }

                return false;
            }
        }

        #region Binding Helper

        /// <summary>
        /// Use db command megre?
        /// </summary>
        [XmlIgnore]
        public bool UseMerge
        {
            get { return (DbCommand == DbCommandType.Merge || DbCommand == DbCommandType.Merge2005); }
        }

        /// <summary>
        /// Use multi threading?
        /// </summary>
        [XmlIgnore]
        public bool UseMultiThreading
        {
            get { return (TlType == TableLoaderType.FastLoad); }
        }

        /// <summary>
        /// Is Transaction available?
        /// </summary>
        [XmlIgnore]
        public bool IsTransactionAvailable
        {
            get { return (TlType == TableLoaderType.TxAll); }
        }

        /// <summary>
        /// Is transaction used?
        /// </summary>
        [XmlIgnore]
        public bool IsTransactionUsed
        {
            get { return (Transaction == TransactionType.Internal || Transaction == TransactionType.External); }
        }

        /// <summary>
        /// Can use custom command?
        /// </summary>
        [XmlIgnore]
        public bool CanUseCustomCommand
        {
            get { return (DbCommand == DbCommandType.Merge || DbCommand == DbCommandType.Merge2005 || DbCommand == DbCommandType.Insert); }
        }

        /// <summary>
        /// Use external transaction?
        /// </summary>
        [XmlIgnore]
        public bool UseExternalTransaction
        {
            get { return Transaction == TransactionType.External && TlType == TableLoaderType.TxAll; }
        }


        /// <summary>
        /// Use internal transaction?
        /// </summary>
        [XmlIgnore]
        public bool UseInternalTransaction
        {
            get { return Transaction == TransactionType.Internal; }
        }

        /// <summary>
        /// Use chunk size for db command?
        /// </summary>
        [XmlIgnore]
        public bool UseChunkSizeDbCommand
        {
            get { return DbCommand != DbCommandType.BulkInsert && DbCommand != DbCommandType.BulkInsertRowLock; }
        }

        /// <summary>
        /// Is destination table set?
        /// </summary>
        [XmlIgnore]
        public bool HasDestinationTable
        {
            get { return (DestinationTable != null && DestinationTable != ""); }
        }

        /// <summary>
        /// Use temporary table?
        /// </summary>
        [XmlIgnore]
        public bool UseTempTable
        {
            get { return (DbCommand != DbCommandType.BulkInsert && DbCommand != DbCommandType.BulkInsertRowLock); }
        }

        /// <summary>
        /// Use bulk insert?
        /// </summary>
        [XmlIgnore]
        public bool UseBulkInsert
        {
            get { return (DbCommand == DbCommandType.BulkInsert || DbCommand == DbCommandType.BulkInsertRowLock); }
        }

        /// <summary>
        /// Has pre sql statement?
        /// </summary>
        [XmlIgnore]
        public bool HasPreSql
        {
            get { return (PreSql != null && PreSql != ""); }
        }

        /// <summary>
        /// Has post sql statement?
        /// </summary>
        [XmlIgnore]
        public bool HasPostSql
        {
            get { return (PostSql != null && PostSql != ""); }
        }

        /// <summary>
        /// Are number of threads limited?
        /// </summary>
        [XmlIgnore]
        public bool HasMaxThreadCount
        {
            get { return MaxThreadCount != 0; }
        }

        /// <summary>
        /// Is user allowed to change TableLoader type?
        /// </summary>
        public bool IsTlTypeEditable
        {
            get
            {
                return !AutoUpdateStandardConfiguration && !AzureCompatible;
            }
        }

        #endregion


        #endregion

        #region Save & Load

        /// <summary>
        /// <summary>
        /// Saves this custom properties
        /// </summary>
        /// <param name="componentMetaData">the components metddata</param>
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
        /// load this properties from an xml string (taken from component metadatas custom properties
        /// and loads standard configuration if needed
        /// </summary>
        /// <param name="componentMetaData">the components metddata</param>
        /// <param name="needsStandardConfiguration">Is standard configuration needed?</param>
        /// <returns>instance of IsagCustomProperties</returns>
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
                if (needsStandardConfiguration)
                    throw new Exception("Cannot load Standard Configuration: " + ex.Message);
            }


            return properties;

        }

        /// <summary>
        /// Saves this properties to an xml string
        /// </summary>
        /// <returns>xml string</returns>
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
        /// load this properties from an xml string
        /// </summary>
        /// <param name="xml">xml string</param>
        /// <returns>instance of IsagCustomProperties</returns>
        public static IsagCustomProperties LoadFromXml(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(IsagCustomProperties));

            StringReader reader = new StringReader(xml);
            IsagCustomProperties result = (IsagCustomProperties)serializer.Deserialize(reader);

            return result;
        }



        #endregion

        #region Enums

        /// <summary>
        /// TableLoader type
        /// (Fastload with Multithreading, TxAll with transaction avialable)
        /// </summary>
        public enum TableLoaderType
        {
            FastLoad = 0,
            TxAll = 1
        }

        /// <summary>
        /// Transaction type
        /// external: used connection has an open transaction (set in an Exec Sql Task)
        /// </summary>
        public enum TransactionType
        {
            Internal = 0,
            External = 1,
            None = 2
        }

        /// <summary>
        /// Database command type
        /// </summary>
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

        /// <summary>
        /// Database command type string representations for GUI
        /// </summary>
        public static string[] DB_COMMAND_MERGE_STRING_VALUES = {"Merge (table based)", "Merge (table based - SQL Server 2005)",
                                                                 "Update (table based) - Insert(row based)","Update (row based) - Insert(row based)",
                                                                  "Bulk Insert", "Insert", "Bulk Insert (rowlock)"};
        #endregion

        #region ColumnConfig

        /// <summary>
        /// Creates, adds and returns a column configuration
        /// </summary>
        /// <param name="sqlColumns">Sql column list</param>
        /// <returns>column configuration</returns>
        public ColumnConfig AddColumnConfig(SqlColumnList sqlColumns)
        {
            ColumnConfig result = new ColumnConfig();
            result.SetSqlColumnDefinitions(sqlColumns);
            result.Default = "";
            result.Function = "";
            this.ColumnConfigList.Add(result);

            return result;
        }

        /// <summary>
        /// Creates an sql column list from a database table
        /// </summary>
        /// <param name="con">sql connection</param>
        /// <param name="destinationTableName">destination table name</param>
        /// <returns>Sql column list</returns>
        public SqlColumnList AddSqlColumnDefinitions(SqlConnection con, string destinationTableName)
        {
            SqlColumnList result = new SqlColumnList();

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

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

        /// <summary>
        /// Adds an sql column list
        /// </summary>
        /// <param name="sqlColumns"></param>
        public void AddSqlColumnDefinitions(SqlColumnList sqlColumns)
        {
            foreach (ColumnConfig config in this.ColumnConfigList)
            {
                config.SetSqlColumnDefinitions(sqlColumns);
            }
        }

        /// <summary>
        /// Automap all input columns to output columns
        /// </summary>
        public void AutoMap()
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                config.AutoMap(PrefixInput, PrefixOutput);
            }
        }

        /// <summary>
        /// Automap an input column to an output column
        /// </summary>
        /// <param name="config">column configuration
        /// </param>
        public void AutoMap(ColumnConfig config)
        {
            config.AutoMap(PrefixInput, PrefixOutput);
        }

        /// <summary>
        /// Remove all output columns
        /// </summary>
        public void RemoveOutput()
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                config.RemoveOutput();
            }
        }

        /// <summary>
        /// Get input column names that are reference in functions
        /// </summary>
        /// <returns>list of input column names</returns>
        public List<string> GetNeededColumnsReferenceByFunction()
        {
            List<string> result = new List<string>();

            List<string> functionList = ColumnConfigList.Where(column => column.HasFunction)
                .Select(col => col.Function).ToList();
            result = ColumnConfigList.Where(column => functionList.Exists(funcs => funcs.Contains("@(" + column.BulkColumnName + ")")))
                .Select(col => col.InputColumnName).ToList();

            return result;
        }

        /// <summary>
        /// Is column written to temporary table?
        /// </summary>
        /// <param name="column">column configuration</param>
        /// <returns>Is column written to temporary table?</returns>
        public bool IsColumnUsedForBulkCopy(ColumnConfig column)
        {
            List<string> referencedByFunction = GetNeededColumnsReferenceByFunction();

            return column.IsInputColumnUsed || referencedByFunction.Contains(column.InputColumnName);
        }

        #endregion

        /// <summary>
        /// Rebuilds mapping of input and output columns
        /// (errors are corrected if possible)
        /// </summary>
        /// <param name="componentMetaData">SSIS components metadata</param>
        public void RebuildMappings(IDTSComponentMetaData100 componentMetaData, IsagEvents events)
        {
            IDTSInput100 input = componentMetaData.InputCollection[Constants.INPUT_NAME];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            Dictionary<int, ColumnConfig> mappingsInput = new Dictionary<int, ColumnConfig>();
            List<ColumnConfig> mappingsWithoutInputs = new List<ColumnConfig>();
            List<ColumnConfig> newMappings = new List<ColumnConfig>();

            if (this.ContainsWrongUsageType(vInput.VirtualInputColumnCollection, events))
                ComponentMetaDataTools.SetUsageTypeReadOnly(vInput);

            //Writre existing mappings in 2 lists (one with input columns, one without)
            foreach (ColumnConfig config in this.ColumnConfigList)
            {

                if (config.HasInput)
                    mappingsInput.Add(config.InputColumnId, config);
                else
                    mappingsWithoutInputs.Add(config);

            }

            //Generate new mapping using SSIS input columns
            foreach (IDTSInputColumn100 inputCol in input.InputColumnCollection)
            {
                ColumnConfig config;

                if (mappingsInput.ContainsKey(inputCol.ID))
                {
                    config = mappingsInput[inputCol.ID];
                    config.InputColumnName = inputCol.Name;
                    config.DataTypeInput = SqlCreator.GetSQLServerDataTypeFromInput(inputCol, config.IsGeometryDataType);
                }
                else
                {
                    config = new ColumnConfig(inputCol.Name, SqlCreator.GetSQLServerDataTypeFromInput(inputCol, isGeometry: false), inputCol);
                }

                newMappings.Add(config);
            }

            //Add properties to the newly created mapping
            ColumnConfigList.Clear();

            foreach (ColumnConfig config in newMappings)
                ColumnConfigList.Add(config);
            foreach (ColumnConfig config in mappingsWithoutInputs)
                ColumnConfigList.Add(config);

            this.Save(componentMetaData);
        }

        #region Helpers

        /// <summary>
        /// Get column configuration by input column name
        /// </summary>
        /// <param name="inputColumnName">input column name</param>
        /// <returns>column configuration</returns>
        public ColumnConfig GetColumnConfigByInputColumnName(string inputColumnName)
        {
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.InputColumnName == inputColumnName)
                    return config;
            }

            return null;
        }

        /// <summary>
        /// Create temporary table name
        /// </summary>
        /// <returns></returns>
        public string CreateTempTableName()
        {
            return "[##tempTable" + Guid.NewGuid().ToString() + "]";
        }

        /// <summary>
        /// Set defalut values of properties
        /// </summary>
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

        /// <summary>
        /// Gets an array with all input column names from column configuration list
        /// </summary>
        /// <returns>array with all input column names</returns>
        public string[] GetInputColumns()
        {
            List<string> result = new List<string>();

            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.HasInput)
                    result.Add(config.InputColumnName);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Is a given output column name assigned to an input column?
        /// </summary>
        /// <param name="outputColumnName">output column name</param>
        /// <returns>Is a given output column name assigned to an input column?</returns>
        public bool IsOutputColumnAssigned(string outputColumnName)
        {

            foreach (ColumnConfig config in _columnConfigList)
            {
                if (config.OutputColumnName == outputColumnName)
                    return true;
            }

            return false;

        }

        /// <summary>
        /// Gets assembly version
        /// </summary>
        /// <returns>assembly version</returns>
        public static string GetVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            return fvi.FileVersion;
        }

        /// <summary>
        /// Gets custom sql command
        /// </summary>
        /// <returns>custom sql command</returns>
        public string GetCustomCommand()
        {
            string result = "";

            switch (DbCommand)
            {
                case IsagCustomProperties.DbCommandType.Merge:
                    result = SqlCreator.GetSqlMerge(this, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, true);
                    break;
                case IsagCustomProperties.DbCommandType.Merge2005:
                    result = SqlCreator.GetSqlMerge2005(this, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, true);
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
                    result = SqlCreator.GetSqlInsert(this, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS, true);
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets a list of input and output column name mappings 
        /// (if bulk insert is used)
        /// </summary>
        /// <returns></returns>
        public List<ColumnMapping> GetBulkColumnMapping()
        {
            List<ColumnMapping> result = null;

            if (UseBulkInsert)
            {
                result = new List<ColumnMapping>();
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Insert)
                        result.Add(new ColumnMapping(config.InputColumnName, config.OutputColumnName));
                }
            }

            return result;
        }

        #endregion

        #region Validate

        /// <summary>
        /// Validate configuration
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>Configuration contains error that cann be corrected?</returns>
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

        /// <summary>
        /// Checks if SCD configuration is correct and returns Isag events if problems are found
        /// </summary>
        /// <param name="events">Isag events</param>
        private void WarnIfScdIsNotValid(IsagEvents events)
        {
            SCDList scdList = new SCDList(ColumnConfigList, DestinationTable);

            string message = "";
            if (!scdList.IsValid(ref message))
                events.Fire(IsagEvents.IsagEventType.Warning, message);

            bool isValid = true;
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (config.IsScdColumn && config.IsScdValidFrom)
                    isValid = false;
            }
            if (!isValid)
                events.Fire(IsagEvents.IsagEventType.Warning, @"You have to choose ""SCD Column"" OR ""SCD ValidFrom"" for one column but not both!");

            isValid = true;
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (string.IsNullOrEmpty(config.ScdTable) && (config.IsScdColumn || config.IsScdValidFrom))
                    isValid = false;
            }
            if (!isValid)
                events.Fire(IsagEvents.IsagEventType.Warning, @"If choosing ""SCD Column"" or ""SCD ValidFrom"" you also have to fill out ""SCD Table"".");

            isValid = true;
            foreach (ColumnConfig config in ColumnConfigList)
            {
                if (!string.IsNullOrEmpty(config.ScdTable) && !config.IsScdColumn && !config.IsScdValidFrom)
                    isValid = false;
            }
            if (!isValid)
                events.Fire(IsagEvents.IsagEventType.Warning, @"If filling out ""SCD Table"" you have to choose ""SCD Column"" or ""SCD ValidFrom"".");
        }

        /// <summary>
        /// Checks if more that one key is selected and returns Isag events if problems are found
        /// </summary>
        /// <param name="events">Isag events</param>
        private void WarnIfMoreThanOneKeyIsSelected(IsagEvents events)
        {
            if (DbCommand == DbCommandType.Merge || DbCommand == DbCommandType.Merge2005)
            {
                int keys = 0;
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Key)
                        keys++;
                }

                if (keys > 1)
                {
                    events.Fire(IsagEvents.IsagEventType.Warning, "Es sind mehrere Keys ausgewählt.");
                }
            }
        }
        /// <summary>
        /// Checks if column names and datatype are valid
        /// Does the mapping contain a row without a match (input column ids are compared) in the SSIS input column collection?
        /// If match is found: Are mappings input columm name and datatype equal to SSIS input column name and datatype?
        /// </summary>
        /// <param name="input">SSIS input</param>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>Are column names and datatypes valid?</returns>
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

                    if (inputColumn.Name != config.InputColumnName || config.DataTypeInput != SqlCreator.GetSQLServerDataTypeFromInput(inputColumn, config.IsGeometryDataType))
                    {
                        events.Fire(IsagEvents.IsagEventType.Error, "The Mapping contains at least one column with a name or datatype differing from the assigned input column!");

                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Does the SSIS input column collection contain an input column without a match in the oclumn configuration list?
        /// </summary>
        /// <param name="vInput">SSIS virtual input</param>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>Does the SSIS input column collection contain an input column without a match in the oclumn configuration list?</returns>
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
        /// If update or insert is set, a destination column has to be set, too. Checks if condition is fullfilled.
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>If update or insert is set, a destination column has to be set, too. Is condition is fullfilled?</returns>
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
        /// Are all columns usage types set to readonly?
        /// </summary>
        /// <param name="vInputColumnCollection">SSIS virtual input column collection</param>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>Are all columns usage types set to readonly?</returns>
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
        /// For database commands other "Bulk Insert" and "Insert", at least one column has to be marked as a key.
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>For database commands other "Bulk Insert" and "Insert", at least one column has to be marked as a key. Is condition fulfilled?</returns>
        private bool IsKeyMissing(IsagEvents events)
        {
            if (DbCommand != DbCommandType.BulkInsert && DbCommand != DbCommandType.BulkInsertRowLock && DbCommand != DbCommandType.Insert)
            {
                foreach (ColumnConfig config in ColumnConfigList)
                {
                    if (config.Key)
                        return false;
                }
            }
            else
                return false;

            events.Fire(IsagEvents.IsagEventType.Error, @"No Key has been selected!");

            return true;
        }

        /// <summary>
        /// If destination column is identity, neither insert nor update must be marked
        /// (exception: custom database command)
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>If destination column is identity, neither insert nor update must be marked. Is condition fulfilled</returns>
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
        /// Connection managers are valid, if 
        /// - main connection is set
        /// 
        /// Sofern die externe Transaktion gewählt ist, 
        /// If external transaction is used
        /// - bulk connection must be set
        /// - main connection must be able to access tthe temporary table created with the bulk connection 
        /// - main connection and bulk connection must not be the same connections
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>Are all connection managers valid?</returns>
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

                if (tempConn is SqlConnection)
                    mainConn = (SqlConnection)tempConn;
                else
                {
                    events.Fire(IsagEvents.IsagEventType.Error, "Only ADO.NET SQL Server connections are supported for the ADO.NET [Main] Connection.");
                    return false;
                }
            }

            runtimeConn = null;

            //Bulk
            if (!UseExternalTransaction && mainConn != null)
                bulkConn = mainConn;
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

                    if (tempConn is SqlConnection)
                        bulkConn = (SqlConnection)tempConn;
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

                if (mainConnectionServer.StartsWith("."))
                    mainConnectionServer = "localhost" + mainConnectionServer.Substring(1);
                if (bulkConnectionServer.StartsWith("."))
                    bulkConnectionServer = "localhost" + bulkConnectionServer.Substring(1);

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

        /// <summary>
        /// Is an output column assigned to an input column twice?
        /// </summary>
        /// <param name="events">Isag events</param>
        /// <returns>Is an output column assigned to an input column twice?
        /// </returns>
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
                    else
                        outputColumns.Add(config.OutputColumnName);
                }
            }

            return false;
        }

        /// <summary>
        /// Does the mapping contian ia destination column, but insert or update is not marked?
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <returns>Does the mapping contian ia destination column, but insert or update is not marked?</returns>
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
