using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System.Collections;
using System.Data.SqlClient;
using System.Data.OleDb;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using System.Data.Common;
using ComponentFramework.Controls;
using ComponentFramework.Gui;



namespace TableLoader
{
    public partial class frmTableLoaderUI : Form
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
        private ValueList _outputColumnValueList;
        private Dictionary<string, DataRow> _cfgList = new Dictionary<string, DataRow>();
        private StandardConfiguration _stdConfig;
        //GUI Elemente
        private IsagDataGrid ugMapping = new IsagDataGrid();
        private IsagConnectionManager _ConnectionManagerMain;
        private IsagConnectionManager _ConnectionManagerBulk;
        private IsagUltraComboEditor _cmbTableLoaderType = new IsagUltraComboEditor();
        private IsagUltraComboEditor _cmbDestinationTable = new IsagUltraComboEditor();
        private IsagUltraComboEditor _cmbDbCommand = new IsagUltraComboEditor();
        private IsagUltraComboEditor _cmbTransaction = new IsagUltraComboEditor();
        //private IsagUltraComboEditor _cmbVariablesPreSql = new IsagUltraComboEditor();
        //private IsagUltraComboEditor _cmbVariablesPostSql = new IsagUltraComboEditor();
        //private IsagUltraComboEditor _cmbVariablesCustomCommand = new IsagUltraComboEditor();
        private IsagVariableChooser _cmpVariableChooserCustomCommand = new IsagVariableChooser();
        private IsagVariableChooser _cmpVariableChooserLog = new IsagVariableChooser();
        private IsagVariableChooser _cmpVariableChooserPreSql = new IsagVariableChooser();
        private IsagVariableChooser _cmpVariableChooserPostSql = new IsagVariableChooser();
        private IsagCheckBox _checkUseCustomCommand = new IsagCheckBox();
        private MenuItem _miLimitOutputColumnNames;
        private IsagUltraComboEditor _cmbStandardConfig = new IsagUltraComboEditor();
        private IsagCheckBox _checkStandardConfigAuto = new IsagCheckBox();
        private IsagCheckBox _checkDisableTablock = new IsagCheckBox();
        private IsagCheckBox _checkExcludePreSqlFromTransaction = new IsagCheckBox();

        private DbConnection _configConnection;
        public DbConnection ConfigConnection
        {
            get
            {
                if (_configConnection != null)
                {
                    if (_configConnection.State == ConnectionState.Closed) _configConnection.Open();
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
                        _configConnection = (DbConnection)conMgr.AcquireConnection(null);
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
            InitializeFunctionButtons();
            PopulateOutputColumnList();
            InitStandardConfig();

            this.Text += " " + _IsagCustomProperties.Version;
        }

        #region Initialize



        private void CreateBindings()
        {
            ugMapping.DataSource = _IsagCustomProperties.ColumnConfigList;
            ugMapping.DataBind();

            foreach (UltraGridColumn col in ugMapping.DisplayLayout.Bands[0].Columns)
            {
                col.Header.Column.SortIndicator = SortIndicator.Ascending;
            }

            tbPrefixInput.DataBindings.Add("Text", _IsagCustomProperties, "PrefixInput");
            tbPrefixOutput.DataBindings.Add("Text", _IsagCustomProperties, "PrefixOutput");

            _checkExcludePreSqlFromTransaction.DataBindings.Add("Checked", _IsagCustomProperties, "ExcludePreSqlFromTransaction");
            _checkExcludePreSqlFromTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionUsed");
            lblExcludePreSqlFromTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionUsed");
            _checkDisableTablock.DataBindings.Add("Checked", _IsagCustomProperties, "DisableTablock");
            _checkUseCustomCommand.DataBindings.Add("Checked", _IsagCustomProperties, "UseCustomMergeCommand");
            tbCustomMergeCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");
            tbCustomMergeCommand.DataBindings.Add("Text", _IsagCustomProperties, "CustomMergeCommand");
            btnInsertDefaultMergeCommand.DataBindings.Add("Enabled", _checkUseCustomCommand, "Checked");
            uTabCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "CanUseCustomCommand");

           
            _cmpVariableChooserCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");
            btnInsertVarCustomCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "UseCustomMergeCommand");

            _cmbTableLoaderType.DataBindings.Add("Value", _IsagCustomProperties, "TlType");
            _cmbTableLoaderType.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");

            imgHelpTransactions.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            lblTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            _cmbTransaction.DataBindings.Add("Visible", _IsagCustomProperties, "IsTransactionAvailable");
            _cmbTransaction.DataBindings.Add("Value", _IsagCustomProperties, "Transaction");
            _cmbTransaction.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");
            _cmbDbCommand.DataBindings.Add("Value", _IsagCustomProperties, "DbCommand");
            _cmbDbCommand.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");

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
            //tbTimeout.DataBindings.Add("Enabled", _IsagCustomProperties, "NoAutoUpdateStandardConfiguration");
            tbReattempts.DataBindings.Add("Text", _IsagCustomProperties, "Reattempts");


            _ConnectionManagerBulk.DataBindings.Add("Visible", _IsagCustomProperties, "UseExternalTransaction");
            lblConMgrBulk.DataBindings.Add("Visible", _IsagCustomProperties, "UseExternalTransaction");

            tbPreSql.DataBindings.Add("Text", _IsagCustomProperties, "PreSql");
            tbPostSql.DataBindings.Add("Text", _IsagCustomProperties, "PostSql");

            btnAddRow.DataBindings.Add("Enabled", _IsagCustomProperties, "HasDestinationTable");

            //lblWarning.DataBindings.Add("Visible", _IsagCustomProperties, "UseBulkInsert");

            tbMessage.DataBindings.Add("Text", _IsagCustomProperties, "CustumLoggingTemplate");
            numLogLevel.DataBindings.Add("Value", _IsagCustomProperties, "LogLevel");

            //DestinationTable Auswahl            
            _cmbDestinationTable.DataBindings.Add("Value", _IsagCustomProperties, "DestinationTable");
            PopulateDestinationTableName();

            //Standard Configuration
            _cmbStandardConfig.DataBindings.Add("Value", _IsagCustomProperties, "StandarConfiguration");
            _checkStandardConfigAuto.DataBindings.Add("Checked", _IsagCustomProperties, "AutoUpdateStandardConfiguration");

            uTabConfig.ActiveTabChanged += new Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventHandler(uTabConfig_ActiveTabChanged);
        }

       

