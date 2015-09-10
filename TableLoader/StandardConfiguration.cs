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
    public class StandardConfiguration
    {
        public string ConnectionManagerId { get; set; }

        private Connections _connections;

        private DbConnection _configConnection;
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

        public bool HasConnection { get { return _configConnection != null; } }

        private bool _needsStandardConfiguration;

        public StandardConfiguration(Connections connections)
        {
            _connections = connections;
            InitDesignTimeConnection();
        }

        public StandardConfiguration(IDTSRuntimeConnectionCollection100 connections, bool needsStandardConfiguration)
        {
            _needsStandardConfiguration = needsStandardConfiguration;
            _connections = null;
            InitRunTimeConnection(connections);
        }

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

        public void CloseConnection()
        {
            if (HasConnection && _configConnection.State == ConnectionState.Open) _configConnection.Close();
        }

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
