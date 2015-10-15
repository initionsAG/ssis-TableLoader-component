using ComponentFramework.Controls;
using ComponentFramework.Gui;
using Lookup2.ComponentFramework.Controls;
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
using TableLoader.ComponentFramework;



namespace TableLoader
{
    public partial class frmTableLoaderUI: Form
    {
        #region Properties

        //DTS Members
        private Connections _connections;
        private IDTSComponentMetaData100 _metadata;
        private IServiceProvider _serviceProvider;
        private Variables _variables;

        //Custom Members
        private IsagCustomProperties _IsagCustomProperties;
        private SqlColumnList _sqlColumns;
        private bool _abortClosing = false;

        private Dictionary<string, DataRow> _cfgList = new Dictionary<string, DataRow>();
        private StandardConfiguration _stdConfig;

        //GUI Elemente
        private IsagDataGridView ugMapping = new IsagDataGridView();
        private BindingList<object> _mappingOutputColumnItemSource = new BindingList<object>();
        private List<string> _outputColumnList = new List<string>();

        private IsagConnectionManager _connectionManagerMain = new IsagConnectionManager();
        private IsagConnectionManager _connectionManagerBulk = new IsagConnectionManager();

        private IsagComboBox _cmbDestinationTable = new IsagComboBox();
        private IsagComboBox _cmbTableLoaderType = new IsagComboBox();
        private IsagComboBox _cmbDbCommand = new IsagComboBox();
        private IsagComboBox _cmbTransaction = new IsagComboBox();

        private IsagVariableChooser _cmbVariableChooserCustomCommand = new IsagVariableChooser();
        private IsagVariableChooser _cmbVariableChooserLog = new IsagVariableChooser();
        private IsagVariableChooser _cmbVariableChooserPreSql = new IsagVariableChooser();
        private IsagVariableChooser _cmbVariableChooserPostSql = new IsagVariableChooser();

        private MenuItem _miLimitOutputColumnNames;
        private MenuItem _miRemoveRows;
        private MenuItem _miFunctionEditor;
        private ComboBox _cmbStandardConfig = new ComboBox();

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
                    ConnectionManager conMgr = _connections[Constants.CONNECTION_MANAGER_OLEDB_NAME_CONFIG]; // _metadata.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_CONFIG];
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
            //PopulateOutputColumnList();
            InitStandardConfig();

            this.Text += " " + _IsagCustomProperties.Version;
        }

        #region Initialize
        /// <summary>
        /// Add bindings to the GUI elements
        /// </summary>
        private void CreateBindings()
        {
            //ugMapping.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            ugMapping.DataSource = _IsagCustomProperties.ColumnConfigList; 

          //  Binding cmbDbCommandBinding = new Binding(
            tbPrefixInput.DataBindings.Add("Text", _IsagCustomProperties, "PrefixInput");
            tbPrefixOutput.DataBindings.Add("Text", _IsagCustomProperties, "PrefixOutput");

            _checkExcludePreSqlFromTransaction.DataBindings.Add("Checked", _IsagCustomProperties, "ExcludePreSqlFromTransaction");
            _checkExcludePreSqlFromTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionUsed");
            lblExcludePreSqlFromTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionUsed");
            checkDisableTablock.DataBindings.Add("Checked", _IsagCustomProperties, "DisableTablock");
            _checkUseCustomCommand.DataBindings.Add("Checked", _IsagCustomProperties, "UseCustomMergeCommand");
            tbCustomMergeCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");
            tbCustomMergeCommand.DataBindings.Add("Text", _IsagCustomProperties, "CustomMergeCommand");
            btnInsertDefaultMergeCommand.DataBindings.Add("Enabled", _checkUseCustomCommand, "Checked");
            uTabCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "CanUseCustomCommand");


            _cmbVariableChooserCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");
            btnInsertVarCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");


            _cmbTableLoaderType.DataBindings.Add("SelectedItem", _IsagCustomProperties, "TlType");
            _cmbTableLoaderType.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");

            imgHelpTransactions.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            lblTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            _cmbTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            _cmbTransaction.DataBindings.Add("SelectedItem", _IsagCustomProperties, "Transaction");
            _cmbTransaction.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");


            
            _cmbDbCommand.DataBindings.Add("SelectedValue", _IsagCustomProperties, "DbCommand");
            _cmbDbCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");
            _cmbDbCommand.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
         
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

