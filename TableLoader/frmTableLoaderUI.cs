using ComponentFramework;
using ComponentFramework.Controls;
using ComponentFramework.Gui;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TableLoader.Framework.Gui;




namespace TableLoader {
    /// <summary>
    /// The GUI for the TableLoader component
    /// </summary>
    public partial class frmTableLoaderUI: Form {
        #region Properties

        //DTS Members
        private Connections _connections;

        /// <summary>
        /// SSIS metadata for the component
        /// </summary>
        private IDTSComponentMetaData100 _metadata;

        /// <summary>
        /// Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
        /// </summary>
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// SSIS variables
        /// </summary>
        private Variables _variables;

        //Custom Members

        /// <summary>
        /// custom properties for the component
        /// </summary>
        private IsagCustomProperties _IsagCustomProperties;

        /// <summary>
        /// Sql column list for destination table
        /// </summary>
        private SqlColumnList _sqlColumns;

        /// <summary>
        /// Abort closing the GUI?
        /// </summary>
        private bool _abortClosing = false;

        /// <summary>
        /// Dictionary of standard configuration names (key) and standard configuration properties (value)
        /// </summary>
        private Dictionary<string, DataRow> _cfgList = new Dictionary<string, DataRow>();

        /// <summary>
        /// Standard configuration
        /// </summary>
        private StandardConfiguration _stdConfig;

        /// <summary>
        /// DataGrid DataSource
        /// </summary>
        private BindingList<object> _mappingOutputColumnItemSource = new BindingList<object>();

        /// <summary>
        /// DataGrid DataSources for combobox column itemlist containing output column names
        /// </summary>
        private List<string> _outputColumnList = new List<string>();

        //GUI Elemente

        /// <summary>
        /// Isag Connection Manager (Main comnnection) GUI element
        /// </summary>
        private IsagConnectionManager _connectionManagerMain = new IsagConnectionManager();


        /// <summary>
        /// Isag Connection Manager (Bulk comnnection) GUI element
        /// </summary>
        private IsagConnectionManager _connectionManagerBulk = new IsagConnectionManager();

        /// <summary>
        /// Variable Chooser GUI element (for custom command)
        /// </summary>
        private IsagVariableChooser _cmbVariableChooserCustomCommand = new IsagVariableChooser();
        /// <summary>
        /// Variable Chooser GUI element (for logging)
        /// </summary>
        private IsagVariableChooser _cmbVariableChooserLog = new IsagVariableChooser();
        /// <summary>
        /// Variable Chooser GUI element (for pre sql command)
        /// </summary>
        private IsagVariableChooser _cmbVariableChooserPreSql = new IsagVariableChooser();
        /// <summary>
        /// Variable Chooser GUI element (for post sql command)
        /// </summary>
        private IsagVariableChooser _cmbVariableChooserPostSql = new IsagVariableChooser();

        /// <summary>
        /// Menu item (limit output columns)
        /// </summary>
        private MenuItem _miLimitOutputColumnNames;

        /// <summary>
        /// Menu item (remove rows)
        /// </summary>
        private MenuItem _miRemoveRows;

        /// <summary>
        /// Menu item (function editor)
        /// </summary>
        private MenuItem _miFunctionEditor;

        /// <summary>
        /// DB connection for standard configuration
        /// </summary>
        private DbConnection _configConnection;
        public DbConnection ConfigConnection
        {
            get
            {
                if (_configConnection != null)
                {
                    if (_configConnection.State == ConnectionState.Closed)
                        _configConnection.Open();
                    return _configConnection;
                }

                try
                {
                    ConnectionManager conMgr = _connections[Constants.CONNECTION_MANAGER_OLEDB_NAME_CONFIG];
                    _configConnection = new OleDbConnection(conMgr.ConnectionString);
                    _configConnection.Open();

                    return _configConnection;
                }
                catch (Exception)
                {
                    try
                    {
                        ConnectionManager conMgr = _connections[Constants.CONNECTION_MANAGER_ADO_NAME_CONFIG];
                        _configConnection = (DbConnection) conMgr.AcquireConnection(null);
                        return _configConnection;
                    }
                    catch (Exception)
                    {

                        return null;
                    }
                }

            }

        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connections">SSIS connections</param>
        /// <param name="metadata">SSIS component metadata</param>
        /// <param name="serviceProvider">SSIS service provider</param>
        /// <param name="variables">SSIS variables</param>
        public frmTableLoaderUI(Connections connections,
                                IDTSComponentMetaData100 metadata,
                                IServiceProvider serviceProvider,
                                Variables variables)
        {
            InitializeComponent();

            _connections = connections;
            _metadata = metadata;
            _serviceProvider = serviceProvider;
            _variables = variables;

            InitializeCustomComponents();
            _IsagCustomProperties = IsagCustomProperties.Load(_metadata, false);
            CreateBindings();
            InitializeContextMenu();
            InitStandardConfig();

            this.Text += " " + _IsagCustomProperties.Version;

            cmbLayoutMapping.Visible = false;
            lblLayoutMapping.Visible = false;
        }

        #region Initialize
        /// <summary>
        /// Add bindings to the GUI elements
        /// </summary>
        private void CreateBindings()
        {
            dgvMapping.DataSource = _IsagCustomProperties.ColumnConfigList;

            tbPrefixInput.DataBindings.Add("Text", _IsagCustomProperties, "PrefixInput");
            tbPrefixOutput.DataBindings.Add("Text", _IsagCustomProperties, "PrefixOutput");

            _checkExcludePreSqlFromTransaction.DataBindings.Add("Checked", _IsagCustomProperties, "ExcludePreSqlFromTransaction");
            _checkExcludePreSqlFromTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionUsed");
            lblExcludePreSqlFromTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionUsed");
            checkDisableTablock.DataBindings.Add("Checked", _IsagCustomProperties, "DisableTablock");
            checkAzureCompatible.DataBindings.Add("Checked", _IsagCustomProperties, "AzureCompatible", true, DataSourceUpdateMode.OnPropertyChanged);
            checkEnableScdIndex.DataBindings.Add("Checked", _IsagCustomProperties, "EnableIndexOnSCD");
            _checkUseCustomCommand.DataBindings.Add("Checked", _IsagCustomProperties, "UseCustomMergeCommand");
            tbCustomMergeCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");
            tbCustomMergeCommand.DataBindings.Add("Text", _IsagCustomProperties, "CustomMergeCommand");
            btnInsertDefaultMergeCommand.DataBindings.Add("Enabled", _checkUseCustomCommand, "Checked");
            uTabCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "CanUseCustomCommand");

            _cmbVariableChooserCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");
            btnInsertVarCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");

            cmbTableLoaderType.DataBindings.Add("SelectedItem", _IsagCustomProperties, "TlType");
            cmbTableLoaderType.DataBindings.Add("Enabled", _IsagCustomProperties, "IsTlTypeEditable");

            imgHelpTransactions.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            lblTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            cmbTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            cmbTransaction.DataBindings.Add("SelectedItem", _IsagCustomProperties, "Transaction");
            cmbTransaction.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");

            cmbDbCommand.DataBindings.Add("SelectedValue", _IsagCustomProperties, "DbCommand");
            cmbDbCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration", true, DataSourceUpdateMode.OnPropertyChanged);

            lblMaxThreadCount.DataBindings.Add("Visible", _IsagCustomProperties, "UseMultiThreading");
            tbMaxThreadCount.DataBindings.Add("Visible", _IsagCustomProperties, "UseMultiThreading");
            tbMaxThreadCount.DataBindings.Add("Text", _IsagCustomProperties, "MaxThreadCount");
            tbMaxThreadCount.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");
            tbChunkSizeBulk.DataBindings.Add("Text", _IsagCustomProperties, "ChunckSizeBulk");
            tbChunkSizeBulk.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");
            tbChunkSizeDbCommand.DataBindings.Add("Text", _IsagCustomProperties, "ChunkSizeDbCommand");
            tbChunkSizeDbCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");
            tbChunkSizeDbCommand.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            lblChunkSizeDbCommand.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");

            tbTimeout.DataBindings.Add("Text", _IsagCustomProperties, "TimeOutDb");

            tbReattempts.DataBindings.Add("Text", _IsagCustomProperties, "Reattempts");

            pnlConnMgrBulk.DataBindings.Add("Visible", _IsagCustomProperties, "UseExternalTransaction");
            lblConMgrBulk.DataBindings.Add("Visible", _IsagCustomProperties, "UseExternalTransaction");

            tbPreSql.DataBindings.Add("Text", _IsagCustomProperties, "PreSql");
            tbPostSql.DataBindings.Add("Text", _IsagCustomProperties, "PostSql");

            btnAddRow.DataBindings.Add("Enabled", _IsagCustomProperties, "HasDestinationTable");

            tbMessage.DataBindings.Add("Text", _IsagCustomProperties, "CustumLoggingTemplate");
            numLogLevel1.DataBindings.Add("Text", _IsagCustomProperties, "LogLevel");

            //DestinationTable             
            cmbDestinationTable.DataBindings.Add("SelectedItem", _IsagCustomProperties, "DestinationTable");
            PopulateDestinationTableName();

            dgvMapping.AddCellBoundedComboBox("ScdTimeStampGranularity", typeof(ColumnConfig.ScdTimeStampGranularityType), false);
        }

