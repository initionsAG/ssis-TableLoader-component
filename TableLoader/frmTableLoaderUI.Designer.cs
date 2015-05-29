namespace TableLoader
{
    partial class frmTableLoaderUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Database Timeout in seconds", Infragistics.Win.ToolTipImage.Default, "", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Maximum number of BulkCopy threads", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Number of rows  used for a single Bulk Copy thread", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Maximum number of BulkCopy threads", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Database Timeout in seconds", Infragistics.Win.ToolTipImage.Default, "", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo6 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Number of rows  used for a single Bulk Copy thread", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableLoaderUI));
            this.uTabConfiguration = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnCreateScdTable = new Infragistics.Win.Misc.UltraButton();
            this.pnlCheckDisableTablock = new System.Windows.Forms.Panel();
            this.pnlCheckStandardConfigAuto = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAlterTable = new Infragistics.Win.Misc.UltraButton();
            this.btnCreateTable = new Infragistics.Win.Misc.UltraButton();
            this.lblConMgrBulk = new Infragistics.Win.Misc.UltraLabel();
            this.tbReattempts = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.tbTimeout = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.tbMaxThreadCount = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.tbChunkSizeDbCommand = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.tbChunkSizeBulk = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pnlTableLoaderType = new System.Windows.Forms.Panel();
            this.pnlTransaction = new System.Windows.Forms.Panel();
            this.pnlCmbStandardConfig = new System.Windows.Forms.Panel();
            this.pnlDbCommand = new System.Windows.Forms.Panel();
            this.lblTlType = new System.Windows.Forms.Label();
            this.pnlDestTable = new System.Windows.Forms.Panel();
            this.lblTransaction = new System.Windows.Forms.Label();
            this.imgHelpStandardConfig = new System.Windows.Forms.PictureBox();
            this.imgHelpChunkSize = new System.Windows.Forms.PictureBox();
            this.imgHelpTransactions = new System.Windows.Forms.PictureBox();
            this.pnlConnMgrBulk = new System.Windows.Forms.Panel();
            this.pnlConnMgrMain = new System.Windows.Forms.Panel();
            this.lblWarning = new System.Windows.Forms.Label();
            this.lbDbCommand = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblMaxThreadCount = new System.Windows.Forms.Label();
            this.lblChunkSizeDbCommand = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblChunkSizeBulk = new System.Windows.Forms.Label();
            this.uTabMapping = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnAddRow = new Infragistics.Win.Misc.UltraButton();
            this.btnRemoveRow = new Infragistics.Win.Misc.UltraButton();
            this.btnSelect = new Infragistics.Win.Misc.UltraButton();
            this.btnDeSelect = new Infragistics.Win.Misc.UltraButton();
            this.tbPrefixOutput = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnAutoMap = new System.Windows.Forms.Button();
            this.tbPrefixInput = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pnlDGV = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.uTabPreSqlStatement = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlExcludePreSqlFromTransaction = new System.Windows.Forms.Panel();
            this.lblExcludePreSqlFromTransaction = new System.Windows.Forms.Label();
            this.btnInsertTruncatePreSql = new Infragistics.Win.Misc.UltraButton();
            this.btnInsertVariablePreSql = new Infragistics.Win.Misc.UltraButton();
            this.pnlVariablesPreSql = new System.Windows.Forms.Panel();
            this.lblVariablesPreSql = new Infragistics.Win.Misc.UltraLabel();
            this.tbPreSql = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uTabPostSqlStatement = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnInsertVariablePostSql = new Infragistics.Win.Misc.UltraButton();
            this.pnlVariablesPostSql = new System.Windows.Forms.Panel();
            this.lblVariablesPostSql = new Infragistics.Win.Misc.UltraLabel();
            this.tbPostSql = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.uTabSqlPreview = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnSqlPreview = new Infragistics.Win.Misc.UltraButton();
            this.tbSqlPreview = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.uTabCustomCommand = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlUseCustomCommand = new System.Windows.Forms.Panel();
            this.btnInsertVarCustomCommand = new Infragistics.Win.Misc.UltraButton();
            this.pnlVariablesCustomCommand = new System.Windows.Forms.Panel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.tbCustomMergeCommand = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.btnInsertDefaultMergeCommand = new Infragistics.Win.Misc.UltraButton();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.pnlVariableChooserLog = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.numLogLevel = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.btnInsert = new Infragistics.Win.Misc.UltraButton();
            this.tbMessage = new Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.ultraMessageBox = new Infragistics.Win.UltraMessageBox.UltraMessageBoxManager(this.components);
            this.uTabConfig = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.uceLayoutMapping = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lblLayoutMapping = new System.Windows.Forms.Label();
            this.uTabConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbReattempts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxThreadCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbChunkSizeDbCommand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbChunkSizeBulk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpStandardConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpChunkSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpTransactions)).BeginInit();
            this.uTabMapping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPrefixOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPrefixInput)).BeginInit();
            this.uTabPreSqlStatement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            this.uTabPostSqlStatement.SuspendLayout();
            this.uTabSqlPreview.SuspendLayout();
            this.uTabCustomCommand.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLogLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTabConfig)).BeginInit();
            this.uTabConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uceLayoutMapping)).BeginInit();
            this.SuspendLayout();
            // 
            // uTabConfiguration
            // 
            this.uTabConfiguration.Controls.Add(this.btnCreateScdTable);
            this.uTabConfiguration.Controls.Add(this.pnlCheckDisableTablock);
            this.uTabConfiguration.Controls.Add(this.pnlCheckStandardConfigAuto);
            this.uTabConfiguration.Controls.Add(this.label3);
            this.uTabConfiguration.Controls.Add(this.btnAlterTable);
            this.uTabConfiguration.Controls.Add(this.btnCreateTable);
            this.uTabConfiguration.Controls.Add(this.lblConMgrBulk);
            this.uTabConfiguration.Controls.Add(this.tbReattempts);
            this.uTabConfiguration.Controls.Add(this.tbTimeout);
            this.uTabConfiguration.Controls.Add(this.tbMaxThreadCount);
            this.uTabConfiguration.Controls.Add(this.tbChunkSizeDbCommand);
            this.uTabConfiguration.Controls.Add(this.tbChunkSizeBulk);
            this.uTabConfiguration.Controls.Add(this.pnlTableLoaderType);
            this.uTabConfiguration.Controls.Add(this.pnlTransaction);
            this.uTabConfiguration.Controls.Add(this.pnlCmbStandardConfig);
            this.uTabConfiguration.Controls.Add(this.pnlDbCommand);
            this.uTabConfiguration.Controls.Add(this.lblTlType);
            this.uTabConfiguration.Controls.Add(this.pnlDestTable);
            this.uTabConfiguration.Controls.Add(this.lblTransaction);
            this.uTabConfiguration.Controls.Add(this.imgHelpStandardConfig);
            this.uTabConfiguration.Controls.Add(this.imgHelpChunkSize);
            this.uTabConfiguration.Controls.Add(this.imgHelpTransactions);
            this.uTabConfiguration.Controls.Add(this.pnlConnMgrBulk);
            this.uTabConfiguration.Controls.Add(this.pnlConnMgrMain);
            this.uTabConfiguration.Controls.Add(this.lblWarning);
            this.uTabConfiguration.Controls.Add(this.lbDbCommand);
            this.uTabConfiguration.Controls.Add(this.label11);
            this.uTabConfiguration.Controls.Add(this.label14);
            this.uTabConfiguration.Controls.Add(this.lblMaxThreadCount);
            this.uTabConfiguration.Controls.Add(this.lblChunkSizeDbCommand);
            this.uTabConfiguration.Controls.Add(this.label2);
            this.uTabConfiguration.Controls.Add(this.label1);
            this.uTabConfiguration.Controls.Add(this.label17);
            this.uTabConfiguration.Controls.Add(this.lblChunkSizeBulk);
            this.uTabConfiguration.Location = new System.Drawing.Point(1, 23);
            this.uTabConfiguration.Name = "uTabConfiguration";
            this.uTabConfiguration.Size = new System.Drawing.Size(913, 371);
            // 
            // btnCreateScdTable
            // 
            this.btnCreateScdTable.Location = new System.Drawing.Point(586, 108);
            this.btnCreateScdTable.Name = "btnCreateScdTable";
            this.btnCreateScdTable.Size = new System.Drawing.Size(95, 21);
            this.btnCreateScdTable.TabIndex = 19;
            this.btnCreateScdTable.Text = "Create SCD ";
            this.btnCreateScdTable.Click += new System.EventHandler(this.btnCreateScdTable_Click);
            // 
            // pnlCheckDisableTablock
            // 
            this.pnlCheckDisableTablock.BackColor = System.Drawing.Color.Transparent;
            this.pnlCheckDisableTablock.Location = new System.Drawing.Point(142, 318);
            this.pnlCheckDisableTablock.Name = "pnlCheckDisableTablock";
            this.pnlCheckDisableTablock.Size = new System.Drawing.Size(22, 21);
            this.pnlCheckDisableTablock.TabIndex = 15;
            // 
            // pnlCheckStandardConfigAuto
            // 
            this.pnlCheckStandardConfigAuto.BackColor = System.Drawing.Color.Transparent;
            this.pnlCheckStandardConfigAuto.Location = new System.Drawing.Point(142, 281);
            this.pnlCheckStandardConfigAuto.Name = "pnlCheckStandardConfigAuto";
            this.pnlCheckStandardConfigAuto.Size = new System.Drawing.Size(22, 21);
            this.pnlCheckStandardConfigAuto.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(18, 284);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Standard Configuration";
            // 
            // btnAlterTable
            // 
            this.btnAlterTable.Location = new System.Drawing.Point(687, 81);
            this.btnAlterTable.Name = "btnAlterTable";
            this.btnAlterTable.Size = new System.Drawing.Size(95, 21);
            this.btnAlterTable.TabIndex = 17;
            this.btnAlterTable.Text = "Alter Table";
            this.btnAlterTable.Click += new System.EventHandler(this.btnAlterTable_Click);
            // 
            // btnCreateTable
            // 
            this.btnCreateTable.Location = new System.Drawing.Point(586, 81);
            this.btnCreateTable.Name = "btnCreateTable";
            this.btnCreateTable.Size = new System.Drawing.Size(95, 21);
            this.btnCreateTable.TabIndex = 17;
            this.btnCreateTable.Text = "Create Table";
            this.btnCreateTable.Click += new System.EventHandler(this.btnCreateTable_Click);
            // 
            // lblConMgrBulk
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.lblConMgrBulk.Appearance = appearance1;
            this.lblConMgrBulk.Location = new System.Drawing.Point(18, 52);
            this.lblConMgrBulk.Name = "lblConMgrBulk";
            this.lblConMgrBulk.Size = new System.Drawing.Size(145, 13);
            this.lblConMgrBulk.TabIndex = 16;
            this.lblConMgrBulk.Text = "Connection Manager (Bulk):";
            // 
            // tbReattempts
            // 
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            this.tbReattempts.Appearance = appearance2;
            this.tbReattempts.BackColor = System.Drawing.SystemColors.Window;
            this.tbReattempts.Location = new System.Drawing.Point(652, 211);
            this.tbReattempts.Name = "tbReattempts";
            this.tbReattempts.Size = new System.Drawing.Size(147, 21);
            this.tbReattempts.TabIndex = 15;
            // 
            // tbTimeout
            // 
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            this.tbTimeout.Appearance = appearance3;
            this.tbTimeout.BackColor = System.Drawing.SystemColors.Window;
            this.tbTimeout.Location = new System.Drawing.Point(170, 211);
            this.tbTimeout.Name = "tbTimeout";
            this.tbTimeout.Size = new System.Drawing.Size(363, 21);
            this.tbTimeout.TabIndex = 15;
            ultraToolTipInfo1.ToolTipText = "Database Timeout in seconds";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbTimeout, ultraToolTipInfo1);
            // 
            // tbMaxThreadCount
            // 
            appearance4.BackColor = System.Drawing.SystemColors.Window;
            this.tbMaxThreadCount.Appearance = appearance4;
            this.tbMaxThreadCount.BackColor = System.Drawing.SystemColors.Window;
            this.tbMaxThreadCount.Location = new System.Drawing.Point(170, 114);
            this.tbMaxThreadCount.Name = "tbMaxThreadCount";
            this.tbMaxThreadCount.Size = new System.Drawing.Size(363, 21);
            this.tbMaxThreadCount.TabIndex = 15;
            ultraToolTipInfo2.ToolTipText = "Maximum number of BulkCopy threads";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbMaxThreadCount, ultraToolTipInfo2);
            // 
            // tbChunkSizeDbCommand
            // 
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.tbChunkSizeDbCommand.Appearance = appearance5;
            this.tbChunkSizeDbCommand.BackColor = System.Drawing.SystemColors.Window;
            this.tbChunkSizeDbCommand.Location = new System.Drawing.Point(170, 165);
            this.tbChunkSizeDbCommand.Name = "tbChunkSizeDbCommand";
            this.tbChunkSizeDbCommand.Size = new System.Drawing.Size(363, 21);
            this.tbChunkSizeDbCommand.TabIndex = 15;
            // 
            // tbChunkSizeBulk
            // 
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.tbChunkSizeBulk.Appearance = appearance6;
            this.tbChunkSizeBulk.BackColor = System.Drawing.SystemColors.Window;
            this.tbChunkSizeBulk.Location = new System.Drawing.Point(170, 140);
            this.tbChunkSizeBulk.Name = "tbChunkSizeBulk";
            this.tbChunkSizeBulk.Size = new System.Drawing.Size(363, 21);
            this.tbChunkSizeBulk.TabIndex = 15;
            ultraToolTipInfo3.ToolTipText = "Number of rows  used for a single Bulk Copy thread";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbChunkSizeBulk, ultraToolTipInfo3);
            // 
            // pnlTableLoaderType
            // 
            this.pnlTableLoaderType.BackColor = System.Drawing.Color.Transparent;
            this.pnlTableLoaderType.Location = new System.Drawing.Point(652, 21);
            this.pnlTableLoaderType.Name = "pnlTableLoaderType";
            this.pnlTableLoaderType.Size = new System.Drawing.Size(147, 21);
            this.pnlTableLoaderType.TabIndex = 14;
            // 
            // pnlTransaction
            // 
            this.pnlTransaction.BackColor = System.Drawing.Color.Transparent;
            this.pnlTransaction.Location = new System.Drawing.Point(652, 48);
            this.pnlTransaction.Name = "pnlTransaction";
            this.pnlTransaction.Size = new System.Drawing.Size(147, 21);
            this.pnlTransaction.TabIndex = 14;
            // 
            // pnlCmbStandardConfig
            // 
            this.pnlCmbStandardConfig.BackColor = System.Drawing.Color.Transparent;
            this.pnlCmbStandardConfig.Location = new System.Drawing.Point(170, 281);
            this.pnlCmbStandardConfig.Name = "pnlCmbStandardConfig";
            this.pnlCmbStandardConfig.Size = new System.Drawing.Size(363, 21);
            this.pnlCmbStandardConfig.TabIndex = 14;
            // 
            // pnlDbCommand
            // 
            this.pnlDbCommand.BackColor = System.Drawing.Color.Transparent;
            this.pnlDbCommand.Location = new System.Drawing.Point(170, 238);
            this.pnlDbCommand.Name = "pnlDbCommand";
            this.pnlDbCommand.Size = new System.Drawing.Size(363, 21);
            this.pnlDbCommand.TabIndex = 14;
            // 
            // lblTlType
            // 
            this.lblTlType.AutoSize = true;
            this.lblTlType.BackColor = System.Drawing.Color.Transparent;
            this.lblTlType.Location = new System.Drawing.Point(586, 24);
            this.lblTlType.Name = "lblTlType";
            this.lblTlType.Size = new System.Drawing.Size(50, 13);
            this.lblTlType.TabIndex = 12;
            this.lblTlType.Text = "TL Type:";
            // 
            // pnlDestTable
            // 
            this.pnlDestTable.BackColor = System.Drawing.Color.Transparent;
            this.pnlDestTable.Location = new System.Drawing.Point(170, 81);
            this.pnlDestTable.Name = "pnlDestTable";
            this.pnlDestTable.Size = new System.Drawing.Size(363, 21);
            this.pnlDestTable.TabIndex = 14;
            // 
            // lblTransaction
            // 
            this.lblTransaction.AutoSize = true;
            this.lblTransaction.BackColor = System.Drawing.Color.Transparent;
            this.lblTransaction.Location = new System.Drawing.Point(586, 51);
            this.lblTransaction.Name = "lblTransaction";
            this.lblTransaction.Size = new System.Drawing.Size(66, 13);
            this.lblTransaction.TabIndex = 12;
            this.lblTransaction.Text = "Transaction:";
            // 
            // imgHelpStandardConfig
            // 
            this.imgHelpStandardConfig.BackgroundImage = global::TableLoader.Properties.Resources.Help;
            this.imgHelpStandardConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgHelpStandardConfig.ErrorImage = null;
            this.imgHelpStandardConfig.Location = new System.Drawing.Point(539, 282);
            this.imgHelpStandardConfig.Name = "imgHelpStandardConfig";
            this.imgHelpStandardConfig.Size = new System.Drawing.Size(23, 20);
            this.imgHelpStandardConfig.TabIndex = 10;
            this.imgHelpStandardConfig.TabStop = false;
            this.imgHelpStandardConfig.Click += new System.EventHandler(this.imgHelpStandardConfig_Click);
            // 
            // imgHelpChunkSize
            // 
            this.imgHelpChunkSize.BackgroundImage = global::TableLoader.Properties.Resources.Help;
            this.imgHelpChunkSize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgHelpChunkSize.ErrorImage = null;
            this.imgHelpChunkSize.Location = new System.Drawing.Point(539, 141);
            this.imgHelpChunkSize.Name = "imgHelpChunkSize";
            this.imgHelpChunkSize.Size = new System.Drawing.Size(23, 20);
            this.imgHelpChunkSize.TabIndex = 10;
            this.imgHelpChunkSize.TabStop = false;
            this.imgHelpChunkSize.Click += new System.EventHandler(this.imgHelpChunkSize_Click);
            // 
            // imgHelpTransactions
            // 
            this.imgHelpTransactions.BackgroundImage = global::TableLoader.Properties.Resources.Help;
            this.imgHelpTransactions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgHelpTransactions.ErrorImage = null;
            this.imgHelpTransactions.Location = new System.Drawing.Point(811, 48);
            this.imgHelpTransactions.Name = "imgHelpTransactions";
            this.imgHelpTransactions.Size = new System.Drawing.Size(23, 20);
            this.imgHelpTransactions.TabIndex = 10;
            this.imgHelpTransactions.TabStop = false;
            this.imgHelpTransactions.Click += new System.EventHandler(this.imgHelpTransactions_Click);
            // 
            // pnlConnMgrBulk
            // 
            this.pnlConnMgrBulk.BackColor = System.Drawing.Color.Transparent;
            this.pnlConnMgrBulk.Location = new System.Drawing.Point(170, 48);
            this.pnlConnMgrBulk.Name = "pnlConnMgrBulk";
            this.pnlConnMgrBulk.Size = new System.Drawing.Size(363, 21);
            this.pnlConnMgrBulk.TabIndex = 3;
            // 
            // pnlConnMgrMain
            // 
            this.pnlConnMgrMain.BackColor = System.Drawing.Color.Transparent;
            this.pnlConnMgrMain.Location = new System.Drawing.Point(170, 21);
            this.pnlConnMgrMain.Name = "pnlConnMgrMain";
            this.pnlConnMgrMain.Size = new System.Drawing.Size(363, 21);
            this.pnlConnMgrMain.TabIndex = 3;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.BackColor = System.Drawing.Color.Transparent;
            this.lblWarning.ForeColor = System.Drawing.Color.Red;
            this.lblWarning.Location = new System.Drawing.Point(558, 242);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(271, 13);
            this.lblWarning.TabIndex = 2;
            this.lblWarning.Text = "Beim BulkInsert müssen alle Zielspalten bedient werden!";
            this.lblWarning.Visible = false;
            // 
            // lbDbCommand
            // 
            this.lbDbCommand.AutoSize = true;
            this.lbDbCommand.BackColor = System.Drawing.Color.Transparent;
            this.lbDbCommand.Location = new System.Drawing.Point(18, 242);
            this.lbDbCommand.Name = "lbDbCommand";
            this.lbDbCommand.Size = new System.Drawing.Size(72, 13);
            this.lbDbCommand.TabIndex = 2;
            this.lbDbCommand.Text = "DB Command";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(18, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(141, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Connection Manager (Main):";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(18, 85);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Destination Table";
            // 
            // lblMaxThreadCount
            // 
            this.lblMaxThreadCount.AutoSize = true;
            this.lblMaxThreadCount.BackColor = System.Drawing.Color.Transparent;
            this.lblMaxThreadCount.Location = new System.Drawing.Point(17, 118);
            this.lblMaxThreadCount.Name = "lblMaxThreadCount";
            this.lblMaxThreadCount.Size = new System.Drawing.Size(95, 13);
            this.lblMaxThreadCount.TabIndex = 2;
            this.lblMaxThreadCount.Text = "Max Thread Count";
            ultraToolTipInfo4.ToolTipText = "Maximum number of BulkCopy threads";
            this.ultraToolTipManager1.SetUltraToolTip(this.lblMaxThreadCount, ultraToolTipInfo4);
            // 
            // lblChunkSizeDbCommand
            // 
            this.lblChunkSizeDbCommand.AutoSize = true;
            this.lblChunkSizeDbCommand.BackColor = System.Drawing.Color.Transparent;
            this.lblChunkSizeDbCommand.Location = new System.Drawing.Point(17, 169);
            this.lblChunkSizeDbCommand.Name = "lblChunkSizeDbCommand";
            this.lblChunkSizeDbCommand.Size = new System.Drawing.Size(135, 13);
            this.lblChunkSizeDbCommand.TabIndex = 2;
            this.lblChunkSizeDbCommand.Text = "Chunk Size (DB Command)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(18, 322);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Disable Tablock";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(586, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Reattempts";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.Location = new System.Drawing.Point(17, 215);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(77, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "DB Timeout [s]";
            ultraToolTipInfo5.ToolTipText = "Database Timeout in seconds";
            this.ultraToolTipManager1.SetUltraToolTip(this.label17, ultraToolTipInfo5);
            // 
            // lblChunkSizeBulk
            // 
            this.lblChunkSizeBulk.AutoSize = true;
            this.lblChunkSizeBulk.BackColor = System.Drawing.Color.Transparent;
            this.lblChunkSizeBulk.Location = new System.Drawing.Point(17, 144);
            this.lblChunkSizeBulk.Name = "lblChunkSizeBulk";
            this.lblChunkSizeBulk.Size = new System.Drawing.Size(91, 13);
            this.lblChunkSizeBulk.TabIndex = 2;
            this.lblChunkSizeBulk.Text = "Chunk Size (Bulk)";
            ultraToolTipInfo6.ToolTipText = "Number of rows  used for a single Bulk Copy thread";
            this.ultraToolTipManager1.SetUltraToolTip(this.lblChunkSizeBulk, ultraToolTipInfo6);
            // 
            // uTabMapping
            // 
            this.uTabMapping.Controls.Add(this.btnAddRow);
            this.uTabMapping.Controls.Add(this.btnRemoveRow);
            this.uTabMapping.Controls.Add(this.btnSelect);
            this.uTabMapping.Controls.Add(this.btnDeSelect);
            this.uTabMapping.Controls.Add(this.tbPrefixOutput);
            this.uTabMapping.Controls.Add(this.btnAutoMap);
            this.uTabMapping.Controls.Add(this.tbPrefixInput);
            this.uTabMapping.Controls.Add(this.pnlDGV);
            this.uTabMapping.Controls.Add(this.label5);
            this.uTabMapping.Controls.Add(this.label6);
            this.uTabMapping.Location = new System.Drawing.Point(-10000, -10000);
            this.uTabMapping.Name = "uTabMapping";
            this.uTabMapping.Size = new System.Drawing.Size(913, 371);
            // 
            // btnAddRow
            // 
            this.btnAddRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddRow.Enabled = false;
            this.btnAddRow.Location = new System.Drawing.Point(685, 344);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(97, 21);
            this.btnAddRow.TabIndex = 22;
            this.btnAddRow.Text = "Add Row";
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // btnRemoveRow
            // 
            this.btnRemoveRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveRow.Enabled = false;
            this.btnRemoveRow.Location = new System.Drawing.Point(788, 344);
            this.btnRemoveRow.Name = "btnRemoveRow";
            this.btnRemoveRow.Size = new System.Drawing.Size(97, 21);
            this.btnRemoveRow.TabIndex = 23;
            this.btnRemoveRow.Text = "Remove Row(s)";
            this.btnRemoveRow.Click += new System.EventHandler(this.btnRemoveRow_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelect.Location = new System.Drawing.Point(491, 343);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(74, 23);
            this.btnSelect.TabIndex = 17;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDeSelect
            // 
            this.btnDeSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeSelect.Location = new System.Drawing.Point(571, 343);
            this.btnDeSelect.Name = "btnDeSelect";
            this.btnDeSelect.Size = new System.Drawing.Size(74, 23);
            this.btnDeSelect.TabIndex = 18;
            this.btnDeSelect.Text = "Deselect";
            this.btnDeSelect.Click += new System.EventHandler(this.btnDeSelect_Click);
            // 
            // tbPrefixOutput
            // 
            this.tbPrefixOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.tbPrefixOutput.Appearance = appearance7;
            this.tbPrefixOutput.BackColor = System.Drawing.SystemColors.Window;
            this.tbPrefixOutput.ImageTransparentColor = System.Drawing.Color.Turquoise;
            this.tbPrefixOutput.Location = new System.Drawing.Point(339, 344);
            this.tbPrefixOutput.Name = "tbPrefixOutput";
            this.tbPrefixOutput.Size = new System.Drawing.Size(88, 21);
            this.tbPrefixOutput.TabIndex = 21;
            // 
            // btnAutoMap
            // 
            this.btnAutoMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAutoMap.Location = new System.Drawing.Point(5, 344);
            this.btnAutoMap.Name = "btnAutoMap";
            this.btnAutoMap.Size = new System.Drawing.Size(75, 21);
            this.btnAutoMap.TabIndex = 19;
            this.btnAutoMap.Text = "AutoMap";
            this.btnAutoMap.UseVisualStyleBackColor = true;
            this.btnAutoMap.Click += new System.EventHandler(this.btnAutoMap_Click);
            // 
            // tbPrefixInput
            // 
            this.tbPrefixInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.tbPrefixInput.Appearance = appearance8;
            this.tbPrefixInput.BackColor = System.Drawing.SystemColors.Window;
            this.tbPrefixInput.ImageTransparentColor = System.Drawing.Color.Turquoise;
            this.tbPrefixInput.Location = new System.Drawing.Point(161, 344);
            this.tbPrefixInput.Name = "tbPrefixInput";
            this.tbPrefixInput.Size = new System.Drawing.Size(88, 21);
            this.tbPrefixInput.TabIndex = 20;
            // 
            // pnlDGV
            // 
            this.pnlDGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDGV.Location = new System.Drawing.Point(5, 3);
            this.pnlDGV.Name = "pnlDGV";
            this.pnlDGV.Size = new System.Drawing.Size(898, 333);
            this.pnlDGV.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(95, 348);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Input Prefix";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(265, 348);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Output Prefix";
            // 
            // uTabPreSqlStatement
            // 
            this.uTabPreSqlStatement.Controls.Add(this.pnlExcludePreSqlFromTransaction);
            this.uTabPreSqlStatement.Controls.Add(this.lblExcludePreSqlFromTransaction);
            this.uTabPreSqlStatement.Controls.Add(this.btnInsertTruncatePreSql);
            this.uTabPreSqlStatement.Controls.Add(this.btnInsertVariablePreSql);
            this.uTabPreSqlStatement.Controls.Add(this.pnlVariablesPreSql);
            this.uTabPreSqlStatement.Controls.Add(this.lblVariablesPreSql);
            this.uTabPreSqlStatement.Controls.Add(this.tbPreSql);
            this.uTabPreSqlStatement.Controls.Add(this.ultraTextEditor1);
            this.uTabPreSqlStatement.Location = new System.Drawing.Point(-10000, -10000);
            this.uTabPreSqlStatement.Name = "uTabPreSqlStatement";
            this.uTabPreSqlStatement.Size = new System.Drawing.Size(913, 371);
            // 
            // pnlExcludePreSqlFromTransaction
            // 
            this.pnlExcludePreSqlFromTransaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlExcludePreSqlFromTransaction.BackColor = System.Drawing.Color.Transparent;
            this.pnlExcludePreSqlFromTransaction.Location = new System.Drawing.Point(889, 343);
            this.pnlExcludePreSqlFromTransaction.Name = "pnlExcludePreSqlFromTransaction";
            this.pnlExcludePreSqlFromTransaction.Size = new System.Drawing.Size(22, 21);
            this.pnlExcludePreSqlFromTransaction.TabIndex = 17;
            // 
            // lblExcludePreSqlFromTransaction
            // 
            this.lblExcludePreSqlFromTransaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExcludePreSqlFromTransaction.AutoSize = true;
            this.lblExcludePreSqlFromTransaction.BackColor = System.Drawing.Color.Transparent;
            this.lblExcludePreSqlFromTransaction.Location = new System.Drawing.Point(760, 346);
            this.lblExcludePreSqlFromTransaction.Name = "lblExcludePreSqlFromTransaction";
            this.lblExcludePreSqlFromTransaction.Size = new System.Drawing.Size(123, 13);
            this.lblExcludePreSqlFromTransaction.TabIndex = 16;
            this.lblExcludePreSqlFromTransaction.Text = "Exclude from transaction";
            // 
            // btnInsertTruncatePreSql
            // 
            this.btnInsertTruncatePreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertTruncatePreSql.Location = new System.Drawing.Point(413, 342);
            this.btnInsertTruncatePreSql.Name = "btnInsertTruncatePreSql";
            this.btnInsertTruncatePreSql.Size = new System.Drawing.Size(109, 22);
            this.btnInsertTruncatePreSql.TabIndex = 14;
            this.btnInsertTruncatePreSql.Text = "Insert Truncate";
            this.btnInsertTruncatePreSql.Click += new System.EventHandler(this.btnInsertTruncatePreSql_Click);
            // 
            // btnInsertVariablePreSql
            // 
            this.btnInsertVariablePreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertVariablePreSql.Location = new System.Drawing.Point(298, 342);
            this.btnInsertVariablePreSql.Name = "btnInsertVariablePreSql";
            this.btnInsertVariablePreSql.Size = new System.Drawing.Size(109, 22);
            this.btnInsertVariablePreSql.TabIndex = 14;
            this.btnInsertVariablePreSql.Text = "Insert Variable";
            this.btnInsertVariablePreSql.Click += new System.EventHandler(this.btnInsertVariablePreSql_Click);
            // 
            // pnlVariablesPreSql
            // 
            this.pnlVariablesPreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariablesPreSql.Location = new System.Drawing.Point(72, 342);
            this.pnlVariablesPreSql.Name = "pnlVariablesPreSql";
            this.pnlVariablesPreSql.Size = new System.Drawing.Size(200, 22);
            this.pnlVariablesPreSql.TabIndex = 13;
            // 
            // lblVariablesPreSql
            // 
            this.lblVariablesPreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance9.BackColor = System.Drawing.Color.Transparent;
            this.lblVariablesPreSql.Appearance = appearance9;
            this.lblVariablesPreSql.Location = new System.Drawing.Point(8, 346);
            this.lblVariablesPreSql.Name = "lblVariablesPreSql";
            this.lblVariablesPreSql.Size = new System.Drawing.Size(57, 15);
            this.lblVariablesPreSql.TabIndex = 12;
            this.lblVariablesPreSql.Text = "Variables";
            // 
            // tbPreSql
            // 
            this.tbPreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPreSql.Location = new System.Drawing.Point(0, 0);
            this.tbPreSql.Name = "tbPreSql";
            this.tbPreSql.Size = new System.Drawing.Size(913, 335);
            this.tbPreSql.TabIndex = 11;
            this.tbPreSql.Value = "";
            this.tbPreSql.TextChanged += new System.EventHandler(this.tbPreSql_TextChanged);
            // 
            // ultraTextEditor1
            // 
            this.ultraTextEditor1.Location = new System.Drawing.Point(662, 169);
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.Size = new System.Drawing.Size(100, 21);
            this.ultraTextEditor1.TabIndex = 0;
            this.ultraTextEditor1.Text = "ultraTextEditor1";
            // 
            // uTabPostSqlStatement
            // 
            this.uTabPostSqlStatement.Controls.Add(this.btnInsertVariablePostSql);
            this.uTabPostSqlStatement.Controls.Add(this.pnlVariablesPostSql);
            this.uTabPostSqlStatement.Controls.Add(this.lblVariablesPostSql);
            this.uTabPostSqlStatement.Controls.Add(this.tbPostSql);
            this.uTabPostSqlStatement.Location = new System.Drawing.Point(-10000, -10000);
            this.uTabPostSqlStatement.Name = "uTabPostSqlStatement";
            this.uTabPostSqlStatement.Size = new System.Drawing.Size(913, 371);
            // 
            // btnInsertVariablePostSql
            // 
            this.btnInsertVariablePostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertVariablePostSql.Location = new System.Drawing.Point(298, 342);
            this.btnInsertVariablePostSql.Name = "btnInsertVariablePostSql";
            this.btnInsertVariablePostSql.Size = new System.Drawing.Size(109, 22);
            this.btnInsertVariablePostSql.TabIndex = 18;
            this.btnInsertVariablePostSql.Text = "Insert Variable";
            this.btnInsertVariablePostSql.Click += new System.EventHandler(this.btnInsertVariablePostSql_Click);
            // 
            // pnlVariablesPostSql
            // 
            this.pnlVariablesPostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariablesPostSql.BackColor = System.Drawing.Color.Transparent;
            this.pnlVariablesPostSql.Location = new System.Drawing.Point(72, 342);
            this.pnlVariablesPostSql.Name = "pnlVariablesPostSql";
            this.pnlVariablesPostSql.Size = new System.Drawing.Size(200, 22);
            this.pnlVariablesPostSql.TabIndex = 16;
            // 
            // lblVariablesPostSql
            // 
            this.lblVariablesPostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVariablesPostSql.Location = new System.Drawing.Point(8, 346);
            this.lblVariablesPostSql.Name = "lblVariablesPostSql";
            this.lblVariablesPostSql.Size = new System.Drawing.Size(57, 15);
            this.lblVariablesPostSql.TabIndex = 15;
            this.lblVariablesPostSql.Text = "Variables";
            // 
            // tbPostSql
            // 
            this.tbPostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPostSql.Location = new System.Drawing.Point(0, 0);
            this.tbPostSql.Name = "tbPostSql";
            this.tbPostSql.Size = new System.Drawing.Size(913, 335);
            this.tbPostSql.TabIndex = 10;
            this.tbPostSql.Value = "";
            this.tbPostSql.TextChanged += new System.EventHandler(this.tbPostSql_TextChanged);
            // 
            // uTabSqlPreview
            // 
            this.uTabSqlPreview.Controls.Add(this.btnSqlPreview);
            this.uTabSqlPreview.Controls.Add(this.tbSqlPreview);
            this.uTabSqlPreview.Location = new System.Drawing.Point(-10000, -10000);
            this.uTabSqlPreview.Name = "uTabSqlPreview";
            this.uTabSqlPreview.Size = new System.Drawing.Size(913, 371);
            // 
            // btnSqlPreview
            // 
            this.btnSqlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSqlPreview.Location = new System.Drawing.Point(3, 344);
            this.btnSqlPreview.Name = "btnSqlPreview";
            this.btnSqlPreview.Size = new System.Drawing.Size(109, 22);
            this.btnSqlPreview.TabIndex = 19;
            this.btnSqlPreview.Text = "Refresh";
            this.btnSqlPreview.Click += new System.EventHandler(this.btnSqlPreview_Click);
            // 
            // tbSqlPreview
            // 
            this.tbSqlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance10.FontData.BoldAsString = "False";
            appearance10.FontData.ItalicAsString = "False";
            appearance10.FontData.Name = "Microsoft Sans Serif";
            appearance10.FontData.SizeInPoints = 8.25F;
            appearance10.FontData.StrikeoutAsString = "False";
            appearance10.FontData.UnderlineAsString = "False";
            this.tbSqlPreview.Appearance = appearance10;
            this.tbSqlPreview.Location = new System.Drawing.Point(-1, 3);
            this.tbSqlPreview.Name = "tbSqlPreview";
            this.tbSqlPreview.Size = new System.Drawing.Size(913, 335);
            this.tbSqlPreview.TabIndex = 11;
            this.tbSqlPreview.Value = "";
            // 
            // uTabCustomCommand
            // 
            this.uTabCustomCommand.Controls.Add(this.pnlUseCustomCommand);
            this.uTabCustomCommand.Controls.Add(this.btnInsertVarCustomCommand);
            this.uTabCustomCommand.Controls.Add(this.pnlVariablesCustomCommand);
            this.uTabCustomCommand.Controls.Add(this.ultraLabel1);
            this.uTabCustomCommand.Controls.Add(this.tbCustomMergeCommand);
            this.uTabCustomCommand.Controls.Add(this.btnInsertDefaultMergeCommand);
            this.uTabCustomCommand.Location = new System.Drawing.Point(-10000, -10000);
            this.uTabCustomCommand.Name = "uTabCustomCommand";
            this.uTabCustomCommand.Size = new System.Drawing.Size(913, 371);
            // 
            // pnlUseCustomCommand
            // 
            this.pnlUseCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlUseCustomCommand.BackColor = System.Drawing.Color.Transparent;
            this.pnlUseCustomCommand.Location = new System.Drawing.Point(5, 345);
            this.pnlUseCustomCommand.Name = "pnlUseCustomCommand";
            this.pnlUseCustomCommand.Size = new System.Drawing.Size(200, 22);
            this.pnlUseCustomCommand.TabIndex = 21;
            // 
            // btnInsertVarCustomCommand
            // 
            this.btnInsertVarCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertVarCustomCommand.Location = new System.Drawing.Point(498, 345);
            this.btnInsertVarCustomCommand.Name = "btnInsertVarCustomCommand";
            this.btnInsertVarCustomCommand.Size = new System.Drawing.Size(109, 22);
            this.btnInsertVarCustomCommand.TabIndex = 21;
            this.btnInsertVarCustomCommand.Text = "Insert Variable";
            this.btnInsertVarCustomCommand.Click += new System.EventHandler(this.btnInsertVarCustomCommand_Click);
            // 
            // pnlVariablesCustomCommand
            // 
            this.pnlVariablesCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariablesCustomCommand.BackColor = System.Drawing.Color.Transparent;
            this.pnlVariablesCustomCommand.Location = new System.Drawing.Point(292, 345);
            this.pnlVariablesCustomCommand.Name = "pnlVariablesCustomCommand";
            this.pnlVariablesCustomCommand.Size = new System.Drawing.Size(200, 22);
            this.pnlVariablesCustomCommand.TabIndex = 20;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel1.Location = new System.Drawing.Point(228, 349);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel1.TabIndex = 19;
            this.ultraLabel1.Text = "Variables";
            // 
            // tbCustomMergeCommand
            // 
            this.tbCustomMergeCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance11.FontData.BoldAsString = "False";
            appearance11.FontData.ItalicAsString = "False";
            appearance11.FontData.Name = "Microsoft Sans Serif";
            appearance11.FontData.SizeInPoints = 8.25F;
            appearance11.FontData.StrikeoutAsString = "False";
            appearance11.FontData.UnderlineAsString = "False";
            this.tbCustomMergeCommand.Appearance = appearance11;
            this.tbCustomMergeCommand.Location = new System.Drawing.Point(0, 0);
            this.tbCustomMergeCommand.Name = "tbCustomMergeCommand";
            this.tbCustomMergeCommand.Size = new System.Drawing.Size(913, 338);
            this.tbCustomMergeCommand.TabIndex = 6;
            this.tbCustomMergeCommand.Value = "";
            // 
            // btnInsertDefaultMergeCommand
            // 
            this.btnInsertDefaultMergeCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInsertDefaultMergeCommand.Location = new System.Drawing.Point(744, 344);
            this.btnInsertDefaultMergeCommand.Name = "btnInsertDefaultMergeCommand";
            this.btnInsertDefaultMergeCommand.Size = new System.Drawing.Size(161, 24);
            this.btnInsertDefaultMergeCommand.TabIndex = 5;
            this.btnInsertDefaultMergeCommand.Text = "DB Command generieren";
            this.btnInsertDefaultMergeCommand.Click += new System.EventHandler(this.btnInsertDefaultCustomCommand_Click);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.pnlVariableChooserLog);
            this.ultraTabPageControl1.Controls.Add(this.ultraLabel2);
            this.ultraTabPageControl1.Controls.Add(this.numLogLevel);
            this.ultraTabPageControl1.Controls.Add(this.btnInsert);
            this.ultraTabPageControl1.Controls.Add(this.tbMessage);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(913, 371);
            // 
            // pnlVariableChooserLog
            // 
            this.pnlVariableChooserLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariableChooserLog.BackColor = System.Drawing.Color.Transparent;
            this.pnlVariableChooserLog.Location = new System.Drawing.Point(7, 346);
            this.pnlVariableChooserLog.Name = "pnlVariableChooserLog";
            this.pnlVariableChooserLog.Size = new System.Drawing.Size(200, 22);
            this.pnlVariableChooserLog.TabIndex = 28;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel2.Location = new System.Drawing.Point(803, 352);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(57, 15);
            this.ultraLabel2.TabIndex = 27;
            this.ultraLabel2.Text = "Log Level";
            // 
            // numLogLevel
            // 
            this.numLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance12.BackColor = System.Drawing.Color.Transparent;
            this.numLogLevel.Appearance = appearance12;
            this.numLogLevel.BackColor = System.Drawing.Color.Transparent;
            this.numLogLevel.Location = new System.Drawing.Point(866, 347);
            this.numLogLevel.MaskInput = "n";
            this.numLogLevel.MaxValue = 3;
            this.numLogLevel.MinValue = 1;
            this.numLogLevel.Name = "numLogLevel";
            this.numLogLevel.Size = new System.Drawing.Size(44, 21);
            this.numLogLevel.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.numLogLevel.SpinIncrement = 1;
            this.numLogLevel.TabIndex = 26;
            // 
            // btnInsert
            // 
            this.btnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsert.Location = new System.Drawing.Point(209, 347);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(75, 21);
            this.btnInsert.TabIndex = 25;
            this.btnInsert.Text = "Insert";
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // tbMessage
            // 
            this.tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.FontData.BoldAsString = "False";
            appearance13.FontData.ItalicAsString = "False";
            appearance13.FontData.Name = "Microsoft Sans Serif";
            appearance13.FontData.SizeInPoints = 8.25F;
            appearance13.FontData.StrikeoutAsString = "False";
            appearance13.FontData.UnderlineAsString = "False";
            this.tbMessage.Appearance = appearance13;
            this.tbMessage.HideSelection = false;
            this.tbMessage.Location = new System.Drawing.Point(7, 3);
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(903, 338);
            this.tbMessage.TabIndex = 22;
            this.tbMessage.Value = "";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(840, 418);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(753, 418);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 26);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ultraMessageBox
            // 
            this.ultraMessageBox.ContainingControl = this;
            this.ultraMessageBox.MinimumWidth = 775;
            this.ultraMessageBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // uTabConfig
            // 
            this.uTabConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uTabConfig.Controls.Add(this.ultraTabSharedControlsPage1);
            this.uTabConfig.Controls.Add(this.uTabConfiguration);
            this.uTabConfig.Controls.Add(this.uTabMapping);
            this.uTabConfig.Controls.Add(this.uTabPreSqlStatement);
            this.uTabConfig.Controls.Add(this.uTabPostSqlStatement);
            this.uTabConfig.Controls.Add(this.uTabSqlPreview);
            this.uTabConfig.Controls.Add(this.uTabCustomCommand);
            this.uTabConfig.Controls.Add(this.ultraTabPageControl1);
            this.uTabConfig.Location = new System.Drawing.Point(4, 12);
            this.uTabConfig.Name = "uTabConfig";
            this.uTabConfig.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.uTabConfig.Size = new System.Drawing.Size(917, 397);
            this.uTabConfig.TabIndex = 3;
            ultraTab1.TabPage = this.uTabConfiguration;
            ultraTab1.Text = "Configuration";
            ultraTab2.TabPage = this.uTabMapping;
            ultraTab2.Text = "Mapping";
            ultraTab3.TabPage = this.uTabPreSqlStatement;
            ultraTab3.Text = "Pre SQL Statement";
            ultraTab4.TabPage = this.uTabPostSqlStatement;
            ultraTab4.Text = "Post SQL Statement";
            ultraTab5.TabPage = this.uTabSqlPreview;
            ultraTab5.Text = "SQL Preview";
            ultraTab6.TabPage = this.uTabCustomCommand;
            ultraTab6.Text = "Custom Command";
            ultraTab7.TabPage = this.ultraTabPageControl1;
            ultraTab7.Text = "Logging";
            this.uTabConfig.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab5,
            ultraTab6,
            ultraTab7});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(913, 371);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // uceLayoutMapping
            // 
            this.uceLayoutMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.uceLayoutMapping.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "Standard";
            valueListItem1.DisplayText = "Standard";
            valueListItem2.DataValue = "Minimal";
            valueListItem2.DisplayText = "Minimal";
            valueListItem3.DataValue = "SCD";
            valueListItem3.DisplayText = "SCD";
            this.uceLayoutMapping.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.uceLayoutMapping.LimitToList = true;
            this.uceLayoutMapping.Location = new System.Drawing.Point(50, 423);
            this.uceLayoutMapping.Name = "uceLayoutMapping";
            this.uceLayoutMapping.Nullable = false;
            this.uceLayoutMapping.Size = new System.Drawing.Size(144, 21);
            this.uceLayoutMapping.TabIndex = 4;
            this.uceLayoutMapping.Text = "Standard";
            this.uceLayoutMapping.SelectionChanged += new System.EventHandler(this.uceLayoutMapping_SelectionChanged);
            // 
            // lblLayoutMapping
            // 
            this.lblLayoutMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLayoutMapping.AutoSize = true;
            this.lblLayoutMapping.Location = new System.Drawing.Point(5, 427);
            this.lblLayoutMapping.Name = "lblLayoutMapping";
            this.lblLayoutMapping.Size = new System.Drawing.Size(39, 13);
            this.lblLayoutMapping.TabIndex = 5;
            this.lblLayoutMapping.Text = "Layout";
            // 
            // frmTableLoaderUI
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(927, 453);
            this.Controls.Add(this.lblLayoutMapping);
            this.Controls.Add(this.uceLayoutMapping);
            this.Controls.Add(this.uTabConfig);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(16, 350);
            this.Name = "frmTableLoaderUI";
            this.Text = "Table Loader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTableLoaderUI_FormClosing);
            this.Load += new System.EventHandler(this.frmTableLoaderUI_Load);
            this.uTabConfiguration.ResumeLayout(false);
            this.uTabConfiguration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbReattempts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxThreadCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbChunkSizeDbCommand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbChunkSizeBulk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpStandardConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpChunkSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpTransactions)).EndInit();
            this.uTabMapping.ResumeLayout(false);
            this.uTabMapping.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPrefixOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPrefixInput)).EndInit();
            this.uTabPreSqlStatement.ResumeLayout(false);
            this.uTabPreSqlStatement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            this.uTabPostSqlStatement.ResumeLayout(false);
            this.uTabSqlPreview.ResumeLayout(false);
            this.uTabCustomCommand.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLogLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uTabConfig)).EndInit();
            this.uTabConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uceLayoutMapping)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox imgHelpTransactions;
        private Infragistics.Win.UltraMessageBox.UltraMessageBoxManager ultraMessageBox;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl uTabConfig;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl uTabConfiguration;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl uTabMapping;
        private Infragistics.Win.Misc.UltraButton btnSelect;
        private Infragistics.Win.Misc.UltraButton btnDeSelect;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbPrefixOutput;
        private System.Windows.Forms.Button btnAutoMap;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbPrefixInput;
        private System.Windows.Forms.Panel pnlDGV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl uTabPreSqlStatement;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl uTabPostSqlStatement;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl uTabSqlPreview;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl uTabCustomCommand;
        private Infragistics.Win.Misc.UltraButton btnInsertDefaultMergeCommand;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel pnlConnMgrBulk;
        private System.Windows.Forms.Panel pnlConnMgrMain;
        private System.Windows.Forms.Label lblTransaction;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblChunkSizeBulk;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbTimeout;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbChunkSizeBulk;
        private System.Windows.Forms.Panel pnlDbCommand;
        private System.Windows.Forms.Panel pnlDestTable;
        private System.Windows.Forms.Label lbDbCommand;
        private System.Windows.Forms.Panel pnlTransaction;
        private Infragistics.Win.Misc.UltraLabel lblConMgrBulk;
        private Infragistics.Win.Misc.UltraButton btnCreateTable;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor tbPreSql;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor tbPostSql;
        private Infragistics.Win.Misc.UltraButton btnInsertTruncatePreSql;
        private Infragistics.Win.Misc.UltraButton btnInsertVariablePreSql;
        private System.Windows.Forms.Panel pnlVariablesPreSql;
        private Infragistics.Win.Misc.UltraLabel lblVariablesPreSql;
        private Infragistics.Win.Misc.UltraButton btnInsertVariablePostSql;
        private System.Windows.Forms.Panel pnlVariablesPostSql;
        private Infragistics.Win.Misc.UltraLabel lblVariablesPostSql;
        private Infragistics.Win.Misc.UltraButton btnAddRow;
        private Infragistics.Win.Misc.UltraButton btnRemoveRow;
        private Infragistics.Win.Misc.UltraButton btnSqlPreview;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor tbSqlPreview;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor tbCustomMergeCommand;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.PictureBox imgHelpChunkSize;
        private Infragistics.Win.Misc.UltraButton btnAlterTable;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbMaxThreadCount;
        private System.Windows.Forms.Label lblMaxThreadCount;
        private Infragistics.Win.Misc.UltraButton btnInsertVarCustomCommand;
        private System.Windows.Forms.Panel pnlVariablesCustomCommand;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.Panel pnlUseCustomCommand;
        private System.Windows.Forms.Panel pnlCheckStandardConfigAuto;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlCmbStandardConfig;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.Misc.UltraButton btnInsert;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedTextEditor tbMessage;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor numLogLevel;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private System.Windows.Forms.PictureBox imgHelpStandardConfig;
        private System.Windows.Forms.Panel pnlTableLoaderType;
        private System.Windows.Forms.Label lblTlType;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbChunkSizeDbCommand;
        private System.Windows.Forms.Label lblChunkSizeDbCommand;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor tbReattempts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlVariableChooserLog;
        private System.Windows.Forms.Panel pnlCheckDisableTablock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlExcludePreSqlFromTransaction;
        private System.Windows.Forms.Label lblExcludePreSqlFromTransaction;
        private Infragistics.Win.Misc.UltraButton btnCreateScdTable;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor uceLayoutMapping;
        private System.Windows.Forms.Label lblLayoutMapping;
    }
}