            //DestinationTable Auswahl            
            _cmbDestinationTable.DataBindings.Add("SelectedItem", _IsagCustomProperties, "DestinationTable");
            PopulateDestinationTableName();
        }

        /// <summary>
        /// Initialize the components / add them to their gui containers
        /// </summary>
        private void InitializeCustomComponents()
        {

            //Mapping Grid
            pnlDGV.Controls.Add(ugMapping);
            ugMapping.Dock = DockStyle.Fill;
            ugMapping.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);
            ugMapping.ColumnHeadersHeight = ugMapping.ColumnHeadersHeight * 2;

            //ConnectionManager
            _connectionManagerBulk.Initialize(_metadata, _serviceProvider, _connections, Constants.CONNECTION_MANAGER_NAME_BULK);
            pnlConnMgrBulk.Controls.Add(_connectionManagerBulk);
            _connectionManagerBulk.Dock = DockStyle.Fill;
            _connectionManagerBulk.TabIndex = 0;
            _connectionManagerMain.Initialize(_metadata, _serviceProvider, _connections, Constants.CONNECTION_MANAGER_NAME_MAIN);
            pnlConnMgrMain.Controls.Add(_connectionManagerMain);
            _connectionManagerMain.Dock = DockStyle.Fill;
            _connectionManagerMain.TabIndex = 20;

            //DestinationTable
            pnlDestinationTanble.Controls.Add(_cmbDestinationTable);
            _cmbDestinationTable.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            _cmbDestinationTable.AutoCompleteSource = AutoCompleteSource.ListItems;
            _cmbDestinationTable.Sorted = true;
            _cmbDestinationTable.UpdateSelectedItemBindingOnSelectedIndexChanged = true;
            _cmbDestinationTable.Dock = DockStyle.Fill;
            _cmbDestinationTable.TabIndex = 40;

            //TableLoader Type
            _cmbTableLoaderType.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbTableLoaderType.UpdateSelectedItemBindingOnSelectedIndexChanged = true;
            _cmbTableLoaderType.SetItemList(typeof(IsagCustomProperties.TableLoaderType));
            _cmbTableLoaderType.Dock = DockStyle.Fill;
            pnlTableLoaderType.Controls.Add(_cmbTableLoaderType);
            _cmbTableLoaderType.TabIndex = 10;

            //Transaction
            pnlTransaction.Controls.Add(_cmbTransaction);
            _cmbTransaction.SetItemList(typeof(IsagCustomProperties.TransactionType));
            _cmbTransaction.Dock = DockStyle.Fill;
            _cmbTransaction.UpdateSelectedItemBindingOnSelectedIndexChanged = true;
            _cmbTransaction.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbTransaction.TabIndex = 30;

            //DB Command
            pnlDbCommand.Controls.Add(_cmbDbCommand);
            _cmbDbCommand.Dock = DockStyle.Fill;
            _cmbDbCommand.DropDownStyle = ComboBoxStyle.DropDownList;
            ItemDataSource dbCommandItemSource = new ItemDataSource(typeof(IsagCustomProperties.DbCommandType), IsagCustomProperties.DB_COMMAND_MERGE_STRING_VALUES);
            _cmbDbCommand.SetItemDataSource(dbCommandItemSource);
            _cmbDbCommand.TabIndex = 130;

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


            //Standard Configuration
            pnlCmbStandardConfig.Controls.Add(_cmbStandardConfig);
            _cmbStandardConfig.Dock = DockStyle.Fill;
            _cmbStandardConfig.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbStandardConfig.Sorted = true;
            _cmbStandardConfig.TabIndex = 150;

            //Layout
            cmbLayoutMapping.SelectedIndex = 0;


        }


        /// <summary>
        /// - Initialisieren der Events
        /// - Aktualisiert die Auswahlliste mit den DB Commands 
        /// - Aktualisiert Anzeigeinstellungen der GUI (Warnung bei unterschiedlichen Datentypen, Schreibschutz für nicht Properties setzen, ...)
        /// </summary>
        private void FinishInitializingAfterFormLoad()
        {
            PopulateOutputColumnList();
            ugMapping.AddCellBoundedComboBox("OutputColumnName", _mappingOutputColumnItemSource,true);

            _cmbDbCommand.SelectedIndexChanged += new EventHandler(_DbCommand_SelectedIndexChanged);
            _connectionManagerMain.ConnectionManagerChanged += new EventHandler(connectionManagerChanged);
            _connectionManagerBulk.ConnectionManagerChanged += new EventHandler(connectionManagerChanged);
            _cmbTransaction.SelectedIndexChanged += new EventHandler(_cmbTransaction_SelectedIndexChanged);
            _cmbDestinationTable.SelectedIndexChanged += new EventHandler(cmbDestinationTable_SelectedIndexChanged);
            ugMapping.SelectionChanged += ugMapping_SelectionChanged;
            ugMapping.CellValueChanged += ugMapping_CellValueChanged;
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
            ugMapping.ContextMenu = new ContextMenu();
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("Select", menuItem_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("DeSelect", menuItem_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            _miLimitOutputColumnNames = new MenuItem("Limit OutputColumnList", menuItem_Click);
            ugMapping.ContextMenu.MenuItems.Add(_miLimitOutputColumnNames);
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("AutoMap", menuItem_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("AutoMap Selection", menuItem_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("Add Row", menuItem_Click));
            _miRemoveRows = new MenuItem("Remove Row(s)", menuItem_Click);
            ugMapping.ContextMenu.MenuItems.Add(_miRemoveRows);
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            _miFunctionEditor = new MenuItem("Function Editor", menuItem_Click);
            ugMapping.ContextMenu.MenuItems.Add(_miFunctionEditor);

            ugMapping.MouseDown += ugMapping_MouseDown;
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
                bool showFunctionEditor = ugMapping.Columns[ugMapping.CurrentCell.ColumnIndex].Name == "Function" && !ugMapping.CurrentCell.ReadOnly;
                _miFunctionEditor.Visible = showFunctionEditor;
                ugMapping.ContextMenu.MenuItems[_miFunctionEditor.Index - 1].Visible = showFunctionEditor;

                //Show context menu
                ugMapping.ContextMenu.Show(ugMapping, new Point(e.X, e.Y));
            }
        }

        /// <summary>
        /// Exexutes the choosen context menu item
        /// </summary>
        /// <param name="sender"></param>
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
        /// SQL ColumnDefinitions erstellen und den ColumnConfigs zuweisen.
        /// Füllen der Zielspalten-Auswahlisten im Mapping-Grid
        /// </summary>
        private void PopulateOutputColumnList()
        {
            _outputColumnList.Clear();

            if (GetDesignTimeConnection() != null)
            {
                try
                {

                    _sqlColumns = _IsagCustomProperties.AddSqlColumnDefinitions(GetDesignTimeConnection(), _cmbDestinationTable.Text);

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
        /// Die ComboxBox für die Zieltabelle füllen
        /// </summary>
        private void PopulateDestinationTableName()
        {
            string oldValue = _IsagCustomProperties.DestinationTable;

            _cmbDestinationTable.Items.Clear();

            if (GetDesignTimeConnection() != null)
            {
                SqlConnection conn = GetDesignTimeConnection();

                SqlCommand sqlCom = conn.CreateCommand();
                DataTable schema = conn.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_SCHEMA"].ToString() + "." + row["TABLE_NAME"].ToString();
                    _cmbDestinationTable.Items.Add(tableName);

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
        /// 1. Aktualisiert die Auswahlliste mit den Zieltabellen (sofern sich die DesignTimeConnection geändert hat)
        /// 2. Aktualisiert die Auswahlliste mit den DB Commands
        /// </summary>
        /// <param name="isMainConnection"></param>
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
        /// 1. Aktualisiert die Auswahlliste mit den DB Commands (Merge nicht mit SQL Server 2005 verwendebar)
        /// 2. Aktualisiert die Auswahlliste mit den Transaktionen
        /// </summary>
        private void UpdateDbCommandList()
        {
            //DB Command "Merge" is available if SQL Server Version != 2005
            ItemDataSource items = (ItemDataSource) _cmbDbCommand.DataSource;

            if (IsSqlServer2005())
                items.RemoveItem(IsagCustomProperties.DbCommandType.Merge);
            else
                items.AddItem(IsagCustomProperties.DbCommandType.Merge);

            //Dpending on the DB Command not all transactions are available
            UpdateTransactionList();
        }

        /// <summary>
        /// Aktualisiert die Auswahlliste mit den Transaktionen (Beim BulkInsert ist eine externe Transaktion nicht erlaubt)
        /// </summary>
        private void UpdateTransactionList()
        {
            bool externalInItems = _cmbTransaction.Items.Contains(IsagCustomProperties.TransactionType.External);

            if (_IsagCustomProperties.UseBulkInsert)
            {
                if (externalInItems)
                    _cmbTransaction.Items.Remove(IsagCustomProperties.TransactionType.External);
            }
            else
            {
                if (!externalInItems)
                    _cmbTransaction.Items.Add(IsagCustomProperties.TransactionType.External);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Schließen des Fensters verhindern, falls beim Speichern ein Fehler aufgetreten ist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTableLoaderUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_abortClosing && this.DialogResult == System.Windows.Forms.DialogResult.OK &&
                ShowMessage("Error while saving, changes would be lost. \n" +
                            "Abort closing the TableLoader", "",
                            MessageBoxIcon.Question, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                e.Cancel = _abortClosing;
        }

        private void frmTableLoaderUI_Load(object sender, EventArgs e)
        {
            FinishInitializingAfterFormLoad();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _abortClosing = !save();
        }

        /// <summary>
        /// Change properties according to the choosen standard configuration.
        /// The Checkbox check stated that determines if standard configuration is set to "checked".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    _cmbTableLoaderType.SelectedItem = tableLoaderType;
                    _cmbDbCommand.SelectedItem = dbCommand;
                    _cmbTransaction.SelectedItem = transaction;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            ColumnConfig config = (ColumnConfig) ugMapping.CurrentRow.DataBoundItem;
            ;

            frmFunctionEditor editor = new frmFunctionEditor(config.InputColumnName, config.DataTypeOutput,
                                                             _IsagCustomProperties.DbCommand, ugMapping.CurrentCell.Value.ToString(),
                                                             _IsagCustomProperties.GetInputColumns());

            if (editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ugMapping.CurrentCell.Value = editor.Value;
        }

        /// <summary>
        /// Only Rows without an input column can be remove. 
        /// The Button for removing rows has to be enabled/disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugMapping_SelectionChanged(object sender, EventArgs e)
        {
            AdjustRemoveRowsButton();
            if (ugMapping.CurrentRow != null) DisableDefaultValue(ugMapping.CurrentRow);  //necesarry only when grid is displayed the first time
        }

        /// <summary>
        /// If a cell value changed, several other cells (value, properties like color/readonly) might change. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugMapping_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
            if (ugMapping.Columns[e.ColumnIndex].Name == "OutputColumnName")
            {

                AdjustOutputColumnItemList();
                MarkDifferentVarTypes(ugMapping.CurrentRow);
                MarkColumnAsKey(ugMapping.CurrentRow);
            }
           
            MarkAutoIdAsUnused(ugMapping.CurrentRow);
            AdjustSettingsForMerge(ugMapping.CurrentRow);
            DisableDefaultValue(ugMapping.CurrentRow);
        }

        /// <summary>
        /// Prüfen ob die selektierten Zeilen gelöscht werden können und 
        /// den Button "Remove Row(s)" entsprechend ein- oder ausschalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void AdjustRemoveRowsButton()
        {
            bool canDelete = (ugMapping.SelectedRows.Count > 0);

            foreach (DataGridViewRow row in ugMapping.SelectedRows)
            {
                ColumnConfig config = (ColumnConfig) row.DataBoundItem;

                if (config.HasInput)
                    canDelete = false;
            }

            btnRemoveRow.Enabled = canDelete;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DoSelect();
        }

        private void btnDeSelect_Click(object sender, EventArgs e)
        {
            DoDeSelect();
        }

        private void btnAutoMap_Click(object sender, EventArgs e)
        {
            AutoMap();
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void btnRemoveRow_Click(object sender, EventArgs e)
        {
            RemoveRows();
        }

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
            foreach (DataGridViewRow row in ugMapping.SelectedRows)
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
            ugMapping.SelectCheckBoxes(true);
        }

        private void DoDeSelect()
        {
            ugMapping.SelectCheckBoxes(false);
        }

        private void AddRow()
        {
            _IsagCustomProperties.AddColumnConfig(_sqlColumns);
        }

        private void RemoveRows()
        {
            ugMapping.RemoveSelectedRows();
        }

        #endregion



        #region Pre-/Post Sql

        private void btnInsertVariablePreSql_Click(object sender, EventArgs e)
        {
            tbPreSql.Text = tbPreSql.Text.Insert(tbPreSql.SelectionStart, "@(" + _cmbVariableChooserPreSql.SelectedVariable + ")");
        }

        private void btnInsertVariablePostSql_Click(object sender, EventArgs e)
        {
            tbPostSql.Text = tbPostSql.Text.Insert(tbPostSql.SelectionStart, "@(" + _cmbVariableChooserPostSql.SelectedVariable + ")");
        }

        private void btnInsertTruncatePreSql_Click(object sender, EventArgs e)
        {
            string destTableName = "";
            if (_cmbDestinationTable.SelectedItem != null)
                destTableName = _cmbDestinationTable.SelectedItem.ToString();

            tbPreSql.Text = tbPreSql.Text.Insert(tbPreSql.SelectionStart, "TRUNCATE TABLE " + destTableName);
        }

        /// <summary>
        /// Färbt den Titel des PostSql-Tabs ein, falls es ein PostSql-Statement gibt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbPostSql_TextChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabPostSQLStatement, tbPostSql.Text != null && tbPostSql.Text != "");
        }

        /// <summary>
        /// Färbt den Titel des PreSql-Tabs ein, falls es ein PreSql-Statement gibt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbPreSql_TextChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabPreSQLStatement, tbPreSql.Text != null && tbPreSql.Text != "");
        }

        /// <summary>
        /// Färbt den Titel des Custom Command-Tabs ein, falls es das Custom Command aktiviert wurde 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _checkUseCustomCommand_CheckedChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabCustomCommand, _checkUseCustomCommand.Checked);
        }

        #endregion

        #region Sql Preview

        private void btnSqlPreview_Click(object sender, EventArgs e)
        {
            tbSqlPreview.Text = SqlCreator.GetCreateTempTable(_metadata.InputCollection[Constants.INPUT_NAME], _IsagCustomProperties, "#tempTable")
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

        private void btnInsertDefaultCustomCommand_Click(object sender, EventArgs e)
        {

            string customCommand = _IsagCustomProperties.GetCustomCommand();
            if (customCommand != "")
                tbCustomMergeCommand.Text = customCommand;
        }

        private void btnInsertVarCustomCommand_Click(object sender, EventArgs e)
        {
            tbCustomMergeCommand.Text = tbCustomMergeCommand.Text.Insert(tbCustomMergeCommand.SelectionStart, "@(" + _cmbVariableChooserCustomCommand.SelectedVariable + ")");
        }

        /// <summary>
        /// Färbt den Titel des CustomCommand-Tabs ein, falls "CustomCommand" aktiviert ist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uceUseCustomMergeCommand_CheckedChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabCustomCommand, _checkUseCustomCommand.Checked);
        }

        #endregion

        #region StartPage

        #region Events

        private void connectionManagerChanged(object sender, EventArgs e)
        {
            IsagConnectionManager control = (IsagConnectionManager) sender;

            ReactOnConnectionManagerChanged(control.Equals(_connectionManagerMain));
        }

        private void _cmbTransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDbCommandList();
        }

        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            CreateDestinationTable();
        }

        private void btnAlterTable_Click(object sender, EventArgs e)
        {
            AlterDestinationTable();
        }

        private void btnCreateScdTable_Click(object sender, EventArgs e)
        {
            CreateScdTable();
        }

        private void imgHelpTransactions_Click(object sender, EventArgs e)
        {
            ShowHelpTransaction();
        }

        private void imgHelpChunkSize_Click(object sender, EventArgs e)
        {
            ShowHelpChunkSize();
        }

        private void imgHelpStandardConfig_Click(object sender, EventArgs e)
        {
            ShowHelpStandardConfig();
        }

        private void _DbCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTransactionList();
            AdjustSettingsForMerge();
            DisableDefaultValue();
        }

        private void cmbDestinationTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            _IsagCustomProperties.RemoveOutput();
            PopulateOutputColumnList();
            ShowWarningOnDestinationTableChanged();
        }

        #endregion
        /// <summary>
        /// Warnung anzeigen, wenn die Destination Table geändert wurde und das PreSql Statement einen truncate-Befehl enthält
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
        /// Den Hilfetext für die StandardConfigs anzeigen
        /// </summary>
        private void ShowHelpStandardConfig()
        {
            IsagMessageBox messageBox = new IsagMessageBox();
            messageBox.SetHelpText(Properties.Resources.Help_StdConfig);
            messageBox.Show();
        }

        /// <summary>
        /// Den Hilfetext für die Transaktionen anzeigen
        /// </summary>
        private void ShowHelpTransaction()
        {
            IsagMessageBox messageBox = new IsagMessageBox();
            messageBox.SetHelpText(Properties.Resources.Help_Transaction);
            messageBox.Show();
        }

        private void ShowHelpChunkSize()
        {
            IsagMessageBox messageBox = new IsagMessageBox();
            messageBox.SetHelpText(Properties.Resources.Help_ChunkSize);
            messageBox.Show();
        }
        /// <summary>
        /// Den Dialog zum Erzeugen der Zieltabelle (anhand des Inputs) anzeigen
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
                    _cmbDestinationTable.Text = frm.getTableName();
                    //_cmbDestinationTable.SelectedItem = _cmbDestinationTable.ValueList.FindByDataValue(frm.getTableName());
                }

                frm.Dispose();
            }

        }

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
        /// Message mit OK-Button anzeigen anzeigen 
        /// </summary>
        /// <returns>DialogResult</returns>
        private DialogResult ShowMessage(string message, string header, MessageBoxIcon icon)
        {
            return ShowMessage(message, header, icon, MessageBoxButtons.OK);
        }
        /// <summary>
        /// Message anzeigen
        /// </summary>
        /// <returns>DialogResult</returns>
        private DialogResult ShowMessage(string message, string header, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "TableLoader", buttons, icon);


        }

        /// <summary>
        /// Überprüfen es sich bei der DesignTimeConnection um den SQL Server 2005 handelt
        /// </summary>
        /// <returns></returns>
        private bool IsSqlServer2005()
        {
            if (GetDesignTimeConnection() == null)
                return false;

            return GetDesignTimeConnection().ServerVersion.StartsWith("09.");
        }



        /// <summary>
        /// Die genutzte Connection für den DB Zugriff (DesignTimeConnection) ist davon abhängig ob eine externe Transaktion verwendet wird:
        /// keine/interne Transaktion -> Main Connection, externe Transaktion -> Bulk Connection
        ///  
        /// Hintergrund: Wenn RetainSameConnection=True, dann kann die Connection zur DesignTime nicht genutzt werden.
        /// </summary>
        /// <returns>SqlConnection: Die DesignTimeConnection</returns>
        private SqlConnection GetDesignTimeConnection()
        {
            return GetDesignTimeConnection(false);
        }
        /// <summary>
        /// Die genutzte Connection für den DB Zugriff (DesignTimeConnection) ist davon abhängig ob eine externe Transaktion verwendet wird:
        /// keine/interne Transaktion -> Main Connection, externe Transaktion -> Bulk Connection
        ///  
        /// Hintergrund: Wenn RetainSameConnection=True, dann kann die Connection zur DesignTime nicht genutzt werden.
        /// </summary>
        /// 
        /// <param name="throwError">Fehlerausgabe falls kein ConnectionManager gewählt wurde oder keine Verbindung aufgebaut werden kann?</param>
        /// <returns>SqlConnection: Die DesignTimeConnection</returns>
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

        private void MarkColumnAsKey()
        {
            foreach (DataGridViewRow row in ugMapping.Rows)
            {
                MarkColumnAsKey(row);
            }
        }
        /// <summary>
        /// Falls eine Output Column Primary Key ist wird "Key" aktiviert
        /// </summary>
        /// <param name="row"></param>
        private void MarkColumnAsKey(DataGridViewRow row)
        {
            ColumnConfig config = (ColumnConfig) row.DataBoundItem;

            config.Key = config.IsOutputPrimaryKey;
        }

        private void MarkAutoIdAsUnused()
        {
            foreach (DataGridViewRow row in ugMapping.Rows)
            {
                MarkAutoIdAsUnused(row);
            }
        }
        /// <summary>
        /// Falls es sich bei der Output Column um eine "AutoId" handelt, so werden Use Insert&Update deaktiviert
        /// </summary>
        /// <param name="row"></param>
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

            //row.Update();
        }

        private void MarkDifferentVarTypes()
        {
            foreach (DataGridViewRow row in ugMapping.Rows)
            {
                MarkDifferentVarTypes(row);
            }
        }
        /// <summary>
        /// Unterscheiden sich Output- und InputDatatype, so werden beide Zellen rot markiert
        /// </summary>
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

        private void AdjustSettingsForMerge()
        {
            foreach (DataGridViewRow row in ugMapping.Rows)
            {
                AdjustSettingsForMerge(row);
            }
        }
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

            //row.Update();
        }

        private void DisableDefaultValue()
        {
            foreach (DataGridViewRow row in ugMapping.Rows)
            {
                DisableDefaultValue(row);
            }
        }
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
        /// Setzt die Farbe des Titels eines Tabs auf "gehighlightet" oder Standard-
        /// "gehighlightet" heißt, dass im Inhalt etwas (z.B. Custom Merge Command) aktiviert wurde.
        /// </summary>
        /// <param name="tab">das Tab, das (nicht) markiert werden soll</param>
        /// <param name="highlight">Soll der Titel des Tabs farbig markiert werden?</param>
        private void SetTabColor(TabPage tab, bool highlight)
        {
            tab.ForeColor = highlight ? Color.Green : Color.FromKnownColor(KnownColor.ControlText);
        }
        #endregion



        /// <summary>
        /// - Speichern der Custom Properties
        /// - Speichern des MainConnectionManagers
        /// - Speichern oder Löschen des BulkConnectionManagers
        /// </summary>
        /// 
        /// <returns>Speichern erfolgreich?</returns>
        private bool save()
        {
            try
            {
                _IsagCustomProperties.Save(_metadata);
                //Configuration.SaveConfiguration(_variables, _metadata, _IsagCustomProperties);
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



        private void btnInsert_Click(object sender, EventArgs e)
        {
            tbMessage.Text = tbMessage.Text.Insert(tbMessage.SelectionStart, "@(" + _cmbVariableChooserLog.SelectedVariable + ")");
        }

        #endregion

        #region View

        private void ShowColumn(DataGridViewColumn col)
        {
            col.Visible = true;
        }

        private void HideColumn(DataGridViewColumn col)
        {
            col.Visible = false;
        }

        private void ShowAllColumns()
        {
            foreach (DataGridViewColumn col in ugMapping.Columns)
            {
                ShowColumn(col);
            }
        }

        private void ShowMinimalLayout()
        {
            ShowAllColumns();

            DataGridViewColumnCollection columns = ugMapping.Columns;
            HideColumn(columns["Default"]);
            HideColumn(columns["Function"]);
            HideColumn(columns["IsOutputPrimaryKey"]);
            HideColumn(columns["AllowOutputDbNull"]);
            HideColumn(columns["IsOutputAutoId"]);
            HideColumn(columns["IsScdColumn"]);
            HideColumn(columns["ScdTable"]);
            HideColumn(columns["IsScdValidFrom"]);
        }

        private void ShowSCDLayout()
        {
            ShowAllColumns();

            DataGridViewColumnCollection columns = ugMapping.Columns;
            HideColumn(columns["Default"]);
            HideColumn(columns["Function"]);
            HideColumn(columns["IsOutputPrimaryKey"]);
            HideColumn(columns["AllowOutputDbNull"]);
            HideColumn(columns["IsOutputAutoId"]);
        }

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

        private void uTabConfig_TabIndexChanged(object sender, EventArgs e)
        {
            cmbLayoutMapping.Visible = (uTabConfig.SelectedTab == uTabMapping);
            lblLayoutMapping.Visible = (uTabConfig.SelectedTab == uTabMapping);
        }

        private void CloseConfigConnection()
        {
            if (_configConnection != null && _configConnection.State == ConnectionState.Open)
                _configConnection.Close();
        }















































































    }
}