        /// <summary>
        /// Initialize the components / add them to their gui containers
        /// </summary>
        private void InitializeCustomComponents()
        {
            //ConnectionManager
            _connectionManagerBulk.Initialize(_metadata, _serviceProvider, _connections, Constants.CONNECTION_MANAGER_NAME_BULK);
            pnlConnMgrBulk.Controls.Add(_connectionManagerBulk);
            _connectionManagerBulk.Dock = DockStyle.Fill;
            _connectionManagerBulk.TabIndex = 0;
            _connectionManagerMain.Initialize(_metadata, _serviceProvider, _connections, Constants.CONNECTION_MANAGER_NAME_MAIN);
            pnlConnMgrMain.Controls.Add(_connectionManagerMain);
            _connectionManagerMain.Dock = DockStyle.Fill;
            _connectionManagerMain.TabIndex = 20;

            //TableLoader Type
            cmbTableLoaderType.SetItemList(typeof(IsagCustomProperties.TableLoaderType));

            //Transaction
            cmbTransaction.SetItemList(typeof(IsagCustomProperties.TransactionType));

            //DB Command
            ItemDataSource dbCommandItemSource = new ItemDataSource(typeof(IsagCustomProperties.DbCommandType), IsagCustomProperties.DB_COMMAND_MERGE_STRING_VALUES);
            cmbDbCommand.SetItemDataSource(dbCommandItemSource);

            //Text Editors
            tbCustomMergeCommand.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            tbPreSql.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            tbPostSql.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            //Custom Command
            _checkUseCustomCommand.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            //Variable Chooser
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableLoaderUI));
            Icon icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            _cmbVariableChooserLog.Initialize(_variables, icon);
            pnlVariableChooserLog.Controls.Add(_cmbVariableChooserLog);
            _cmbVariableChooserCustomCommand.Initialize(_variables, icon);
            pnlVariablesCustomCommand.Controls.Add(_cmbVariableChooserCustomCommand);
            _cmbVariableChooserPreSql.Initialize(_variables, icon);
            _cmbVariableChooserPreSql.Dock = DockStyle.Fill;
            pnlVariablesPreSql.Controls.Add(_cmbVariableChooserPreSql);
            _cmbVariableChooserPostSql.Initialize(_variables, icon);
            pnlVariablesPostSql.Controls.Add(_cmbVariableChooserPostSql);