        private void InitializeCustomComponents()
        {

            //Mapping Grid
            pnlDGV.Controls.Add(ugMapping);

            //ConnectionManager
            _ConnectionManagerBulk = new IsagConnectionManager(_metadata, _serviceProvider, _connections, Constants.CONNECTION_MANAGER_NAME_BULK);
            pnlConnMgrBulk.Controls.Add(_ConnectionManagerBulk);
            _ConnectionManagerMain = new IsagConnectionManager(_metadata, _serviceProvider, _connections, Constants.CONNECTION_MANAGER_NAME_MAIN);
            pnlConnMgrMain.Controls.Add(_ConnectionManagerMain);

            //TableLoader Type
            pnlTableLoaderType.Controls.Add(_cmbTableLoaderType);
            _cmbTableLoaderType.SetValueList(typeof(IsagCustomProperties.TableLoaderType));

            //Transaktion
            pnlTransaction.Controls.Add(_cmbTransaction);
            _cmbTransaction.SetValueList(typeof(IsagCustomProperties.TransactionType));

            //Destination Table
            pnlDestTable.Controls.Add(_cmbDestinationTable);

            //DB Command
            pnlDbCommand.Controls.Add(_cmbDbCommand);
            _cmbDbCommand.SetValueList(typeof(IsagCustomProperties.DbCommandType), IsagCustomProperties.DB_COMMAND_MERGE_STRING_VALUES);

            //Text Editors
            tbCustomMergeCommand.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            tbPreSql.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            tbPostSql.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            //Custom Command
            _checkUseCustomCommand.Text = "Use Custom Command";
            _checkUseCustomCommand.Dock = DockStyle.Fill;
            pnlUseCustomCommand.Controls.Add(_checkUseCustomCommand);
            _checkUseCustomCommand.CheckedChanged += new EventHandler(_checkUseCustomCommand_CheckedChanged);


            //Variable Chooser
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableLoaderUI));
            Icon icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            _cmpVariableChooserLog.Initialize(_variables, icon);
            pnlVariableChooserLog.Controls.Add(_cmpVariableChooserLog);
            _cmpVariableChooserCustomCommand.Initialize(_variables, icon);
            pnlVariablesCustomCommand.Controls.Add(_cmpVariableChooserCustomCommand);            
            _cmpVariableChooserPreSql.Initialize(_variables, icon);
            pnlVariablesPreSql.Controls.Add(_cmpVariableChooserPreSql);
            _cmpVariableChooserPostSql.Initialize(_variables, icon);
            pnlVariablesPostSql.Controls.Add(_cmpVariableChooserPostSql);


            //Standard Configuration
            pnlCmbStandardConfig.Controls.Add(_cmbStandardConfig);
            //_checkStandardConfigAuto.Text = "AutoLoad Configuration";
            _checkStandardConfigAuto.Dock = DockStyle.Fill;
            _checkStandardConfigAuto.Enabled = false;
            pnlCheckStandardConfigAuto.Controls.Add(_checkStandardConfigAuto);

            //Disable Tablock
            _checkDisableTablock.Dock = DockStyle.Fill;
            pnlCheckDisableTablock.Controls.Add(_checkDisableTablock);

            //Exclude PreSql from transaction
            _checkExcludePreSqlFromTransaction.Dock = DockStyle.Fill;
            pnlExcludePreSqlFromTransaction.Controls.Add(_checkExcludePreSqlFromTransaction);
        }

     

        private void InitializeFunctionButtons()
        {
            ugMapping.DisplayLayout.Bands[0].Columns["Function"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            ugMapping.DisplayLayout.Override.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            ugMapping.ClickCellButton += new CellEventHandler(ugMapping_ClickCellButton);
        }

        private void InitializeContextMenu()
        {

            ugMapping.ContextMenu = new ContextMenu();
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("Select", menuItemLimitOutputColumns_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("DeSelect", menuItemLimitOutputColumns_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            _miLimitOutputColumnNames = new MenuItem("Limit OutputColumnList", menuItemLimitOutputColumns_Click);
            ugMapping.ContextMenu.MenuItems.Add(_miLimitOutputColumnNames);
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("AutoMap", menuItemLimitOutputColumns_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("AutoMap Selection", menuItemLimitOutputColumns_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("Add Row", menuItemLimitOutputColumns_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("Remove Row(s)", menuItemLimitOutputColumns_Click));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("-"));
            ugMapping.ContextMenu.MenuItems.Add(new MenuItem("Remove Selected Mappings", menuItemLimitOutputColumns_Click));
        }

        #endregion

        #region Populate

        /// <summary>
        /// SQL ColumnDefinitions erstellen und den ColumnConfigs zuweisen.
        /// Füllen der Zielspalten-Auswahlisten im Mapping-Grid
        /// </summary>
        private void PopulateOutputColumnList()
        {
            ValueList valueList;
            if (ugMapping.DisplayLayout.ValueLists.IndexOf("Columns") != -1)
            {
                valueList = ugMapping.DisplayLayout.ValueLists["Columns"];
                valueList.ValueListItems.Clear();
            }
            else valueList = this.ugMapping.DisplayLayout.ValueLists.Add("Columns");

            if (GetDesignTimeConnection() != null)
            {
                try
                {

                    _sqlColumns = _IsagCustomProperties.AddSqlColumnDefinitions(GetDesignTimeConnection(), _cmbDestinationTable.Text);

                    foreach (string columnName in _sqlColumns.Keys)
                    {

                        valueList.ValueListItems.Add(columnName);
                    }

                }
                catch (Exception)
                {
                    //Exception wird noch ignoriert, da es vorkommen kann dass der ConnectionManager noch nicht gesetzt ist.
                }
            }

            valueList.SortStyle = ValueListSortStyle.AscendingByValue;
            this.ugMapping.DisplayLayout.Bands[0].Columns["OutputColumnName"].ValueList = valueList;
            this.ugMapping.DisplayLayout.Bands[0].Columns["OutputColumnName"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            _outputColumnValueList = valueList;
        }

        /// <summary>
        /// Die ComboxBox für die Zieltabelle füllen
        /// </summary>
        private void PopulateDestinationTableName()
        {
            string oldValue = _IsagCustomProperties.DestinationTable;

            _cmbDestinationTable.ValueList.ValueListItems.Clear();

            if (GetDesignTimeConnection() != null)
            {
                SqlConnection conn = GetDesignTimeConnection();

                SqlCommand sqlCom = conn.CreateCommand();
                DataTable schema = conn.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_SCHEMA"].ToString() + "." + row["TABLE_NAME"].ToString();
                    _cmbDestinationTable.ValueList.ValueListItems.Add(tableName);

                    if (!string.IsNullOrEmpty(oldValue) && oldValue != tableName && oldValue.ToUpper() == tableName.ToUpper())
                    {
                        _IsagCustomProperties.DestinationTable = tableName;
                        MessageBox.Show("Der gewählte Tabellenname hat sich bzgl. der Groß- und Kleinschreibung automatisch geändert!", "TableLoader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                _cmbDestinationTable.ValueList.SortStyle = ValueListSortStyle.AscendingByValue;
                _cmbDestinationTable.LimitToList = true;
                _cmbDestinationTable.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                _cmbDestinationTable.AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
                _cmbDestinationTable.DropDownStyle = DropDownStyle.DropDown;
                _cmbDestinationTable.Nullable = false;
                _cmbDestinationTable.ShowOverflowIndicator = true;

                conn.Close();
            }
        }

        private void InitStandardConfig()
        {
            try
            {
                _stdConfig = new StandardConfiguration(_connections);
                if (_stdConfig.HasConnection)
                {
                    _cmbStandardConfig.ValueList = _stdConfig.GetStandardConfigurationAsValueList();
                    _cfgList = _stdConfig.GetStandardConfigurationAsDictionary();
                    if (_IsagCustomProperties.AutoUpdateStandardConfiguration && !string.IsNullOrEmpty(_IsagCustomProperties.StandarConfiguration))
                    {
                        _cmbStandardConfig.Value = _IsagCustomProperties.StandarConfiguration;
                        _cmbStandardConfig_SelectionChanged(null, null);
                    }
                }
                else if (_IsagCustomProperties.AutoUpdateStandardConfiguration) throw new Exception("Database Connection to the Standard Configuration is missing, but autoload is enabled.");

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
            //Aktualisieren der Destination Table?
            if ((isMainConnection && !_IsagCustomProperties.UseExternalTransaction) ||
                (!isMainConnection && _IsagCustomProperties.UseExternalTransaction))
                PopulateDestinationTableName();

            UpdateDbCommandList();
        }

        /// <summary>
        /// 1. Aktualisiert die Auswahlliste mit den DB Commands (Merge nicht mit SQL Server 2005 verwendebar)
        /// 2. Aktualisiert die Auswahlliste mit den Transaktionen
        /// </summary>
        private void UpdateDbCommandList()
        {
            //DB Command "Merge" erlaubt?
            ValueListItem item = _cmbDbCommand.ValueList.FindByDataValue(IsagCustomProperties.DbCommandType.Merge);
            bool isSql2005 = IsSqlServer2005();
            if (isSql2005 && item != null)
            {
                if (_cmbDbCommand.SelectedItem == item) _cmbDbCommand.SelectedItem = _cmbDbCommand.ValueList.FindByDataValue(IsagCustomProperties.DbCommandType.Merge2005);
                _cmbDbCommand.ValueList.ValueListItems.Remove(item);
            }
            else if (!isSql2005 && item == null) _cmbDbCommand.ValueList.ValueListItems.Add(IsagCustomProperties.DbCommandType.Merge);

            UpdateTransactionList();
        }

        /// <summary>
        /// Aktualisiert die Auswahlliste mit den Transaktionen (Beim BulkInsert ist eine externe Transaktion ni´cht erlaubt)
        /// </summary>
        private void UpdateTransactionList()
        {
            ValueListItem item = _cmbTransaction.ValueList.FindByDataValue(IsagCustomProperties.TransactionType.External);

            if (_IsagCustomProperties.UseBulkInsert)
            {
                if (item != null)
                    _cmbTransaction.ValueList.ValueListItems.Remove(item);
            }
            else
            {
                if (item == null)
                    _cmbTransaction.ValueList.ValueListItems.Add(IsagCustomProperties.TransactionType.External);
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
                ShowMessage("Da beim Speichern ein Fehler aufgetreten ist, würden Änderungen beim Schließen des TableLoaders verworfen werden. <br/><br/>" +
                            "Soll das Schließen des TableLoaders abgebrochen werden?", "",
                            MessageBoxIcon.Question, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                e.Cancel = _abortClosing;
        }

        /// <summary>
        /// - Initialisieren der Events
        /// - Aktualisiert die Auswahlliste mit den DB Commands 
        /// - Aktualisiert Anzeigeinstellungen der GUI (Warnung bei unterschiedlichen Datentypen, Schreibschutz für nicht Properties setzen, ...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTableLoaderUI_Load(object sender, EventArgs e)
        {
            PopulateOutputColumnList();

            _cmbDbCommand.SelectionChanged += new EventHandler(_DbCommand_SelectionChanged);
            _cmbDestinationTable.SelectionChanged += new EventHandler(_DestinationTable_SelectionChanged);
            _ConnectionManagerMain.SelectionChanged += new EventHandler(_ConnectionManagerMain_SelectionChanged);
            _ConnectionManagerBulk.SelectionChanged += new EventHandler(_ConnectionManagerMain_SelectionChanged);
            _cmbTransaction.SelectionChanged += new EventHandler(_Transaction_SelectionChanged);
            ugMapping.AfterSelectChange += new AfterSelectChangeEventHandler(ugMapping_AfterSelectChange);
            ugMapping.AfterCellUpdate += new CellEventHandler(ugMapping_AfterCellUpdate);
            _cmbStandardConfig.SelectionChanged += new EventHandler(_cmbStandardConfig_SelectionChanged);
            _checkStandardConfigAuto.CheckedChanged += new EventHandler(_checkStandardConfigAuto_CheckedChanged);
            UpdateDbCommandList();

            MarkDifferentVarTypes();
            AdjustSettingsForMerge();
            DisableDefaultValue();

            InitializeContextMenu();
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            _abortClosing = !save();
        }

        private void _cmbStandardConfig_SelectionChanged(object sender, EventArgs e)
        {
            if (_cmbStandardConfig.SelectedItem.DataValue.ToString() != "")
            {
                try
                {
                    DataRow row = _cfgList[_cmbStandardConfig.SelectedItem.DataValue.ToString()];
                    _stdConfig.SetStandardConfiguration(ref _IsagCustomProperties, row);

                    IsagCustomProperties.TableLoaderType tableLoaderType =
                        (IsagCustomProperties.TableLoaderType)Enum.Parse(typeof(IsagCustomProperties.TableLoaderType), row["TableLoaderType"].ToString());
                    IsagCustomProperties.DbCommandType dbCommand =
                        (IsagCustomProperties.DbCommandType)Enum.Parse(typeof(IsagCustomProperties.DbCommandType), row["DbCommand"].ToString());
                    IsagCustomProperties.TransactionType transaction =
                        (IsagCustomProperties.TransactionType)Enum.Parse(typeof(IsagCustomProperties.TransactionType), row["TransactionType"].ToString());

                    _cmbTableLoaderType.Value = tableLoaderType;
                    _cmbDbCommand.Value = dbCommand;
                    _cmbTransaction.Value = transaction;
                    tbChunkSizeBulk.Text = row["ChunkSizeBulk"].ToString();
                    tbChunkSizeDbCommand.Text = row["ChunkSizeDbCommand"].ToString();
                    tbTimeout.Text = row["DbTimeout"].ToString();
                    tbMaxThreadCount.Text = row["MaxThreadCount"].ToString();
                    tbPrefixInput.Text = row["PreFixInput"].ToString();
                    tbPrefixOutput.Text = row["PreFixOutput"].ToString();
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Loading standard configuration failed.", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }



                _checkStandardConfigAuto.Checked = true;
            }

            _checkStandardConfigAuto.Enabled = (_IsagCustomProperties.StandarConfiguration != "");

        }


        private void _checkStandardConfigAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (!_checkStandardConfigAuto.Checked) _cmbStandardConfig.Value = "";
        }




        #region Mapping

        void ugMapping_ClickCellButton(object sender, CellEventArgs e)
        {
            ColumnConfig config = (ColumnConfig)e.Cell.Row.ListObject;
            ValueList items = (ValueList)ugMapping.DisplayLayout.Bands[0].Columns["InputColumnName"].ValueList;

            frmFunctionEditor editor = new frmFunctionEditor(config.InputColumnName, config.DataTypeOutput,
                                                             _IsagCustomProperties.DbCommand, e.Cell.Value.ToString(),
                                                             _IsagCustomProperties.GetInputColumns());

            if (editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                e.Cell.Value = editor.Value;
        }

        private void ugMapping_AfterCellUpdate(object sender, CellEventArgs e)
        {
            MarkDifferentVarTypes(e.Cell.Row);
            if (e.Cell.Column.Key == "OutputColumnName") MarkColumnAsKey(e.Cell.Row);
            MarkAutoIdAsUnused(e.Cell.Row);
            AdjustSettingsForMerge(e.Cell.Row);
            DisableDefaultValue(e.Cell.Row);
            AdjustOutputColumnValueList();
        }

        /// <summary>
        /// Prüfen ob die selektierten Zeilen gelöscht werden können und 
        /// den Button "Remove Row(s)" entsprechend ein- oder ausschalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ugMapping_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            bool canDelete = (ugMapping.Selected.Rows.Count > 0);

            foreach (UltraGridRow row in ugMapping.Selected.Rows)
            {
                ColumnConfig config = (ColumnConfig)row.ListObject;

                if (config.HasInput) canDelete = false;
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

        private void menuItemLimitOutputColumns_Click(object sender, EventArgs e)
        {


            MenuItem item = (MenuItem)sender;

            switch (item.Text)
            {
                case "Limit OutputColumnList":
                    item.Checked = !item.Checked;
                    AdjustOutputColumnValueList();
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
                case "Remove Selected Mappings":
                    RemoveSelectedMappings();
                    break;
                default:
                    break;
            }
        }

        #region AutoMap


        private void AutoMap()
        {
            _IsagCustomProperties.AutoMap();
            AutoMapApply();
        }

        private void AuotMapSelection()
        {
            foreach (UltraGridRow row in ugMapping.Selected.Rows)
            {
                _IsagCustomProperties.AutoMap((ColumnConfig)row.ListObject);
            }

            AutoMapApply();

        }

        private void AutoMapApply()
        {

            ugMapping.DataBind();
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
            ugMapping.DeleteSelectedRows();
        }

        private void RemoveSelectedMappings()
        {
            foreach (UltraGridRow row in ugMapping.Selected.Rows)
            {
                ((ColumnConfig)row.ListObject).RemoveOutput();
                //_IsagCustomProperties.ColumnConfigList[row.Index].RemoveOutput();
                row.Refresh();
            }
        }

        #endregion

        #region StartPage

        private void _ConnectionManagerMain_SelectionChanged(object sender, EventArgs e)
        {
            IsagUltraComboEditor control = (IsagUltraComboEditor)sender;

            ReactOnConnectionManagerChanged(control.Equals(_ConnectionManagerMain));
        }

        private void _Transaction_SelectionChanged(object sender, EventArgs e)
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

        private void _DbCommand_SelectionChanged(object sender, EventArgs e)
        {
            UpdateTransactionList();
            AdjustSettingsForMerge();
            DisableDefaultValue();
        }

        private void _DestinationTable_SelectionChanged(object sender, EventArgs e)
        {
            _IsagCustomProperties.RemoveOutput();
            PopulateOutputColumnList();
            ShowWarningOnDestinationTableChanged();
        }


        #endregion

        #region Pre-/Post Sql

        private void btnInsertVariablePreSql_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetDataObject("@(" + _cmpVariableChooserPreSql.SelectedVariable + ")", true);
            tbPreSql.EditInfo.Paste();
        }

        private void btnInsertVariablePostSql_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetDataObject("@(" + _cmpVariableChooserPostSql.SelectedVariable + ")", true);
            tbPostSql.EditInfo.Paste();
        }

        private void btnInsertTruncatePreSql_Click(object sender, EventArgs e)
        {
            string destTableName = "";
            if (_cmbDestinationTable.SelectedItem != null) destTableName = _cmbDestinationTable.SelectedItem.ToString();

            System.Windows.Forms.Clipboard.SetDataObject("TRUNCATE TABLE " + destTableName, true);
            tbPreSql.EditInfo.Paste();
        }

        /// <summary>
        /// Färbt den Titel des PostSql-Tabs ein, falls es ein PostSql-Statement gibt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbPostSql_TextChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabPostSqlStatement, tbPostSql.Text != null && tbPostSql.Text != "");
        }

        /// <summary>
        /// Färbt den Titel des PreSql-Tabs ein, falls es ein PreSql-Statement gibt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbPreSql_TextChanged(object sender, EventArgs e)
        {
            SetTabColor(uTabPreSqlStatement, tbPreSql.Text != null && tbPreSql.Text != "");
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
            if (customCommand != "") tbCustomMergeCommand.Value = customCommand;
        }

        private void btnInsertVarCustomCommand_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetDataObject("@(" + _cmpVariableChooserCustomCommand.SelectedVariable + ")", true);
            tbCustomMergeCommand.EditInfo.Paste();
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

        #endregion

        #region StartPage

        /// <summary>
        /// Warnung anzeigen, wenn die Destination Table geändert wurde und das PreSql Statement einen truncate-Befehl enthält
        /// </summary>
        private void ShowWarningOnDestinationTableChanged()
        {
            if (_IsagCustomProperties.PreSql != null && _IsagCustomProperties.PreSql.ToUpper().Contains("TRUNCATE"))
            {
                string message = "The PreSql statement contains a \"TRUNCATE\". <br/>" + "Please validate the statement!";

                UltraMessageBoxInfo info = new UltraMessageBoxInfo();
                info.TextFormatted = message;
                info.Header = "Warning: The destination table has changed.";
                info.Caption = "TableLoader";
                info.Icon = MessageBoxIcon.Information;
                ultraMessageBox.ShowMessageBox(info);
            }
        }



        /// <summary>
        /// Den Hilfetext für die StandardConfigs anzeigen
        /// </summary>
        private void ShowHelpStandardConfig()
        {
            string message = "If checked the values for Chunk Size, DB Timeout, MaxThreadCount, PreFixInput, PostFixInput, Transaktion Type and DB Command <br/>" +
                             "will always be used from the database table TL_CFG which must be accessable by the Connection Manager CN_CONFIG (or CN_CONFIG_ADO).<br/><br/>" +
                             "Leaving this option unchecked you can use it as a template by selecting a standard configuration.";

            UltraMessageBoxInfo info = new UltraMessageBoxInfo();
            info.TextFormatted = message;
            info.Header = "Help: Standard Configuration";
            info.Caption = "TableLoader";
            info.Icon = MessageBoxIcon.Information;
            ultraMessageBox.ShowMessageBox(info);
        }


        /// <summary>
        /// Den Hilfetext für die Transaktionen anzeigen
        /// </summary>
        private void ShowHelpTransaction()
        {
            string message = "If using the external Transaction you have to <br/><br/>" +
                                "1. surround the DFT with Execute SQL Tasks (Begin Transaction, Commit, Rollback). <br/>" +
                                @"2. make sure that the Connection (Main) of the TableLoader is identical to Connection of the SQL Task.<br/>   Also ""RetainSameConnection"" has to be activated. <br/>" +
                                "3. make sure that no trancaction is assigned to the TableLoaders Connection (Bulk). <br/> <br/>" +
                                "A Bulk Insert with an external Transaction is not possible!";

            UltraMessageBoxInfo info = new UltraMessageBoxInfo();
            info.TextFormatted = message;
            info.Header = "Help: Transactions";
            info.Caption = "TableLoader";
            info.Icon = MessageBoxIcon.Information;
            ultraMessageBox.ShowMessageBox(info);
        }

        private void ShowHelpChunkSize()
        {
            string message = "ChunkSize (Bulk) is the number of rows that will be written <br/>" +
            "by a Bulk Copy command to the temporary table. <br/><br/>" +
            "If using the DB Command \"Bulk Insert\" the desination is the destination table instead of the temporary table.";

            UltraMessageBoxInfo info = new UltraMessageBoxInfo();
            info.TextFormatted = message;
            info.Header = "Help: ChunkSize (Bulk)";
            info.Caption = "TableLoader";
            info.Icon = MessageBoxIcon.Information;
            ultraMessageBox.ShowMessageBox(info);
        }
        /// <summary>
        /// Den Dialog zum Erzeugen der Zieltabelle (anhand des Inputs) anzeigen
        /// </summary>
        private void CreateDestinationTable()
        {
            SqlConnection con = GetDesignTimeConnection(true);

            if (con != null)
            {
                _cmbDestinationTable.DataBindings["Value"].ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
                frmCreateTable frm = new frmCreateTable(_IsagCustomProperties, GetDesignTimeConnection());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PopulateDestinationTableName();
                    _cmbDestinationTable.SelectedItem = _cmbDestinationTable.ValueList.FindByDataValue(frm.getTableName());
                }

                frm.Dispose();
            }

        }

        private void AlterDestinationTable()
        {
            SqlConnection con = GetDesignTimeConnection(true);

            if (con != null)
            {
                _cmbDestinationTable.DataBindings["Value"].ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
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
                frmCreateTable frm = new frmCreateTable(_IsagCustomProperties, _IsagCustomProperties.ColumnConfigList , GetDesignTimeConnection());
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
            UltraMessageBoxInfo info = new UltraMessageBoxInfo();
            info.TextFormatted = message;
            info.Header = header;
            info.Caption = "TableLoader";
            info.Icon = icon;
            info.Buttons = buttons;
            return ultraMessageBox.ShowMessageBox(info);

        }

        /// <summary>
        /// Überprüfen es sich bei der DesignTimeConnection um den SQL Server 2005 handelt
        /// </summary>
        /// <returns></returns>
        private bool IsSqlServer2005()
        {
            if (GetDesignTimeConnection() == null) return false;

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
                if (_IsagCustomProperties.UseExternalTransaction) connection = _ConnectionManagerBulk.SelectedConnection;
                else connection = _ConnectionManagerMain.SelectedConnection;

                if (connection.State == ConnectionState.Closed) connection.Open();
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
            foreach (UltraGridRow row in ugMapping.Rows)
            {
                MarkColumnAsKey(row);
            }
        }
        /// <summary>
        /// Falls eine Output Column Primary Key ist wird "Key" aktiviert
        /// </summary>
        /// <param name="row"></param>
        private void MarkColumnAsKey(UltraGridRow row)
        {
            ColumnConfig config = (ColumnConfig)row.ListObject;

            config.Key = config.IsOutputPrimaryKey;

            row.Update();
        }

        private void MarkAutoIdAsUnused()
        {
            foreach (UltraGridRow row in ugMapping.Rows)
            {
                MarkAutoIdAsUnused(row);
            }
        }
        /// <summary>
        /// Falls es sich bei der Output Column um eine "AutoId" handelt, so werden Use Insert&Update deaktiviert
        /// </summary>
        /// <param name="row"></param>
        private void MarkAutoIdAsUnused(UltraGridRow row)
        {
            ColumnConfig config = (ColumnConfig)row.ListObject;

            if (config.IsOutputAutoId)
            {
                //config.Insert = false;
                config.Update = false;

                //row.Cells["Insert"].Activation = Activation.NoEdit;
                row.Cells["Update"].Activation = Activation.NoEdit;
            }
            else
            {
                row.Cells["Insert"].Activation = Activation.AllowEdit;
                row.Cells["Update"].Activation = Activation.AllowEdit;
            }

            row.Update();
        }

        private void MarkDifferentVarTypes()
        {
            foreach (UltraGridRow row in ugMapping.Rows)
            {
                MarkDifferentVarTypes(row);
            }
        }
        /// <summary>
        /// Unterscheiden sich Output- und InputDatatype, so werden beide Zellen rot markiert
        /// </summary>
        private void MarkDifferentVarTypes(UltraGridRow row)
        {
            ColumnConfig config = (ColumnConfig)row.ListObject;

            if (config.HasInput && config.HasOutput && config.DataTypeInput != config.DataTypeOutput)
            {
                row.Cells["DataTypeInput"].Appearance.BackColor = Color.LightCoral;
                row.Cells["DataTypeOutput"].Appearance.BackColor = Color.LightCoral;
            }
            else
            {
                row.Cells["DataTypeInput"].Appearance.BackColor = Color.LightGray;
                row.Cells["DataTypeOutput"].Appearance.BackColor = Color.LightGray;
            }
        }


        private void AdjustSettingsForMerge()
        {
            foreach (UltraGridRow row in ugMapping.Rows)
            {
                AdjustSettingsForMerge(row);
            }
        }
        private void AdjustSettingsForMerge(UltraGridRow row)
        {
            ColumnConfig config = (ColumnConfig)row.ListObject;

            if (_IsagCustomProperties.UseMerge && config.Key)
            {
                config.Update = false;
                //config.Insert = !config.IsOutputAutoId;

                //row.Cells["Insert"].Activation = Activation.NoEdit;
                row.Cells["Update"].Activation = Activation.NoEdit;
            }
            else
            {
                row.Cells["Insert"].Activation = Activation.AllowEdit;
                row.Cells["Update"].Activation = Activation.AllowEdit;
                MarkAutoIdAsUnused(row);
            }

            row.Update();
        }

        private void DisableDefaultValue()
        {
            foreach (UltraGridRow row in ugMapping.Rows)
            {
                DisableDefaultValue(row);
            }
        }
        private void DisableDefaultValue(UltraGridRow row)
        {
            ColumnConfig config = (ColumnConfig)row.ListObject;
            if (_IsagCustomProperties.UseBulkInsert)
            {
                row.Cells["Default"].Activation = Activation.NoEdit;
                row.Cells["Function"].Activation = Activation.NoEdit;
            }
            else if (!config.Insert && config.Update)
            {
                row.Cells["Default"].Activation = Activation.NoEdit;
                row.Cells["Function"].Activation = Activation.AllowEdit;
            }
            else
            {
                row.Cells["Default"].Activation = Activation.AllowEdit;
                row.Cells["Function"].Activation = Activation.AllowEdit;
            }
        }

        private ValueList GetLimitedOutputColumnValueList()
        {
            ValueList result = new ValueList();

            foreach (ValueListItem item in _outputColumnValueList.ValueListItems)
            {
                if (!_IsagCustomProperties.IsOutputColumnAssigned(item.DataValue.ToString())) result.ValueListItems.Add(item.DataValue);
            }

            return result;
        }

        private void AdjustOutputColumnValueList()
        {
            this.ugMapping.DisplayLayout.Bands[0].Columns["OutputColumnName"].ValueList =
                _miLimitOutputColumnNames.Checked ? GetLimitedOutputColumnValueList() : _outputColumnValueList;
        }

        /// <summary>
        /// Setzt die Farbe des Titels eines Tabs auf "gehighlightet" oder Standard-
        /// "gehighlightet" heißt, dass im Inhalt etwas (z.B. Custom Merge Command) aktiviert wurde.
        /// </summary>
        /// <param name="tabControl">das Tab, das (nicht) markiert werden soll</param>
        /// <param name="highlight">Soll der Titel des Tabs farbig markiert werden?</param>
        private void SetTabColor(Infragistics.Win.UltraWinTabControl.UltraTabPageControl tabControl, bool highlight)
        {
            tabControl.Tab.Appearance.ForeColor = highlight ? Color.Green : Color.FromKnownColor(KnownColor.ControlText);
        }
        #endregion

        private void CloseConfigConnection()
        {
            if (_configConnection != null && _configConnection.State == ConnectionState.Open) _configConnection.Close();
        }

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

                ShowMessage("The Custom Properties could not be saved! <br/><br/>" + ex.ToString(),
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
            else if (_ConnectionManagerBulk.Value != null && _connections.Contains(_ConnectionManagerBulk.Value.ToString()))
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
                        ShowMessage("The Bulk ConnectionManager could not be saved! <br/><br/>" + ex.ToString(),
                                    "TableLoader: Save", MessageBoxIcon.Error);
                        _stdConfig.CloseConnection();
                        return false;
                    }

                }
                con.ConnectionManagerID = _connections[_ConnectionManagerBulk.Value.ToString()].ID;
            }

            try
            {
                _metadata.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN].ConnectionManagerID =
                _connections[_ConnectionManagerMain.Value.ToString()].ID;
            }
            catch (Exception ex)
            {
                ShowMessage("The Main ConnectionManager could not be saved! <br/><br/>" + ex.ToString(),
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
                ShowMessage("The Config ConnectionManager could not be saved! <br/><br/>" + ex.ToString(),
                             "TableLoader: Save", MessageBoxIcon.Error);
                _stdConfig.CloseConnection();
                return false;
            }



            return true;

        }

        #region Logging



        private void btnInsert_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetDataObject("@(" + _cmpVariableChooserLog.SelectedVariable + ")", true);
            tbMessage.EditInfo.Paste();
        }

        #endregion

        #region View

        private void ShowColumn(UltraGridColumn col)
        {
            col.Hidden = false;
        }

        private void HideColumn(UltraGridColumn col)
        {
            col.Hidden = true;
        }

        private void ShowAllColumns()
        {
            foreach (UltraGridColumn col in ugMapping.DisplayLayout.Bands[0].Columns)
            {
                ShowColumn(col);
            }
        }

        private void ShowMinimalLayout()
        {
            ShowAllColumns();

            ColumnsCollection columns = ugMapping.DisplayLayout.Bands[0].Columns;
            columns["Default"].Hidden = true;
            columns["Function"].Hidden = true;
            columns["IsOutputPrimaryKey"].Hidden = true;
            columns["AllowOutputDbNull"].Hidden = true;
            columns["IsOutputAutoId"].Hidden = true;
            columns["IsScdColumn"].Hidden = true;
            columns["ScdTable"].Hidden = true;
            columns["IsScdValidFrom"].Hidden = true;
        }

        private void ShowSCDLayout()
        {
            ShowAllColumns();

            ColumnsCollection columns = ugMapping.DisplayLayout.Bands[0].Columns;
            columns["Default"].Hidden = true;
            columns["Function"].Hidden = true;
            columns["IsOutputPrimaryKey"].Hidden = true;
            columns["AllowOutputDbNull"].Hidden = true;
            columns["IsOutputAutoId"].Hidden = true;
        }

        private void uceLayoutMapping_SelectionChanged(object sender, EventArgs e)
        {
            switch (uceLayoutMapping.SelectedItem.DisplayText)
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

        void uTabConfig_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            uceLayoutMapping.Visible = (e.Tab == uTabMapping.Tab);
            lblLayoutMapping.Visible = (e.Tab == uTabMapping.Tab);
        }
        #endregion

       












































































    }
}
