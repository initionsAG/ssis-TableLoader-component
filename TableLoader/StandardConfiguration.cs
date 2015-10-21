using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Microsoft.SqlServer.Dts.Runtime;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace TableLoader
{
    /// <summary>
    /// Standard configurations 
    /// used for TableLoader properties:
    /// ChunkSizeBulk, ChunkSizeDbCommand, DbTimeout, MaxThreadCount, PreFixInput, PreFixOutput, TableLoaderType, DbCommand, TransactionType
    /// </summary>
    public class StandardConfiguration
    {
        /// <summary>
        /// Connection Manager id
        /// </summary>
        public string ConnectionManagerId { get; set; }

        /// <summary>
        /// SSIS connections
        /// </summary>
        private Connections _connections;

        /// <summary>
        /// configuration connection
        /// </summary>
        private DbConnection _configConnection;

        /// <summary>
        /// configuration connection
        /// </summary>
        public DbConnection ConfigConnection
        {
            get
            {
                if (HasConnection)
                {
                    if (_configConnection.State == ConnectionState.Closed) _configConnection.Open();
                    return _configConnection;
                }

                return null;
            }

        }

        /// <summary>
        /// Is connection set?
        /// </summary>
        public bool HasConnection { get { return _configConnection != null; } }

        /// <summary>
        /// Is standard configuration needed?
        /// </summary>
        private bool _needsStandardConfiguration;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connections">SSIS connections</param>
        public StandardConfiguration(Connections connections)
        {
            _connections = connections;
            InitDesignTimeConnection();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connections">SSIS connections</param>
        /// <param name="needsStandardConfiguration">Is standard configuration needed?</param>
        public StandardConfiguration(IDTSRuntimeConnectionCollection100 connections, bool needsStandardConfiguration)
        {
            _needsStandardConfiguration = needsStandardConfiguration;
            _connections = null;
            InitRunTimeConnection(connections);
        }

        /// <summary>
        /// Initializes connection used at design time
        /// </summary>
        private void InitDesignTimeConnection()
        {
            try
            {
                ConnectionManager conMgr = _connections[Constants.CONNECTION_MANAGER_OLEDB_NAME_CONFIG];
                _configConnection = new OleDbConnection(conMgr.ConnectionString);
                ConnectionManagerId = conMgr.ID;
            }
            catch (Exception)
            {
                try
                {
                    ConnectionManager conMgr = _connections[Constants.CONNECTION_MANAGER_ADO_NAME_CONFIG];
                    _configConnection = (DbConnection)conMgr.AcquireConnection(null);
                    ConnectionManagerId = conMgr.ID;
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Initializes connection used at runtime
        /// </summary>
        private void InitRunTimeConnection(IDTSRuntimeConnectionCollection100 connections)
        {
            try
            {
                IDTSRuntimeConnection100 conMgr = connections[Constants.CONNECTION_MANAGER_NAME_CONFIG];
                _configConnection = new OleDbConnection(conMgr.ConnectionManager.ConnectionString);
                ConnectionManagerId = conMgr.ConnectionManagerID;
            }
            catch (Exception)
            {
                try
                {
                    IDTSRuntimeConnection100 conMgr = connections[Constants.CONNECTION_MANAGER_NAME_CONFIG];
                    _configConnection = (DbConnection)conMgr.ConnectionManager.AcquireConnection(null);
                    ConnectionManagerId = conMgr.ConnectionManagerID;
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Closes connection if open
        /// </summary>
        public void CloseConnection()
        {
            if (HasConnection && _configConnection.State == ConnectionState.Open) _configConnection.Close();
        }

        /// <summary>
        /// Get standard configuration from database
        /// </summary>
        /// <returns>datatable with standard configuration</returns>
        public DataTable GetStandardConfigurationAsDataTable()
        {
            if (HasConnection)
            {
                try
                {
                    DbCommand sqlCom = ConfigConnection.CreateCommand();
                    sqlCom.CommandText = "select * from " + Constants.CFG_TABLE_STANDARD;

                    DbDataAdapter da;

                    if (sqlCom is OleDbCommand) da = new OleDbDataAdapter((OleDbCommand)sqlCom);
                    else da = new SqlDataAdapter((SqlCommand)sqlCom);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;
                }
                catch (Exception ex)
                {
                    if (_needsStandardConfiguration)
                        MessageBox.Show("Loading standard configuration failed: " + Environment.NewLine + ex.Message);
                }
            }

            return new DataTable();

        }

        /// <summary>
        /// Get standard configuration list (i.e. used as itemlist for combobox)
        /// </summary>
        /// <returns>standard configuration list</returns>
        public List<string> GetStandardConfigurationList()
        {
            if (HasConnection)
            {
                DataTable dt = GetStandardConfigurationAsDataTable();

                List<string> itemList = new List<string>();
                itemList.Add("");

                foreach (DataRow row in dt.Rows)
                {
                    itemList.Add(row["Name"].ToString());
                }

                return itemList;
            }

            return new List<string>();
        }

        /// <summary>
        /// Get standard configuration as dictionary
        /// Key: configuration name
        /// Value: data row from datatable
        /// </summary>
        /// <returns>dictionary with standard configurations</returns>
        public Dictionary<string, DataRow> GetStandardConfigurationAsDictionary()
        {
            if (HasConnection)
            {
                DataTable dt = GetStandardConfigurationAsDataTable();
                Dictionary<string, DataRow> cfgList = new Dictionary<string, DataRow>();

                foreach (DataRow row in dt.Rows)
                {
                    cfgList.Add(row["Name"].ToString(), row);
                }

                return cfgList;
            }

            return new Dictionary<string, DataRow>(); ;
        }

        /// <summary>
        /// Apply standard configuration to custom properties
        /// </summary>
        /// <param name="isagCustomProperties">component custom porperties</param>
        public void SetStandardConfiguration(ref IsagCustomProperties isagCustomProperties)
        {
            if (!isagCustomProperties.AutoUpdateStandardConfiguration) return;

            if (!HasConnection)
            {
                throw new Exception("The Connection Manager for the Standard Configuration is missing.");
            }
            else
            {
                DataTable dt = GetStandardConfigurationAsDataTable();
                if (dt.Rows.Count == 0) throw new Exception("The Configuration Table is empty or the Database connection is not valid.");

                Dictionary<string, DataRow> cfgList = GetStandardConfigurationAsDictionary();

                if (!cfgList.ContainsKey(isagCustomProperties.StandarConfiguration))
                    throw new Exception("The Standard Configuration \"" + isagCustomProperties.StandarConfiguration + "\" could not be found in the configuration table.");

                SetStandardConfiguration(ref isagCustomProperties, cfgList[isagCustomProperties.StandarConfiguration]);
            }
        }

        /// <summary>
        /// Apply standard configuration to custom properties
        /// </summary>
        /// <param name="isagCustomProperties">component custom porperties</param>
        /// <param name="row">data row</param>
        public void SetStandardConfiguration(ref IsagCustomProperties isagCustomProperties, DataRow row)
        {
            try
            {
                isagCustomProperties.ChunckSizeBulk = (long)row["ChunkSizeBulk"];
                isagCustomProperties.ChunkSizeDbCommand = (long)row["ChunkSizeDbCommand"];
                isagCustomProperties.TimeOutDb = (int)row["DbTimeout"];
                isagCustomProperties.MaxThreadCount = (long)row["MaxThreadCount"];
                isagCustomProperties.PrefixInput = row["PreFixInput"].ToString();
                isagCustomProperties.PrefixOutput = row["PreFixOutput"].ToString();

                IsagCustomProperties.TableLoaderType tableLoaderType =
                    (IsagCustomProperties.TableLoaderType)Enum.Parse(typeof(IsagCustomProperties.TableLoaderType), row["TableLoaderType"].ToString());
                IsagCustomProperties.DbCommandType dbCommand =
                    (IsagCustomProperties.DbCommandType)Enum.Parse(typeof(IsagCustomProperties.DbCommandType), row["DbCommand"].ToString());
                IsagCustomProperties.TransactionType transaction =
                    (IsagCustomProperties.TransactionType)Enum.Parse(typeof(IsagCustomProperties.TransactionType), row["TransactionType"].ToString());

                isagCustomProperties.TlType = tableLoaderType; 
                isagCustomProperties.DbCommand = dbCommand;
                isagCustomProperties.Transaction = transaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