            //Layout
            cmbLayoutMapping.SelectedIndex = 0;
        }


        /// <summary>
        /// - Initialize events
        /// - Update combobox itemlist for database commands 
        /// - Update view properties (warning for different datatype, write protection for properties,...)
        /// </summary>
        private void FinishInitializingAfterFormLoad()
        {
            PopulateOutputColumnList();
            dgvMapping.AddCellBoundedComboBox("OutputColumnName", _mappingOutputColumnItemSource, true);

            cmbDbCommand.SelectedIndexChanged += new EventHandler(_DbCommand_SelectedIndexChanged);
            _connectionManagerMain.ConnectionManagerChanged += new EventHandler(connectionManagerChanged);
            _connectionManagerBulk.ConnectionManagerChanged += new EventHandler(connectionManagerChanged);
            cmbTransaction.SelectedIndexChanged += new EventHandler(_cmbTransaction_SelectedIndexChanged);
            cmbDestinationTable.SelectedIndexChanged += new EventHandler(cmbDestinationTable_SelectedIndexChanged);
            dgvMapping.SelectionChanged += ugMapping_SelectionChanged;
            dgvMapping.CellValueChanged += ugMapping_CellValueChanged;
            _cmbStandardConfig.SelectedValueChanged += new EventHandler(cmbStandardConfig_SelectedIndexChanged);
            cmbStandardConfig_SelectedIndexChanged(null, null);
            UpdateDbCommandList();

            MarkDifferentVarTypes();
            AdjustSettingsForMerge();
            DisableDefaultValue();
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Add context menu to datagrid
        /// </summary>
        private void InitializeContextMenu()
        {
            dgvMapping.ContextMenu = new ContextMenu();
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("Select", menuItem_Click));
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("DeSelect", menuItem_Click));
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            _miLimitOutputColumnNames = new MenuItem("Limit OutputColumnList", menuItem_Click);
            dgvMapping.ContextMenu.MenuItems.Add(_miLimitOutputColumnNames);
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("AutoMap", menuItem_Click));
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("AutoMap Selection", menuItem_Click));
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("Add Row", menuItem_Click));
            _miRemoveRows = new MenuItem("Remove Row(s)", menuItem_Click);
            dgvMapping.ContextMenu.MenuItems.Add(_miRemoveRows);
            dgvMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            _miFunctionEditor = new MenuItem("Function Editor", menuItem_Click);
            dgvMapping.ContextMenu.MenuItems.Add(_miFunctionEditor);

            dgvMapping.MouseDown += ugMapping_MouseDown;
        }

        /// <summary>
        /// Depending on the mouse click position some context menu entries are removed/added, then the context menu is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugMapping_MouseDown(object sender, MouseEventArgs e)
        {
            //Configure & show context menu
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //Show/Hide RemoveRow
                _miRemoveRows.Visible = btnRemoveRow.Enabled;

                //Show/Hide Function Editor
                bool showFunctionEditor = dgvMapping.Columns[dgvMapping.CurrentCell.ColumnIndex].Name == "Function" && !dgvMapping.CurrentCell.ReadOnly;
                _miFunctionEditor.Visible = showFunctionEditor;
                dgvMapping.ContextMenu.MenuItems[_miFunctionEditor.Index - 1].Visible = showFunctionEditor;

                //Show context menu
                dgvMapping.ContextMenu.Show(dgvMapping, new Point(e.X, e.Y));
            }
        }

        /// <summary>
        /// Exexutes the choosen context menu item
        /// </summary>
        /// <param name="sender">event sender</param>
        private void ReactOnContextMenuItemClicked(object sender)
        {
            MenuItem item = (MenuItem) sender;

            switch (item.Text)
            {
                case "Limit OutputColumnList":
                    item.Checked = !item.Checked;
                    AdjustOutputColumnItemList();
                    break;
                case "AutoMap":
                    AutoMap();
                    break;
                case "AutoMap Selection":
                    AuotMapSelection();
                    break;
                case "Select":
                    DoSelect();
                    break;
                case "DeSelect":
                    DoDeSelect();
                    break;
                case "Add Row":
                    AddRow();
                    break;
                case "Remove Row(s)":
                    RemoveRows();
                    break;
                    ;
                case "Function Editor":
                    ShowFunctionEditor();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Populate

        /// <summary>
        /// Get column names of destination table and assign them to combobox itemsource
        /// </summary>
        private void PopulateOutputColumnList()
        {
            _outputColumnList.Clear();

            if (GetDesignTimeConnection() != null)
            {
                try
                {
                    _sqlColumns = _IsagCustomProperties.AddSqlColumnDefinitions(GetDesignTimeConnection(), cmbDestinationTable.Text);

                    foreach (string columnName in _sqlColumns.Keys)
                    {
                        _outputColumnList.Add(columnName);
                    }
                }
                catch (Exception)
                {
                    //Exception wird noch ignoriert, da es vorkommen kann dass der ConnectionManager noch nicht gesetzt ist.
                }
            }
            List<object> completeItemList = new List<object>();
            completeItemList.Add("");
            foreach (string item in _outputColumnList)
                completeItemList.Add(item);

            AdjustOutputColumnItemList();
        }

        /// <summary>
        /// Get available tables for connection manager and assign them to the "destinatination table" combobox 
        /// </summary>
        private void PopulateDestinationTableName()
        {
            string oldValue = _IsagCustomProperties.DestinationTable;

            cmbDestinationTable.Items.Clear();

            if (GetDesignTimeConnection() != null)
            {
                SqlConnection conn = GetDesignTimeConnection();

                SqlCommand sqlCom = conn.CreateCommand();
                DataTable schema = conn.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_SCHEMA"].ToString() + "." + row["TABLE_NAME"].ToString();
                    cmbDestinationTable.Items.Add(tableName);

                    if (!string.IsNullOrEmpty(oldValue) && oldValue != tableName && oldValue.ToUpper() == tableName.ToUpper())
                    {
                        _IsagCustomProperties.DestinationTable = tableName;
                        MessageBox.Show("Lowercase/uppercaseThe of the choosen table has changed automatically!", "TableLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                conn.Close();
            }
        }


        /// <summary>
        /// Get standard configurations from database and fill the combobox
        /// </summary>
        private void InitStandardConfig()
        {
            try
            {
                _stdConfig = new StandardConfiguration(_connections);
                if (_stdConfig.HasConnection)
                {
                    _cmbStandardConfig.Items.AddRange(_stdConfig.GetStandardConfigurationList().ToArray());
                    _cfgList = _stdConfig.GetStandardConfigurationAsDictionary();
                    if (_IsagCustomProperties.AutoUpdateStandardConfiguration && !string.IsNullOrEmpty(_IsagCustomProperties.StandarConfiguration))
                    {
                        _cmbStandardConfig.Text = _IsagCustomProperties.StandarConfiguration;
                        cmbStandardConfig_SelectedIndexChanged(null, null);
                    }
                }
                else if (_IsagCustomProperties.AutoUpdateStandardConfiguration)
                    throw new Exception("Database Connection to the Standard Configuration is missing, but autoload is enabled.");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Loading standard configuration failed: " + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        #region Update

        /// <summary>                          
        /// 1. Updates destination table itemlist (if design time connection manager has been changed)
        /// 2. Updates database command item list
        /// </summary>
        /// <param name="isMainConnection">Main connection has been changed?</param>
        private void ReactOnConnectionManagerChanged(bool isMainConnection)
        {
            //Update destination table if necessary
            if ((isMainConnection && !_IsagCustomProperties.UseExternalTransaction) ||
                (!isMainConnection && _IsagCustomProperties.UseExternalTransaction))
                PopulateDestinationTableName();

            //Depending on the used database some db commands are not available
            UpdateDbCommandList();
        }

        /// <summary>
        /// 1. Updates database command item list (merge is unavailable if SQL Server version less than 2008)
        /// 2. Updates sql transaction item list
        /// </summary>
        private void UpdateDbCommandList()
        {
            //DB Command "Merge" is available if SQL Server Version <= 2005
            ItemDataSource items = (ItemDataSource) cmbDbCommand.DataSource;

            if (IsSqlServer2005())
                items.RemoveItem(IsagCustomProperties.DbCommandType.Merge);
            else
                items.AddItem(IsagCustomProperties.DbCommandType.Merge);

            //Dpending on the DB Command not all transactions are available
            UpdateTransactionList();
        }

        /// <summary>
        /// Updates sql transaction item list (bulk insert is not allowed if external transaction is used)
        /// </summary>
        private void UpdateTransactionList()
        {
            bool externalInItems = cmbTransaction.Items.Contains(IsagCustomProperties.TransactionType.External);

            if (_IsagCustomProperties.UseBulkInsert)
            {
                if (externalInItems)
                    cmbTransaction.Items.Remove(IsagCustomProperties.TransactionType.External);
            }
            else
            {
                if (!externalInItems)
                    cmbTransaction.Items.Add(IsagCustomProperties.TransactionType.External);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Prevent closing window if saving is not possible
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void frmTableLoaderUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_abortClosing && this.DialogResult == System.Windows.Forms.DialogResult.OK &&
                ShowMessage("Error while saving, changes would be lost. \n" +
                            "Abort closing the TableLoader", "",
                            MessageBoxIcon.Question, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                e.Cancel = _abortClosing;
        }

        /// <summary>
        /// Reacts on form loaded event
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void frmTableLoaderUI_Load(object sender, EventArgs e)
        {
            FinishInitializingAfterFormLoad();
        }

        /// <summary>
        /// React on OK button clicked:
        /// - Saves properties
        /// - Sets flag if closing window has to be aborted (if saving failed)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            _abortClosing = !save();
        }

        /// <summary>
        /// Change properties according to the choosen standard configuration.
        /// The Checkbox check stated that determines if standard configuration is set to "checked".
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void cmbStandardConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbStandardConfig.Text != "")
            {

                try
                {
                    DataRow row = _cfgList[_cmbStandardConfig.Text];

                    IsagCustomProperties.TableLoaderType tableLoaderType =
                        (IsagCustomProperties.TableLoaderType) Enum.Parse(typeof(IsagCustomProperties.TableLoaderType), row["TableLoaderType"].ToString());
                    IsagCustomProperties.DbCommandType dbCommand =
                        (IsagCustomProperties.DbCommandType) Enum.Parse(typeof(IsagCustomProperties.DbCommandType), row["DbCommand"].ToString());
                    IsagCustomProperties.TransactionType transaction =
                        (IsagCustomProperties.TransactionType) Enum.Parse(typeof(IsagCustomProperties.TransactionType), row["TransactionType"].ToString());

                    cmbTableLoaderType.SelectedItem = tableLoaderType;
                    cmbDbCommand.SelectedItem = dbCommand;
                    cmbTransaction.SelectedItem = transaction;
                    tbChunkSizeBulk.Text = row["ChunkSizeBulk"].ToString();
                    tbChunkSizeDbCommand.Text = row["ChunkSizeDbCommand"].ToString();
                    tbTimeout.Text = row["DbTimeout"].ToString();
                    tbMaxThreadCount.Text = row["MaxThreadCount"].ToString();
                    tbPrefixInput.Text = row["PreFixInput"].ToString();
                    tbPrefixOutput.Text = row["PreFixOutput"].ToString();
                    _checkStandardConfigAuto.Checked = true;
                    _checkStandardConfigAuto.Enabled = true;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Loading standard configuration failed.", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
            }
            else
            {
                _checkStandardConfigAuto.Checked = false;
                _checkStandardConfigAuto.Enabled = false;
            }

            _IsagCustomProperties.StandarConfiguration = _cmbStandardConfig.Text;

        }

        /// <summary>
        /// The combobox value for the standard condiguration is cleared if the configartion will not be used.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void _checkStandardConfigAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (!_checkStandardConfigAuto.Checked)
                _cmbStandardConfig.Text = "";
        }

        #endregion

        #region Mapping

        /// <summary>
        /// Shows the function editor for a selected cell
        /// </summary>
        private void ShowFunctionEditor()
        {
            ColumnConfig config = (ColumnConfig) dgvMapping.CurrentRow.DataBoundItem;
            ;

            frmFunctionEditor editor = new frmFunctionEditor(config.InputColumnName, config.DataTypeOutput,
                                                             _IsagCustomProperties.DbCommand, dgvMapping.CurrentCell.Value.ToString(),
                                                             _IsagCustomProperties.GetInputColumns());

            if (editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                dgvMapping.CurrentCell.Value = editor.Value;
        }

        /// <summary>
        /// Only Rows without an input column can be remove. 
        /// The Button for removing rows has to be enabled/disabled.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void ugMapping_SelectionChanged(object sender, EventArgs e)
        {
            AdjustRemoveRowsButton();
            if (dgvMapping.CurrentRow != null)
                DisableDefaultValue(dgvMapping.CurrentRow);  //necesarry only when grid is displayed the first time
        }

        /// <summary>
        /// If a cell value changed, several other cells (value, properties like color/readonly) might change. 
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        void ugMapping_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvMapping.Columns[e.ColumnIndex].Name == "OutputColumnName")
            {

                AdjustOutputColumnItemList();
                MarkDifferentVarTypes(dgvMapping.CurrentRow);
                MarkColumnAsKey(dgvMapping.CurrentRow);
            }

            MarkAutoIdAsUnused(dgvMapping.CurrentRow);
            AdjustSettingsForMerge(dgvMapping.CurrentRow);
            DisableDefaultValue(dgvMapping.CurrentRow);
        }

        /// <summary>
        /// Checks if selected rows can be deleted and enables/disables "Remove Row(s)" button.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void AdjustRemoveRowsButton()
        {
            bool canDelete = (dgvMapping.SelectedRows.Count > 0);

            foreach (DataGridViewRow row in dgvMapping.SelectedRows)
            {
                ColumnConfig config = (ColumnConfig) row.DataBoundItem;

                if (config.HasInput)
                    canDelete = false;
            }

            btnRemoveRow.Enabled = canDelete;
        }

        /// <summary>
        /// React on Select button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            DoSelect();
        }

        /// <summary>
        /// React on Deselect button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnDeSelect_Click(object sender, EventArgs e)
        {
            DoDeSelect();
        }
        /// <summary>
        /// React on Autmap button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnAutoMap_Click(object sender, EventArgs e)
        {
            AutoMap();
        }

        /// <summary>
        /// React on Add Row button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        /// <summary>
        /// React on Remove Row button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnRemoveRow_Click(object sender, EventArgs e)
        {
            RemoveRows();
        }

        /// <summary>
        /// React on menu item clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void menuItem_Click(object sender, EventArgs e)
        {
            ReactOnContextMenuItemClicked(sender);
        }




        #region AutoMap


        private void AutoMap()
        {
            _IsagCustomProperties.AutoMap();
            AutoMapApply();
        }

        private void AuotMapSelection()
        {
            foreach (DataGridViewRow row in dgvMapping.SelectedRows)
            {
                _IsagCustomProperties.AutoMap((ColumnConfig) row.DataBoundItem);
            }

            AutoMapApply();

        }

        private void AutoMapApply()
        {
            // ugMapping.RefreshCellBoundComboBox("OutputColumnName");

            MarkColumnAsKey();
            MarkDifferentVarTypes();
            MarkAutoIdAsUnused();
            AdjustSettingsForMerge();
        }

        #endregion

        private void DoSelect()
        {
            dgvMapping.SelectCheckBoxes(true);
        }

        private void DoDeSelect()
        {
            dgvMapping.SelectCheckBoxes(false);
        }

        private void AddRow()
        {
            _IsagCustomProperties.AddColumnConfig(_sqlColumns);
        }

        private void RemoveRows()
        {
            dgvMapping.RemoveSelectedRows();
        }

        #endregion



        #region Pre-/Post Sql

        /// <summary>
        /// Insert selected variable into pre sql statement
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertVariablePreSql_Click(object sender, EventArgs e)
        {
            tbPreSql.Text = tbPreSql.Text.Insert(tbPreSql.SelectionStart, "@(" + _cmbVariableChooserPreSql.SelectedVariable + ")");
        }

        /// <summary>
        /// Insert selected variable into post sql statement
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertVariablePostSql_Click(object sender, EventArgs e)
        {
            tbPostSql.Text = tbPostSql.Text.Insert(tbPostSql.SelectionStart, "@(" + _cmbVariableChooserPostSql.SelectedVariable + ")");
        }

        /// <summary>
        /// Insert truncate table into pre sql statement
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertTruncatePreSql_Click(object sender, EventArgs e)
        {
            string destTableName = "";
            if (cmbDestinationTable.SelectedItem != null)
                destTableName = cmbDestinationTable.SelectedItem.ToString();

            tbPreSql.Text = tbPreSql.Text.Insert(tbPreSql.SelectionStart, "TRUNCATE TABLE " + destTableName);
        }

        /// <summary>
        /// Sets post execute tab color to red, if post execute statement is not empty
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void tbPostSql_TextChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabPostSQLStatement, tbPostSql.Text != null && tbPostSql.Text != "");
        }

        /// <summary>
        /// Sets pre execute tab color to red, if pre execute statement is not empty
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void tbPreSql_TextChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabPreSQLStatement, tbPreSql.Text != null && tbPreSql.Text != "");
        }

        /// <summary>
        /// Sets custom command tab color to red, if custom command is enabled
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void _checkUseCustomCommand_CheckedChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabCustomCommand, _checkUseCustomCommand.Checked);
        }

        #endregion

        #region Sql Preview

        /// <summary>
        /// Fills sql preview textbox with sql command using current properties
        /// </summary>
        /// <param name="sender">evetn sender</param>
        /// <param name="e">evetn arguments</param>
        private void btnSqlPreview_Click(object sender, EventArgs e)
        {
            tbSqlPreview.Text = SqlCreator.GetCreateTempTable(_IsagCustomProperties, "#tempTable")
                                + Environment.NewLine + Environment.NewLine;

            switch (_IsagCustomProperties.DbCommand)
            {
                case IsagCustomProperties.DbCommandType.Merge:
                    tbSqlPreview.Text += SqlCreator.GetSqlMerge(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
                    break;
                case IsagCustomProperties.DbCommandType.Merge2005:
                    tbSqlPreview.Text += SqlCreator.GetSqlMerge2005(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
                    break;
                case IsagCustomProperties.DbCommandType.UpdateTblInsertRow:
                    tbSqlPreview.Text += SqlCreator.GetSqlUpdate(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS) + ";" +
                                        Environment.NewLine + Environment.NewLine +
                                        SqlCreator.GetSqlInsertSP(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
                    break;
                case IsagCustomProperties.DbCommandType.UpdateRowInsertRow:
                    tbSqlPreview.Text += SqlCreator.GetSqlUpdateSP(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS) + ";" +
                                        Environment.NewLine + Environment.NewLine +
                                        SqlCreator.GetSqlInsertSP(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
                    break;
                case IsagCustomProperties.DbCommandType.Insert:
                    tbSqlPreview.Text += SqlCreator.GetSqlInsert(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
                    break;
                case IsagCustomProperties.DbCommandType.BulkInsert:
                    tbSqlPreview.Text = SqlCreator.GetCreateDestinationTable(_IsagCustomProperties);
                    break;
                case IsagCustomProperties.DbCommandType.BulkInsertRowLock:
                    tbSqlPreview.Text = SqlCreator.GetCreateDestinationTable(_IsagCustomProperties);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Custom Command

        /// <summary>
        /// Sets custom command to default value (using current properties)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertDefaultCustomCommand_Click(object sender, EventArgs e)
        {

            string customCommand = _IsagCustomProperties.GetCustomCommand();
            if (customCommand != "")
                tbCustomMergeCommand.Text = customCommand;
        }

        /// <summary>
        /// Insert selected variable into custom command 
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertVarCustomCommand_Click(object sender, EventArgs e)
        {
            tbCustomMergeCommand.Text = tbCustomMergeCommand.Text.Insert(tbCustomMergeCommand.SelectionStart, "@(" + _cmbVariableChooserCustomCommand.SelectedVariable + ")");
        }

        #endregion

        #region StartPage

        #region Events

        /// <summary>
        /// React on connection manager changed event
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void connectionManagerChanged(object sender, EventArgs e)
        {
            IsagConnectionManager control = (IsagConnectionManager) sender;

            ReactOnConnectionManagerChanged(control.Equals(_connectionManagerMain));
        }

        /// <summary>
        /// React on transaction changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void _cmbTransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDbCommandList();
        }

        /// <summary>
        /// React on create table button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            CreateDestinationTable();
        }

        /// <summary>
        /// React on alter table button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnAlterTable_Click(object sender, EventArgs e)
        {
            AlterDestinationTable();
        }

        /// <summary>
        /// React on create SCD table button clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnCreateScdTable_Click(object sender, EventArgs e)
        {
            CreateScdTable();
        }

        /// <summary>
        /// React on Help (transaction) clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void imgHelpTransactions_Click(object sender, EventArgs e)
        {
            ShowHelpTransaction();
        }

        /// <summary>
        /// React on Help (chunk size) clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void imgHelpChunkSize_Click(object sender, EventArgs e)
        {
            ShowHelpChunkSize();
        }

        /// <summary>
        /// React on Help (standard configuration) clicked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void imgHelpStandardConfig_Click(object sender, EventArgs e)
        {
            ShowHelpStandardConfig();
        }

        /// <summary>
        /// React on database command changed 
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void _DbCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTransactionList();
            AdjustSettingsForMerge();
            DisableDefaultValue();
        }

        /// <summary>
        /// React on destination tabloe changed 
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void cmbDestinationTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            _IsagCustomProperties.RemoveOutput();
            PopulateOutputColumnList();
            ShowWarningOnDestinationTableChanged();
        }

        #endregion
        /// <summary>
        /// Show warning if destination table changed and pre sql statement contains truncate
        /// </summary>
        private void ShowWarningOnDestinationTableChanged()
        {
            if (_IsagCustomProperties.PreSql != null && _IsagCustomProperties.PreSql.ToUpper().Contains("TRUNCATE"))
            {
                string message = "The PreSql statement contains a \"TRUNCATE\". \n" + "Please validate the statement!";
                MessageBox.Show(message, "TableLoader Warning: The destination table has changed.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Show help for standard configuration
        /// </summary>
        private void ShowHelpStandardConfig()
        {
            IsagMessageBox messageBox = new IsagMessageBox();
            messageBox.SetHelpText(Properties.Resources.Help_StdConfig);
            messageBox.Show();
        }

        /// <summary>
        /// Show help for transactions
        /// </summary>
        private void ShowHelpTransaction()
        {
            IsagMessageBox messageBox = new IsagMessageBox();
            messageBox.SetHelpText(Properties.Resources.Help_Transaction);
            messageBox.Show();
        }

        /// <summary>
        /// Show help for chunk size
        /// </summary>
        private void ShowHelpChunkSize()
        {
            IsagMessageBox messageBox = new IsagMessageBox();
            messageBox.SetHelpText(Properties.Resources.Help_ChunkSize);
            messageBox.Show();
        }
        /// <summary>
        /// Open window for creating of destination table
        /// </summary>
        private void CreateDestinationTable()
        {
            SqlConnection con = GetDesignTimeConnection(true);

            if (con != null)
            {
                //_cmbDestinationTable.DataBindings["Value"].ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
                frmCreateTable frm = new frmCreateTable(_IsagCustomProperties, GetDesignTimeConnection());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PopulateDestinationTableName();
                    cmbDestinationTable.Text = frm.GetTableName();
                    //_cmbDestinationTable.SelectedItem = _cmbDestinationTable.ValueList.FindByDataValue(frm.getTableName());
                }

                frm.Dispose();
            }

        }

        /// <summary>
        /// Open window for altering of destination table
        /// </summary>
        private void AlterDestinationTable()
        {
            SqlConnection con = GetDesignTimeConnection(true);

            if (con != null)
            {
                //_cmbDestinationTable.DataBindings["Value"].ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
                frmCreateTable frm = new frmCreateTable(_IsagCustomProperties, _sqlColumns, GetDesignTimeConnection());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PopulateOutputColumnList();
                }

                frm.Dispose();
            }
        }

        /// <summary>
        /// Open window for creation of SCD table
        /// </summary>
        private void CreateScdTable()
        {
            SqlConnection con = GetDesignTimeConnection(true);

            if (con != null)
            {
                frmCreateTable frm = new frmCreateTable(_IsagCustomProperties, _IsagCustomProperties.ColumnConfigList, GetDesignTimeConnection());
                frm.ShowDialog();
                frm.Dispose();
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Show message with ok button
        /// </summary>
        /// <param name="message">message text</param>
        /// <param name="header">message header</param>
        /// <param name="icon">message icon</param>
        /// <returns>DialogResult</returns>
        private DialogResult ShowMessage(string message, string header, MessageBoxIcon icon)
        {
            return ShowMessage(message, header, icon, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Show message with ok button
        /// </summary>
        /// <param name="message">message text</param>
        /// <param name="header">message header</param>
        /// <param name="icon">message icon</param>
        /// <param name="buttons">message buttons</param>
        /// <returns>DialogResult</returns>
        private DialogResult ShowMessage(string message, string header, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "TableLoader", buttons, icon);


        }

        /// <summary>
        /// Is SQL Server 2015 used?
        /// </summary>
        /// <returns>Is SQL Server 2015 used?</returns>
        private bool IsSqlServer2005()
        {
            if (GetDesignTimeConnection() == null)
                return false;

            return GetDesignTimeConnection().ServerVersion.StartsWith("09.");
        }

        /// <summary>
        /// The used connection at design time differs is an external transaction is used
        /// No/internal connection: Main connection is used
        /// External connection: Bulk connection is used
        ///  
        /// Reason: For external transactions RetainSameConnection is true for connection mangers. These connection managers cannot be used here.
        /// </summary>
        /// <returns>SqlConnection: design time connection</returns>
        private SqlConnection GetDesignTimeConnection()
        {
            return GetDesignTimeConnection(false);
        }

        /// <summary>
        /// The used connection at design time differs is an external transaction is used
        /// No/internal connection: Main connection is used
        /// External connection: Bulk connection is used
        ///  
        /// Reason: For external transactions RetainSameConnection is true for connection mangers. These connection managers cannot be used here.
        /// </summary>
        /// <param name="throwError">Throw error if connection cannot be opened?</param>
        /// <returns>SqlConnection: design time connection</returns>
        private SqlConnection GetDesignTimeConnection(bool throwError)
        {
            SqlConnection connection;

            try
            {
                if (_IsagCustomProperties.UseExternalTransaction)
                    connection = _connectionManagerBulk.SelectedConnection;
                else
                    connection = _connectionManagerMain.SelectedConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();
            }
            catch (Exception ex)
            {
                if (throwError)
                    MessageBox.Show("The Connection Manager could not be opened."
                                    + Environment.NewLine + Environment.NewLine + ex.ToString(), "TableLoader");
                return null;
            }

            return connection;
        }

        #endregion

        #region Automatische Wert-Anpassungen

        /// <summary>
        /// If output column of a row is primary key, property key will be set
        /// </summary>
        private void MarkColumnAsKey()
        {
            foreach (DataGridViewRow row in dgvMapping.Rows)
            {
                MarkColumnAsKey(row);
            }
        }

        /// <summary>
        /// If output column of the row is primary key, property key will be set
        /// </summary>
        /// <param name="row">mapping row</param>
        private void MarkColumnAsKey(DataGridViewRow row)
        {
            ColumnConfig config = (ColumnConfig) row.DataBoundItem;

            config.Key = config.IsOutputPrimaryKey;
        }

        /// <summary>
        /// If output column of a row is identity, column value cannot be updated
        /// </summary>
        private void MarkAutoIdAsUnused()
        {
            foreach (DataGridViewRow row in dgvMapping.Rows)
            {
                MarkAutoIdAsUnused(row);
            }
        }

        /// <summary>
        /// If output column of a row is identity, column value cannot be updated
        /// </summary>
        /// <param name="row">mapping row</param>
        private void MarkAutoIdAsUnused(DataGridViewRow row)
        {
            ColumnConfig config = (ColumnConfig) row.DataBoundItem;

            if (config.IsOutputAutoId)
            {
                config.Update = false;
                row.Cells["Update"].ReadOnly = true;
            }
            else
            {
                row.Cells["Insert"].ReadOnly = false;
                row.Cells["Update"].ReadOnly = false;
            }
        }

        /// <summary>
        /// If input and output column differ, background color of datatypes has to be red
        /// </summary>
        private void MarkDifferentVarTypes()
        {
            foreach (DataGridViewRow row in dgvMapping.Rows)
            {
                MarkDifferentVarTypes(row);
            }
        }
        /// <summary>
        /// If input and output column differ, background color of datatypes has to be red
        /// </summary>
        /// <param name="row">mapping row</param>
        private void MarkDifferentVarTypes(DataGridViewRow row)
        {
            ColumnConfig config = (ColumnConfig) row.DataBoundItem;

            if (config.HasInput && config.HasOutput && config.DataTypeInput != config.DataTypeOutput)
            {
                row.Cells["DataTypeInput"].Style.BackColor = Color.LightCoral;
                row.Cells["DataTypeOutput"].Style.BackColor = Color.LightCoral;
            }
            else
            {
                row.Cells["DataTypeInput"].Style.BackColor = Color.LightGray;
                row.Cells["DataTypeOutput"].Style.BackColor = Color.LightGray;
            }
        }

        /// <summary>
        /// Update insert/update column properties if database command merge has been (de)selected.
        /// </summary>
        private void AdjustSettingsForMerge()
        {
            foreach (DataGridViewRow row in dgvMapping.Rows)
            {
                AdjustSettingsForMerge(row);
            }
        }

        /// <summary>
        /// Update insert/update column properties if database command merge has been (de)selected.
        /// </summary>
        /// <param name="row">mapping row</param>
        private void AdjustSettingsForMerge(DataGridViewRow row)
        {
            ColumnConfig config = (ColumnConfig) row.DataBoundItem;

            if (_IsagCustomProperties.UseMerge && config.Key)
            {
                config.Update = false;
                row.Cells["Update"].ReadOnly = true;
            }
            else
            {
                row.Cells["Insert"].ReadOnly = false;
                row.Cells["Update"].ReadOnly = false;
                MarkAutoIdAsUnused(row);
            }
        }

        /// <summary>
        /// Enable/Disable default / function value
        /// </summary>
        private void DisableDefaultValue()
        {
            foreach (DataGridViewRow row in dgvMapping.Rows)
            {
                DisableDefaultValue(row);
            }
        }

        /// <summary>
        /// Enable/Disable default / function value
        /// </summary>
        /// <param name="row">mapping row</param>
        private void DisableDefaultValue(DataGridViewRow row)
        {
            ColumnConfig config = (ColumnConfig) row.DataBoundItem;
            if (_IsagCustomProperties.UseBulkInsert)
            {
                row.Cells["Default"].ReadOnly = true;
                row.Cells["Function"].ReadOnly = true;
            }
            else if (!config.Insert && config.Update)
            {
                row.Cells["Default"].ReadOnly = true;
                row.Cells["Function"].ReadOnly = false;
            }
            else
            {
                row.Cells["Function"].ReadOnly = false;
                row.Cells["Default"].ReadOnly = false;
            }
        }

        /// <summary>
        /// Get limited output column list
        /// (only output columns that are not used as destination column are return=
        /// </summary>
        /// <returns>Limited output column list</returns>
        private List<string> GetLimitedOutputColumnValueList()
        {
            List<string> result = new List<string>();

            foreach (string item in _outputColumnList)
            {
                if (!_IsagCustomProperties.IsOutputColumnAssigned(item))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// OutputColumnlist (Itemlist) is filled depending on the choosen database table.
        /// Also the list is limited to those columns that are not already assigned if "Limit OutpuColumnList is choosen"
        /// </summary>
        private void AdjustOutputColumnItemList()
        {
            _mappingOutputColumnItemSource.RaiseListChangedEvents = false;
            _mappingOutputColumnItemSource.Clear();

            List<string> columnList = _miLimitOutputColumnNames.Checked ? GetLimitedOutputColumnValueList() : _outputColumnList;

            foreach (string column in columnList)
            {
                _mappingOutputColumnItemSource.Add(column);
            }

            _mappingOutputColumnItemSource.RaiseListChangedEvents = true;
            _mappingOutputColumnItemSource.Insert(0, "");
        }

        /// <summary>
        /// Set tab header color to mark tab
        /// </summary>
        /// <param name="tab">the tab, that has to be highlighted</param>
        /// <param name="highlight">Highlight the tab?</param>
        private void SetTabColor(TabPage tab, bool highlight)
        {
            tab.ForeColor = highlight ? Color.Green : Color.FromKnownColor(KnownColor.ControlText);
        }
        #endregion

        /// <summary>
        /// - Saves custom properties
        /// - Saves main connection manager
        /// - Saves or deletes bulk connection managers
        /// </summary>
        /// 
        /// <returns>Saving successful?</returns>
        private bool save()
        {
            try
            {
                _IsagCustomProperties.Save(_metadata);
            }
            catch (Exception ex)
            {

                ShowMessage("The Custom Properties could not be saved! \n\n" + ex.ToString(),
                            "TableLoader: Save", MessageBoxIcon.Error);
                _stdConfig.CloseConnection();
                return false;
            }


            IDTSRuntimeConnection100 con;

            if (!_IsagCustomProperties.UseExternalTransaction)
            {

                try
                {
                    con = _metadata.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK];
                    _metadata.RuntimeConnectionCollection.RemoveObjectByID(con.ID);
                }
                catch (Exception)
                {
                    //Die zu löschende Connection gibt es nicht
                }

            }
            else if (_connectionManagerBulk.ConnectionManager != null && _connections.Contains(_connectionManagerBulk.ConnectionManager))
            {
                try
                {
                    con = _metadata.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK];
                }
                catch (Exception)
                {
                    try
                    {
                        con = _metadata.RuntimeConnectionCollection.New();
                        con.Name = Constants.CONNECTION_MANAGER_NAME_BULK;
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("The Bulk ConnectionManager could not be saved! (n/n" + ex.ToString(),
                                    "TableLoader: Save", MessageBoxIcon.Error);
                        _stdConfig.CloseConnection();
                        return false;
                    }

                }
                con.ConnectionManagerID = _connections[_connectionManagerBulk.ConnectionManager].ID;
            }

            try
            {
                _metadata.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN].ConnectionManagerID =
                _connections[_connectionManagerMain.ConnectionManager].ID;
            }
            catch (Exception ex)
            {
                ShowMessage("The Main ConnectionManager could not be saved! /n/n" + ex.ToString(),
                            "TableLoader: Save", MessageBoxIcon.Error);
                _stdConfig.CloseConnection();
                return false;
            }

            try
            {
                if (_stdConfig.HasConnection)
                {
                    IDTSRuntimeConnection100 conConfig;
                    try
                    {
                        conConfig = _metadata.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_CONFIG];
                    }
                    catch (Exception)
                    {
                        conConfig = _metadata.RuntimeConnectionCollection.New();
                    }

                    conConfig.ConnectionManagerID = _stdConfig.ConnectionManagerId;
                    conConfig.Name = Constants.CONNECTION_MANAGER_NAME_CONFIG;
                    _stdConfig.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("The Config ConnectionManager could not be saved! /n/n" + ex.ToString(),
                             "TableLoader: Save", MessageBoxIcon.Error);
                _stdConfig.CloseConnection();
                return false;
            }



            return true;

        }

        #region Logging


        /// <summary>
        /// Insert selected variable into custom log text
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            tbMessage.Text = tbMessage.Text.Insert(tbMessage.SelectionStart, "@(" + _cmbVariableChooserLog.SelectedVariable + ")");
        }

        #endregion

        #region View

        /// <summary>
        /// Sets mapping columns visibility = true
        /// </summary>
        /// <param name="col">mapping column</param>
        private void ShowColumn(DataGridViewColumn col)
        {
            col.Visible = true;
        }

        /// <summary>
        /// Sets mapping columns visibility = false
        /// </summary>
        /// <param name="col">mapping column</param>
        private void HideColumn(DataGridViewColumn col)
        {
            col.Visible = false;
        }

        /// <summary>
        /// Set alls mapping columns visibility = true
        /// </summary>
        private void ShowAllColumns()
        {
            foreach (DataGridViewColumn col in dgvMapping.Columns)
            {
                if (col.Name != "OutputColumnName" && col.Name != "ScdTimeStampGranularity") //Bounded column that should never be visible
                    ShowColumn(col);
            }
        }

        /// <summary>
        /// Set alls mapping columns visibility according to minimal layout
        /// </summary>
        private void ShowMinimalLayout()
        {
            ShowAllColumns();

            DataGridViewColumnCollection columns = dgvMapping.Columns;
            HideColumn(columns["Default"]);
            HideColumn(columns["Function"]);
            HideColumn(columns["IsOutputPrimaryKey"]);
            HideColumn(columns["AllowOutputDbNull"]);
            HideColumn(columns["IsOutputAutoId"]);
            HideColumn(columns["IsScdColumn"]);
            HideColumn(columns["ScdTable"]);
            HideColumn(columns["IsScdValidFrom"]);
            HideColumn(columns[IsagDataGridView.CMB_COLUMN_PREFIX + "ScdTimeStampGranularity"]);
        }

        /// <summary>
        /// Set alls mapping columns visibility according to SCD layout
        /// </summary>
        private void ShowSCDLayout()
        {
            ShowAllColumns();

            DataGridViewColumnCollection columns = dgvMapping.Columns;
            HideColumn(columns["Default"]);
            HideColumn(columns["Function"]);
            HideColumn(columns["IsOutputPrimaryKey"]);
            HideColumn(columns["AllowOutputDbNull"]);
            HideColumn(columns["IsOutputAutoId"]);
        }

        /// <summary>
        /// React on selected mapping layoput changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void cmbLayoutMapping_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbLayoutMapping.Text)
            {
                case "Standard":
                    ShowAllColumns();
                    break;
                case "Minimal":
                    ShowMinimalLayout();
                    break;
                case "SCD":
                    ShowSCDLayout();
                    break;
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Close configuration connection
        /// </summary>
        private void CloseConfigConnection()
        {
            if (_configConnection != null && _configConnection.State == ConnectionState.Open)
                _configConnection.Close();
        }


        /// <summary>
        /// React on tab changed:
        /// 
        /// Adjust layout
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void uTabConfig_Selected(object sender, TabControlEventArgs e)
        {
            cmbLayoutMapping.Visible = (e.TabPage == uTabMapping);
            lblLayoutMapping.Visible = (e.TabPage == uTabMapping);
        }

   
        /// <summary>
        /// Update colors (marking different input and output columns) when datagrid is shown the first time
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void dgvMapping_VisibleChanged(object sender, EventArgs e)
        {
            MarkDifferentVarTypes();
            dgvMapping.VisibleChanged -= dgvMapping_VisibleChanged;      
        }















































































    }
}
