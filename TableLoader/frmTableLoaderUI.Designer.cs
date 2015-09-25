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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableLoaderUI));
            this.btnCreateTable = new System.Windows.Forms.Button();
            this.btnAlterTable = new System.Windows.Forms.Button();
            this.btnCreateScdTable = new System.Windows.Forms.Button();
            this.tbReattempts = new System.Windows.Forms.TextBox();
            this.tbMaxThreadCount = new System.Windows.Forms.TextBox();
            this.tbChunkSizeBulk = new System.Windows.Forms.TextBox();
            this.tbChunkSizeDbCommand = new System.Windows.Forms.TextBox();
            this.tbTimeout = new System.Windows.Forms.TextBox();
            this._checkStandardConfigAuto = new System.Windows.Forms.CheckBox();
            this.checkDisableTablock = new System.Windows.Forms.CheckBox();
            this.lblConMgrBulk = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlTableLoaderType = new System.Windows.Forms.Panel();
            this.pnlTransaction = new System.Windows.Forms.Panel();
            this.pnlCmbStandardConfig = new System.Windows.Forms.Panel();
            this.pnlDestinationTanble = new System.Windows.Forms.Panel();
            this.pnlDbCommand = new System.Windows.Forms.Panel();
            this.lblTlType = new System.Windows.Forms.Label();
            this.lblTransaction = new System.Windows.Forms.Label();
            this.imgHelpStandardConfig = new System.Windows.Forms.PictureBox();
            this.imgHelpChunkSize = new System.Windows.Forms.PictureBox();
            this.imgHelpTransactions = new System.Windows.Forms.PictureBox();
            this.pnlConnMgrBulk = new System.Windows.Forms.Panel();
            this.pnlConnMgrMain = new System.Windows.Forms.Panel();
            this.lbDbCommand = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblMaxThreadCount = new System.Windows.Forms.Label();
            this.lblChunkSizeDbCommand = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblChunkSizeBulk = new System.Windows.Forms.Label();
            this.tbPrefixOutput = new System.Windows.Forms.TextBox();
            this.tbPrefixInput = new System.Windows.Forms.TextBox();
            this.btnAutoMap = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnDeSelect = new System.Windows.Forms.Button();
            this.btnRemoveRow = new System.Windows.Forms.Button();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.pnlDGV = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPreSql = new System.Windows.Forms.TextBox();
            this._checkExcludePreSqlFromTransaction = new System.Windows.Forms.CheckBox();
            this.btnInsertTruncatePreSql = new System.Windows.Forms.Button();
            this.btnInsertVariablePreSql = new System.Windows.Forms.Button();
            this.lblVariablesPreSql = new System.Windows.Forms.Label();
            this.lblExcludePreSqlFromTransaction = new System.Windows.Forms.Label();
            this.pnlVariablesPreSql = new System.Windows.Forms.Panel();
            this.tbPostSql = new System.Windows.Forms.TextBox();
            this.lblVariablesPostSql = new System.Windows.Forms.Label();
            this.btnInsertVariablePostSql = new System.Windows.Forms.Button();
            this.pnlVariablesPostSql = new System.Windows.Forms.Panel();
            this.tbSqlPreview = new System.Windows.Forms.TextBox();
            this.btnSqlPreview = new System.Windows.Forms.Button();
            this._checkUseCustomCommand = new System.Windows.Forms.CheckBox();
            this.btnInsertDefaultMergeCommand = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbCustomMergeCommand = new System.Windows.Forms.TextBox();
            this.btnInsertVarCustomCommand = new System.Windows.Forms.Button();
            this.pnlVariablesCustomCommand = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.numLogLevel1 = new System.Windows.Forms.DomainUpDown();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.btnInsert = new System.Windows.Forms.Button();
            this.pnlVariableChooserLog = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblLayoutMapping = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbLayoutMapping = new System.Windows.Forms.ComboBox();
            this.uTabConfig = new System.Windows.Forms.TabControl();
            this.uTabConfiguration = new System.Windows.Forms.TabPage();
            this.uTabMapping = new System.Windows.Forms.TabPage();
            this.uTabPreSQLStatement = new System.Windows.Forms.TabPage();
            this.uTabPostSQLStatement = new System.Windows.Forms.TabPage();
            this.uTabSQLPreview = new System.Windows.Forms.TabPage();
            this.uTabCustomCommand = new System.Windows.Forms.TabPage();
            this.uTabLogging = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpStandardConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpChunkSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpTransactions)).BeginInit();
            this.uTabConfig.SuspendLayout();
            this.uTabConfiguration.SuspendLayout();
            this.uTabMapping.SuspendLayout();
            this.uTabPreSQLStatement.SuspendLayout();
            this.uTabPostSQLStatement.SuspendLayout();
            this.uTabSQLPreview.SuspendLayout();
            this.uTabCustomCommand.SuspendLayout();
            this.uTabLogging.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCreateTable
            // 
            this.btnCreateTable.Location = new System.Drawing.Point(587, 78);
            this.btnCreateTable.Name = "btnCreateTable";
            this.btnCreateTable.Size = new System.Drawing.Size(95, 21);
            this.btnCreateTable.TabIndex = 10;
            this.btnCreateTable.Text = "Create Table";
            this.btnCreateTable.UseVisualStyleBackColor = true;
            this.btnCreateTable.Click += new System.EventHandler(this.btnCreateTable_Click);
            // 
            // btnAlterTable
            // 
            this.btnAlterTable.Location = new System.Drawing.Point(688, 78);
            this.btnAlterTable.Name = "btnAlterTable";
            this.btnAlterTable.Size = new System.Drawing.Size(95, 21);
            this.btnAlterTable.TabIndex = 11;
            this.btnAlterTable.Text = "Alter Table";
            this.btnAlterTable.UseVisualStyleBackColor = true;
            this.btnAlterTable.Click += new System.EventHandler(this.btnAlterTable_Click);
            // 
            // btnCreateScdTable
            // 
            this.btnCreateScdTable.Location = new System.Drawing.Point(587, 105);
            this.btnCreateScdTable.Name = "btnCreateScdTable";
            this.btnCreateScdTable.Size = new System.Drawing.Size(95, 21);
            this.btnCreateScdTable.TabIndex = 12;
            this.btnCreateScdTable.Text = "Create SCD";
            this.btnCreateScdTable.UseVisualStyleBackColor = true;
            this.btnCreateScdTable.Click += new System.EventHandler(this.btnCreateScdTable_Click);
            // 
            // tbReattempts
            // 
            this.tbReattempts.Location = new System.Drawing.Point(653, 208);
            this.tbReattempts.Name = "tbReattempts";
            this.tbReattempts.Size = new System.Drawing.Size(147, 20);
            this.tbReattempts.TabIndex = 22;
            // 
            // tbMaxThreadCount
            // 
            this.tbMaxThreadCount.Location = new System.Drawing.Point(171, 112);
            this.tbMaxThreadCount.Name = "tbMaxThreadCount";
            this.tbMaxThreadCount.Size = new System.Drawing.Size(363, 20);
            this.tbMaxThreadCount.TabIndex = 14;
            this.toolTip1.SetToolTip(this.tbMaxThreadCount, "Maximum number of BulkCopy threads");
            // 
            // tbChunkSizeBulk
            // 
            this.tbChunkSizeBulk.Location = new System.Drawing.Point(171, 138);
            this.tbChunkSizeBulk.Name = "tbChunkSizeBulk";
            this.tbChunkSizeBulk.Size = new System.Drawing.Size(363, 20);
            this.tbChunkSizeBulk.TabIndex = 16;
            this.toolTip1.SetToolTip(this.tbChunkSizeBulk, "Number of rows  used for a single Bulk Copy thread");
            // 
            // tbChunkSizeDbCommand
            // 
            this.tbChunkSizeDbCommand.Location = new System.Drawing.Point(171, 163);
            this.tbChunkSizeDbCommand.Name = "tbChunkSizeDbCommand";
            this.tbChunkSizeDbCommand.Size = new System.Drawing.Size(363, 20);
            this.tbChunkSizeDbCommand.TabIndex = 18;
            // 
            // tbTimeout
            // 
            this.tbTimeout.Location = new System.Drawing.Point(171, 208);
            this.tbTimeout.Name = "tbTimeout";
            this.tbTimeout.Size = new System.Drawing.Size(363, 20);
            this.tbTimeout.TabIndex = 20;
            this.toolTip1.SetToolTip(this.tbTimeout, "Database Timeout in seconds");
            // 
            // _checkStandardConfigAuto
            // 
            this._checkStandardConfigAuto.AutoSize = true;
            this._checkStandardConfigAuto.Enabled = false;
            this._checkStandardConfigAuto.Location = new System.Drawing.Point(145, 281);
            this._checkStandardConfigAuto.Name = "_checkStandardConfigAuto";
            this._checkStandardConfigAuto.Size = new System.Drawing.Size(15, 14);
            this._checkStandardConfigAuto.TabIndex = 26;
            this._checkStandardConfigAuto.UseVisualStyleBackColor = true;
            this._checkStandardConfigAuto.CheckedChanged += new System.EventHandler(this._checkStandardConfigAuto_CheckedChanged);
            // 
            // checkDisableTablock
            // 
            this.checkDisableTablock.AutoSize = true;
            this.checkDisableTablock.Location = new System.Drawing.Point(145, 318);
            this.checkDisableTablock.Name = "checkDisableTablock";
            this.checkDisableTablock.Size = new System.Drawing.Size(15, 14);
            this.checkDisableTablock.TabIndex = 29;
            this.checkDisableTablock.UseVisualStyleBackColor = true;
            // 
            // lblConMgrBulk
            // 
            this.lblConMgrBulk.AutoSize = true;
            this.lblConMgrBulk.BackColor = System.Drawing.Color.Transparent;
            this.lblConMgrBulk.Location = new System.Drawing.Point(18, 51);
            this.lblConMgrBulk.Name = "lblConMgrBulk";
            this.lblConMgrBulk.Size = new System.Drawing.Size(139, 13);
            this.lblConMgrBulk.TabIndex = 4;
            this.lblConMgrBulk.Text = "Connection Manager (Bulk):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(19, 281);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Standard Configuration";
            // 
            // pnlTableLoaderType
            // 
            this.pnlTableLoaderType.BackColor = System.Drawing.Color.Transparent;
            this.pnlTableLoaderType.Location = new System.Drawing.Point(653, 21);
            this.pnlTableLoaderType.Name = "pnlTableLoaderType";
            this.pnlTableLoaderType.Size = new System.Drawing.Size(147, 21);
            this.pnlTableLoaderType.TabIndex = 3;
            // 
            // pnlTransaction
            // 
            this.pnlTransaction.BackColor = System.Drawing.Color.Transparent;
            this.pnlTransaction.Location = new System.Drawing.Point(653, 48);
            this.pnlTransaction.Name = "pnlTransaction";
            this.pnlTransaction.Size = new System.Drawing.Size(147, 21);
            this.pnlTransaction.TabIndex = 7;
            // 
            // pnlCmbStandardConfig
            // 
            this.pnlCmbStandardConfig.BackColor = System.Drawing.Color.Transparent;
            this.pnlCmbStandardConfig.Location = new System.Drawing.Point(171, 278);
            this.pnlCmbStandardConfig.Name = "pnlCmbStandardConfig";
            this.pnlCmbStandardConfig.Size = new System.Drawing.Size(363, 21);
            this.pnlCmbStandardConfig.TabIndex = 27;
            // 
            // pnlDestinationTanble
            // 
            this.pnlDestinationTanble.BackColor = System.Drawing.Color.Transparent;
            this.pnlDestinationTanble.Location = new System.Drawing.Point(171, 78);
            this.pnlDestinationTanble.Name = "pnlDestinationTanble";
            this.pnlDestinationTanble.Size = new System.Drawing.Size(363, 21);
            this.pnlDestinationTanble.TabIndex = 9;
            // 
            // pnlDbCommand
            // 
            this.pnlDbCommand.BackColor = System.Drawing.Color.Transparent;
            this.pnlDbCommand.Location = new System.Drawing.Point(171, 235);
            this.pnlDbCommand.Name = "pnlDbCommand";
            this.pnlDbCommand.Size = new System.Drawing.Size(363, 21);
            this.pnlDbCommand.TabIndex = 24;
            // 
            // lblTlType
            // 
            this.lblTlType.AutoSize = true;
            this.lblTlType.BackColor = System.Drawing.Color.Transparent;
            this.lblTlType.Location = new System.Drawing.Point(587, 24);
            this.lblTlType.Name = "lblTlType";
            this.lblTlType.Size = new System.Drawing.Size(50, 13);
            this.lblTlType.TabIndex = 2;
            this.lblTlType.Text = "TL Type:";
            // 
            // lblTransaction
            // 
            this.lblTransaction.AutoSize = true;
            this.lblTransaction.BackColor = System.Drawing.Color.Transparent;
            this.lblTransaction.Location = new System.Drawing.Point(587, 51);
            this.lblTransaction.Name = "lblTransaction";
            this.lblTransaction.Size = new System.Drawing.Size(66, 13);
            this.lblTransaction.TabIndex = 6;
            this.lblTransaction.Text = "Transaction:";
            // 
            // imgHelpStandardConfig
            // 
            this.imgHelpStandardConfig.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imgHelpStandardConfig.BackgroundImage")));
            this.imgHelpStandardConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgHelpStandardConfig.ErrorImage = null;
            this.imgHelpStandardConfig.Location = new System.Drawing.Point(540, 279);
            this.imgHelpStandardConfig.Name = "imgHelpStandardConfig";
            this.imgHelpStandardConfig.Size = new System.Drawing.Size(23, 20);
            this.imgHelpStandardConfig.TabIndex = 10;
            this.imgHelpStandardConfig.TabStop = false;
            this.imgHelpStandardConfig.Click += new System.EventHandler(this.imgHelpStandardConfig_Click);
            // 
            // imgHelpChunkSize
            // 
            this.imgHelpChunkSize.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imgHelpChunkSize.BackgroundImage")));
            this.imgHelpChunkSize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgHelpChunkSize.ErrorImage = null;
            this.imgHelpChunkSize.Location = new System.Drawing.Point(540, 138);
            this.imgHelpChunkSize.Name = "imgHelpChunkSize";
            this.imgHelpChunkSize.Size = new System.Drawing.Size(23, 20);
            this.imgHelpChunkSize.TabIndex = 10;
            this.imgHelpChunkSize.TabStop = false;
            this.imgHelpChunkSize.Click += new System.EventHandler(this.imgHelpChunkSize_Click);
            // 
            // imgHelpTransactions
            // 
            this.imgHelpTransactions.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imgHelpTransactions.BackgroundImage")));
            this.imgHelpTransactions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgHelpTransactions.ErrorImage = null;
            this.imgHelpTransactions.Location = new System.Drawing.Point(812, 48);
            this.imgHelpTransactions.Name = "imgHelpTransactions";
            this.imgHelpTransactions.Size = new System.Drawing.Size(23, 20);
            this.imgHelpTransactions.TabIndex = 10;
            this.imgHelpTransactions.TabStop = false;
            this.imgHelpTransactions.Click += new System.EventHandler(this.imgHelpTransactions_Click);
            // 
            // pnlConnMgrBulk
            // 
            this.pnlConnMgrBulk.BackColor = System.Drawing.Color.Transparent;
            this.pnlConnMgrBulk.Location = new System.Drawing.Point(171, 42);
            this.pnlConnMgrBulk.Name = "pnlConnMgrBulk";
            this.pnlConnMgrBulk.Size = new System.Drawing.Size(363, 27);
            this.pnlConnMgrBulk.TabIndex = 5;
            // 
            // pnlConnMgrMain
            // 
            this.pnlConnMgrMain.BackColor = System.Drawing.Color.Transparent;
            this.pnlConnMgrMain.Location = new System.Drawing.Point(171, 14);
            this.pnlConnMgrMain.Name = "pnlConnMgrMain";
            this.pnlConnMgrMain.Size = new System.Drawing.Size(363, 27);
            this.pnlConnMgrMain.TabIndex = 1;
            // 
            // lbDbCommand
            // 
            this.lbDbCommand.AutoSize = true;
            this.lbDbCommand.BackColor = System.Drawing.Color.Transparent;
            this.lbDbCommand.Location = new System.Drawing.Point(18, 239);
            this.lbDbCommand.Name = "lbDbCommand";
            this.lbDbCommand.Size = new System.Drawing.Size(72, 13);
            this.lbDbCommand.TabIndex = 23;
            this.lbDbCommand.Text = "DB Command";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(19, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(141, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Connection Manager (Main):";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(19, 82);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "Destination Table";
            // 
            // lblMaxThreadCount
            // 
            this.lblMaxThreadCount.AutoSize = true;
            this.lblMaxThreadCount.BackColor = System.Drawing.Color.Transparent;
            this.lblMaxThreadCount.Location = new System.Drawing.Point(18, 115);
            this.lblMaxThreadCount.Name = "lblMaxThreadCount";
            this.lblMaxThreadCount.Size = new System.Drawing.Size(95, 13);
            this.lblMaxThreadCount.TabIndex = 13;
            this.lblMaxThreadCount.Text = "Max Thread Count";
            // 
            // lblChunkSizeDbCommand
            // 
            this.lblChunkSizeDbCommand.AutoSize = true;
            this.lblChunkSizeDbCommand.BackColor = System.Drawing.Color.Transparent;
            this.lblChunkSizeDbCommand.Location = new System.Drawing.Point(18, 166);
            this.lblChunkSizeDbCommand.Name = "lblChunkSizeDbCommand";
            this.lblChunkSizeDbCommand.Size = new System.Drawing.Size(135, 13);
            this.lblChunkSizeDbCommand.TabIndex = 17;
            this.lblChunkSizeDbCommand.Text = "Chunk Size (DB Command)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(18, 318);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Disable Tablock";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(587, 212);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Reattempts";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.Location = new System.Drawing.Point(18, 212);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(77, 13);
            this.label17.TabIndex = 19;
            this.label17.Text = "DB Timeout [s]";
            // 
            // lblChunkSizeBulk
            // 
            this.lblChunkSizeBulk.AutoSize = true;
            this.lblChunkSizeBulk.BackColor = System.Drawing.Color.Transparent;
            this.lblChunkSizeBulk.Location = new System.Drawing.Point(18, 141);
            this.lblChunkSizeBulk.Name = "lblChunkSizeBulk";
            this.lblChunkSizeBulk.Size = new System.Drawing.Size(91, 13);
            this.lblChunkSizeBulk.TabIndex = 15;
            this.lblChunkSizeBulk.Text = "Chunk Size (Bulk)";
            // 
            // tbPrefixOutput
            // 
            this.tbPrefixOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbPrefixOutput.Location = new System.Drawing.Point(340, 341);
            this.tbPrefixOutput.Name = "tbPrefixOutput";
            this.tbPrefixOutput.Size = new System.Drawing.Size(88, 20);
            this.tbPrefixOutput.TabIndex = 5;
            // 
            // tbPrefixInput
            // 
            this.tbPrefixInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbPrefixInput.Location = new System.Drawing.Point(162, 341);
            this.tbPrefixInput.Name = "tbPrefixInput";
            this.tbPrefixInput.Size = new System.Drawing.Size(88, 20);
            this.tbPrefixInput.TabIndex = 3;
            // 
            // btnAutoMap
            // 
            this.btnAutoMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAutoMap.Location = new System.Drawing.Point(6, 341);
            this.btnAutoMap.Name = "btnAutoMap";
            this.btnAutoMap.Size = new System.Drawing.Size(75, 21);
            this.btnAutoMap.TabIndex = 1;
            this.btnAutoMap.Text = "AutoMap";
            this.btnAutoMap.UseVisualStyleBackColor = true;
            this.btnAutoMap.Click += new System.EventHandler(this.btnAutoMap_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelect.Location = new System.Drawing.Point(492, 340);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(74, 23);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDeSelect
            // 
            this.btnDeSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeSelect.Location = new System.Drawing.Point(572, 340);
            this.btnDeSelect.Name = "btnDeSelect";
            this.btnDeSelect.Size = new System.Drawing.Size(74, 23);
            this.btnDeSelect.TabIndex = 7;
            this.btnDeSelect.Text = "Deselect";
            this.btnDeSelect.UseVisualStyleBackColor = true;
            this.btnDeSelect.Click += new System.EventHandler(this.btnDeSelect_Click);
            // 
            // btnRemoveRow
            // 
            this.btnRemoveRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveRow.Location = new System.Drawing.Point(789, 341);
            this.btnRemoveRow.Name = "btnRemoveRow";
            this.btnRemoveRow.Size = new System.Drawing.Size(97, 21);
            this.btnRemoveRow.TabIndex = 9;
            this.btnRemoveRow.Text = "Remove Row(s)";
            this.btnRemoveRow.UseVisualStyleBackColor = true;
            this.btnRemoveRow.Click += new System.EventHandler(this.btnRemoveRow_Click);
            // 
            // btnAddRow
            // 
            this.btnAddRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddRow.Location = new System.Drawing.Point(686, 341);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(97, 21);
            this.btnAddRow.TabIndex = 8;
            this.btnAddRow.Text = "Add Row";
            this.btnAddRow.UseVisualStyleBackColor = true;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // pnlDGV
            // 
            this.pnlDGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDGV.Location = new System.Drawing.Point(6, 0);
            this.pnlDGV.Name = "pnlDGV";
            this.pnlDGV.Size = new System.Drawing.Size(898, 333);
            this.pnlDGV.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(96, 345);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Input Prefix";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(266, 345);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Output Prefix";
            // 
            // tbPreSql
            // 
            this.tbPreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPreSql.Location = new System.Drawing.Point(-4, 0);
            this.tbPreSql.Multiline = true;
            this.tbPreSql.Name = "tbPreSql";
            this.tbPreSql.Size = new System.Drawing.Size(913, 336);
            this.tbPreSql.TabIndex = 20;
            // 
            // _checkExcludePreSqlFromTransaction
            // 
            this._checkExcludePreSqlFromTransaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._checkExcludePreSqlFromTransaction.AutoSize = true;
            this._checkExcludePreSqlFromTransaction.Location = new System.Drawing.Point(885, 347);
            this._checkExcludePreSqlFromTransaction.Name = "_checkExcludePreSqlFromTransaction";
            this._checkExcludePreSqlFromTransaction.Size = new System.Drawing.Size(15, 14);
            this._checkExcludePreSqlFromTransaction.TabIndex = 19;
            this._checkExcludePreSqlFromTransaction.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._checkExcludePreSqlFromTransaction.UseVisualStyleBackColor = true;
            // 
            // btnInsertTruncatePreSql
            // 
            this.btnInsertTruncatePreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertTruncatePreSql.Location = new System.Drawing.Point(409, 342);
            this.btnInsertTruncatePreSql.Name = "btnInsertTruncatePreSql";
            this.btnInsertTruncatePreSql.Size = new System.Drawing.Size(109, 22);
            this.btnInsertTruncatePreSql.TabIndex = 18;
            this.btnInsertTruncatePreSql.Text = "Insert Truncate";
            this.btnInsertTruncatePreSql.UseVisualStyleBackColor = true;
            this.btnInsertTruncatePreSql.Click += new System.EventHandler(this.btnInsertTruncatePreSql_Click);
            // 
            // btnInsertVariablePreSql
            // 
            this.btnInsertVariablePreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertVariablePreSql.Location = new System.Drawing.Point(294, 342);
            this.btnInsertVariablePreSql.Name = "btnInsertVariablePreSql";
            this.btnInsertVariablePreSql.Size = new System.Drawing.Size(109, 22);
            this.btnInsertVariablePreSql.TabIndex = 18;
            this.btnInsertVariablePreSql.Text = "Insert Variable";
            this.btnInsertVariablePreSql.UseVisualStyleBackColor = true;
            this.btnInsertVariablePreSql.Click += new System.EventHandler(this.btnInsertVariablePreSql_Click);
            // 
            // lblVariablesPreSql
            // 
            this.lblVariablesPreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVariablesPreSql.AutoSize = true;
            this.lblVariablesPreSql.BackColor = System.Drawing.Color.Transparent;
            this.lblVariablesPreSql.Location = new System.Drawing.Point(3, 346);
            this.lblVariablesPreSql.Name = "lblVariablesPreSql";
            this.lblVariablesPreSql.Size = new System.Drawing.Size(50, 13);
            this.lblVariablesPreSql.TabIndex = 16;
            this.lblVariablesPreSql.Text = "Variables";
            // 
            // lblExcludePreSqlFromTransaction
            // 
            this.lblExcludePreSqlFromTransaction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExcludePreSqlFromTransaction.AutoSize = true;
            this.lblExcludePreSqlFromTransaction.BackColor = System.Drawing.Color.Transparent;
            this.lblExcludePreSqlFromTransaction.Location = new System.Drawing.Point(756, 346);
            this.lblExcludePreSqlFromTransaction.Name = "lblExcludePreSqlFromTransaction";
            this.lblExcludePreSqlFromTransaction.Size = new System.Drawing.Size(123, 13);
            this.lblExcludePreSqlFromTransaction.TabIndex = 16;
            this.lblExcludePreSqlFromTransaction.Text = "Exclude from transaction";
            // 
            // pnlVariablesPreSql
            // 
            this.pnlVariablesPreSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariablesPreSql.Location = new System.Drawing.Point(68, 342);
            this.pnlVariablesPreSql.Name = "pnlVariablesPreSql";
            this.pnlVariablesPreSql.Size = new System.Drawing.Size(200, 22);
            this.pnlVariablesPreSql.TabIndex = 13;
            // 
            // tbPostSql
            // 
            this.tbPostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPostSql.Location = new System.Drawing.Point(-4, 0);
            this.tbPostSql.Multiline = true;
            this.tbPostSql.Name = "tbPostSql";
            this.tbPostSql.Size = new System.Drawing.Size(913, 335);
            this.tbPostSql.TabIndex = 21;
            // 
            // lblVariablesPostSql
            // 
            this.lblVariablesPostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVariablesPostSql.AutoSize = true;
            this.lblVariablesPostSql.BackColor = System.Drawing.Color.Transparent;
            this.lblVariablesPostSql.Location = new System.Drawing.Point(4, 346);
            this.lblVariablesPostSql.Name = "lblVariablesPostSql";
            this.lblVariablesPostSql.Size = new System.Drawing.Size(50, 13);
            this.lblVariablesPostSql.TabIndex = 20;
            this.lblVariablesPostSql.Text = "Variables";
            // 
            // btnInsertVariablePostSql
            // 
            this.btnInsertVariablePostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertVariablePostSql.Location = new System.Drawing.Point(294, 342);
            this.btnInsertVariablePostSql.Name = "btnInsertVariablePostSql";
            this.btnInsertVariablePostSql.Size = new System.Drawing.Size(109, 22);
            this.btnInsertVariablePostSql.TabIndex = 19;
            this.btnInsertVariablePostSql.Text = "Insert Variable";
            this.btnInsertVariablePostSql.UseVisualStyleBackColor = true;
            this.btnInsertVariablePostSql.Click += new System.EventHandler(this.btnInsertVariablePostSql_Click);
            // 
            // pnlVariablesPostSql
            // 
            this.pnlVariablesPostSql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariablesPostSql.BackColor = System.Drawing.Color.Transparent;
            this.pnlVariablesPostSql.Location = new System.Drawing.Point(68, 342);
            this.pnlVariablesPostSql.Name = "pnlVariablesPostSql";
            this.pnlVariablesPostSql.Size = new System.Drawing.Size(200, 22);
            this.pnlVariablesPostSql.TabIndex = 16;
            // 
            // tbSqlPreview
            // 
            this.tbSqlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSqlPreview.Location = new System.Drawing.Point(-4, 0);
            this.tbSqlPreview.Multiline = true;
            this.tbSqlPreview.Name = "tbSqlPreview";
            this.tbSqlPreview.Size = new System.Drawing.Size(913, 335);
            this.tbSqlPreview.TabIndex = 22;
            // 
            // btnSqlPreview
            // 
            this.btnSqlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSqlPreview.Location = new System.Drawing.Point(-1, 344);
            this.btnSqlPreview.Name = "btnSqlPreview";
            this.btnSqlPreview.Size = new System.Drawing.Size(109, 22);
            this.btnSqlPreview.TabIndex = 20;
            this.btnSqlPreview.Text = "Refresh";
            this.btnSqlPreview.UseVisualStyleBackColor = true;
            this.btnSqlPreview.Click += new System.EventHandler(this.btnSqlPreview_Click);
            // 
            // _checkUseCustomCommand
            // 
            this._checkUseCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._checkUseCustomCommand.AutoSize = true;
            this._checkUseCustomCommand.BackColor = System.Drawing.Color.Transparent;
            this._checkUseCustomCommand.Location = new System.Drawing.Point(4, 349);
            this._checkUseCustomCommand.Name = "_checkUseCustomCommand";
            this._checkUseCustomCommand.Size = new System.Drawing.Size(133, 17);
            this._checkUseCustomCommand.TabIndex = 25;
            this._checkUseCustomCommand.Text = "Use Custom Command";
            this._checkUseCustomCommand.UseVisualStyleBackColor = false;
            this._checkUseCustomCommand.CheckedChanged += new System.EventHandler(this._checkUseCustomCommand_CheckedChanged);
            // 
            // btnInsertDefaultMergeCommand
            // 
            this.btnInsertDefaultMergeCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInsertDefaultMergeCommand.Location = new System.Drawing.Point(741, 344);
            this.btnInsertDefaultMergeCommand.Name = "btnInsertDefaultMergeCommand";
            this.btnInsertDefaultMergeCommand.Size = new System.Drawing.Size(161, 24);
            this.btnInsertDefaultMergeCommand.TabIndex = 24;
            this.btnInsertDefaultMergeCommand.Text = "DB Command generieren";
            this.btnInsertDefaultMergeCommand.UseVisualStyleBackColor = true;
            this.btnInsertDefaultMergeCommand.Click += new System.EventHandler(this.btnInsertDefaultCustomCommand_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(233, 350);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Variables";
            // 
            // tbCustomMergeCommand
            // 
            this.tbCustomMergeCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCustomMergeCommand.Location = new System.Drawing.Point(-3, 0);
            this.tbCustomMergeCommand.Multiline = true;
            this.tbCustomMergeCommand.Name = "tbCustomMergeCommand";
            this.tbCustomMergeCommand.Size = new System.Drawing.Size(913, 335);
            this.tbCustomMergeCommand.TabIndex = 23;
            // 
            // btnInsertVarCustomCommand
            // 
            this.btnInsertVarCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertVarCustomCommand.Location = new System.Drawing.Point(495, 345);
            this.btnInsertVarCustomCommand.Name = "btnInsertVarCustomCommand";
            this.btnInsertVarCustomCommand.Size = new System.Drawing.Size(109, 22);
            this.btnInsertVarCustomCommand.TabIndex = 22;
            this.btnInsertVarCustomCommand.Text = "Insert Variable";
            this.btnInsertVarCustomCommand.UseVisualStyleBackColor = true;
            this.btnInsertVarCustomCommand.Click += new System.EventHandler(this.btnInsertVarCustomCommand_Click);
            // 
            // pnlVariablesCustomCommand
            // 
            this.pnlVariablesCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariablesCustomCommand.BackColor = System.Drawing.Color.Transparent;
            this.pnlVariablesCustomCommand.Location = new System.Drawing.Point(289, 345);
            this.pnlVariablesCustomCommand.Name = "pnlVariablesCustomCommand";
            this.pnlVariablesCustomCommand.Size = new System.Drawing.Size(200, 22);
            this.pnlVariablesCustomCommand.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(803, 349);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Log Level";
            // 
            // numLogLevel1
            // 
            this.numLogLevel1.Items.Add("3");
            this.numLogLevel1.Items.Add("2");
            this.numLogLevel1.Items.Add("1");
            this.numLogLevel1.Location = new System.Drawing.Point(863, 347);
            this.numLogLevel1.Name = "numLogLevel1";
            this.numLogLevel1.Size = new System.Drawing.Size(44, 20);
            this.numLogLevel1.TabIndex = 22;
            this.numLogLevel1.Text = "1";
            // 
            // tbMessage
            // 
            this.tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMessage.Location = new System.Drawing.Point(-3, 0);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(913, 335);
            this.tbMessage.TabIndex = 30;
            // 
            // btnInsert
            // 
            this.btnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsert.Location = new System.Drawing.Point(210, 346);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(109, 22);
            this.btnInsert.TabIndex = 29;
            this.btnInsert.Text = "Insert Variable";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // pnlVariableChooserLog
            // 
            this.pnlVariableChooserLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlVariableChooserLog.BackColor = System.Drawing.Color.Transparent;
            this.pnlVariableChooserLog.Location = new System.Drawing.Point(4, 346);
            this.pnlVariableChooserLog.Name = "pnlVariableChooserLog";
            this.pnlVariableChooserLog.Size = new System.Drawing.Size(200, 22);
            this.pnlVariableChooserLog.TabIndex = 28;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(840, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 26);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(753, 415);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 26);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblLayoutMapping
            // 
            this.lblLayoutMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLayoutMapping.AutoSize = true;
            this.lblLayoutMapping.Location = new System.Drawing.Point(5, 424);
            this.lblLayoutMapping.Name = "lblLayoutMapping";
            this.lblLayoutMapping.Size = new System.Drawing.Size(39, 13);
            this.lblLayoutMapping.TabIndex = 1;
            this.lblLayoutMapping.Text = "Layout";
            // 
            // cmbLayoutMapping
            // 
            this.cmbLayoutMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbLayoutMapping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayoutMapping.FormattingEnabled = true;
            this.cmbLayoutMapping.Items.AddRange(new object[] {
            "Standard",
            "Minimal",
            "SCD"});
            this.cmbLayoutMapping.Location = new System.Drawing.Point(50, 420);
            this.cmbLayoutMapping.Name = "cmbLayoutMapping";
            this.cmbLayoutMapping.Size = new System.Drawing.Size(144, 21);
            this.cmbLayoutMapping.TabIndex = 2;
            this.cmbLayoutMapping.SelectedIndexChanged += new System.EventHandler(this.cmbLayoutMapping_SelectedIndexChanged);
            // 
            // uTabConfig
            // 
            this.uTabConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uTabConfig.Controls.Add(this.uTabConfiguration);
            this.uTabConfig.Controls.Add(this.uTabMapping);
            this.uTabConfig.Controls.Add(this.uTabPreSQLStatement);
            this.uTabConfig.Controls.Add(this.uTabPostSQLStatement);
            this.uTabConfig.Controls.Add(this.uTabSQLPreview);
            this.uTabConfig.Controls.Add(this.uTabCustomCommand);
            this.uTabConfig.Controls.Add(this.uTabLogging);
            this.uTabConfig.Location = new System.Drawing.Point(4, 1);
            this.uTabConfig.Name = "uTabConfig";
            this.uTabConfig.SelectedIndex = 0;
            this.uTabConfig.Size = new System.Drawing.Size(917, 397);
            this.uTabConfig.TabIndex = 0;
            this.uTabConfig.TabStop = false;
            this.uTabConfig.TabIndexChanged += new System.EventHandler(this.uTabConfig_TabIndexChanged);
            // 
            // uTabConfiguration
            // 
            this.uTabConfiguration.Controls.Add(this.btnCreateTable);
            this.uTabConfiguration.Controls.Add(this.label11);
            this.uTabConfiguration.Controls.Add(this.btnAlterTable);
            this.uTabConfiguration.Controls.Add(this.lblChunkSizeBulk);
            this.uTabConfiguration.Controls.Add(this.btnCreateScdTable);
            this.uTabConfiguration.Controls.Add(this.label17);
            this.uTabConfiguration.Controls.Add(this.tbReattempts);
            this.uTabConfiguration.Controls.Add(this.label1);
            this.uTabConfiguration.Controls.Add(this.tbMaxThreadCount);
            this.uTabConfiguration.Controls.Add(this.label2);
            this.uTabConfiguration.Controls.Add(this.tbChunkSizeBulk);
            this.uTabConfiguration.Controls.Add(this.lblChunkSizeDbCommand);
            this.uTabConfiguration.Controls.Add(this.tbChunkSizeDbCommand);
            this.uTabConfiguration.Controls.Add(this.lblMaxThreadCount);
            this.uTabConfiguration.Controls.Add(this.tbTimeout);
            this.uTabConfiguration.Controls.Add(this.label14);
            this.uTabConfiguration.Controls.Add(this._checkStandardConfigAuto);
            this.uTabConfiguration.Controls.Add(this.lbDbCommand);
            this.uTabConfiguration.Controls.Add(this.checkDisableTablock);
            this.uTabConfiguration.Controls.Add(this.lblConMgrBulk);
            this.uTabConfiguration.Controls.Add(this.pnlConnMgrMain);
            this.uTabConfiguration.Controls.Add(this.label3);
            this.uTabConfiguration.Controls.Add(this.pnlConnMgrBulk);
            this.uTabConfiguration.Controls.Add(this.pnlTableLoaderType);
            this.uTabConfiguration.Controls.Add(this.imgHelpTransactions);
            this.uTabConfiguration.Controls.Add(this.pnlTransaction);
            this.uTabConfiguration.Controls.Add(this.imgHelpChunkSize);
            this.uTabConfiguration.Controls.Add(this.pnlCmbStandardConfig);
            this.uTabConfiguration.Controls.Add(this.imgHelpStandardConfig);
            this.uTabConfiguration.Controls.Add(this.pnlDestinationTanble);
            this.uTabConfiguration.Controls.Add(this.lblTransaction);
            this.uTabConfiguration.Controls.Add(this.pnlDbCommand);
            this.uTabConfiguration.Controls.Add(this.lblTlType);
            this.uTabConfiguration.Location = new System.Drawing.Point(4, 22);
            this.uTabConfiguration.Name = "uTabConfiguration";
            this.uTabConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.uTabConfiguration.Size = new System.Drawing.Size(909, 371);
            this.uTabConfiguration.TabIndex = 0;
            this.uTabConfiguration.Text = "Configuration";
            this.uTabConfiguration.UseVisualStyleBackColor = true;
            // 
            // uTabMapping
            // 
            this.uTabMapping.Controls.Add(this.tbPrefixOutput);
            this.uTabMapping.Controls.Add(this.pnlDGV);
            this.uTabMapping.Controls.Add(this.tbPrefixInput);
            this.uTabMapping.Controls.Add(this.label6);
            this.uTabMapping.Controls.Add(this.btnAutoMap);
            this.uTabMapping.Controls.Add(this.label5);
            this.uTabMapping.Controls.Add(this.btnSelect);
            this.uTabMapping.Controls.Add(this.btnAddRow);
            this.uTabMapping.Controls.Add(this.btnDeSelect);
            this.uTabMapping.Controls.Add(this.btnRemoveRow);
            this.uTabMapping.Location = new System.Drawing.Point(4, 22);
            this.uTabMapping.Name = "uTabMapping";
            this.uTabMapping.Padding = new System.Windows.Forms.Padding(3);
            this.uTabMapping.Size = new System.Drawing.Size(909, 371);
            this.uTabMapping.TabIndex = 1;
            this.uTabMapping.Text = "Mapping";
            this.uTabMapping.UseVisualStyleBackColor = true;
            // 
            // uTabPreSQLStatement
            // 
            this.uTabPreSQLStatement.Controls.Add(this.tbPreSql);
            this.uTabPreSQLStatement.Controls.Add(this._checkExcludePreSqlFromTransaction);
            this.uTabPreSQLStatement.Controls.Add(this.pnlVariablesPreSql);
            this.uTabPreSQLStatement.Controls.Add(this.btnInsertTruncatePreSql);
            this.uTabPreSQLStatement.Controls.Add(this.lblExcludePreSqlFromTransaction);
            this.uTabPreSQLStatement.Controls.Add(this.btnInsertVariablePreSql);
            this.uTabPreSQLStatement.Controls.Add(this.lblVariablesPreSql);
            this.uTabPreSQLStatement.Location = new System.Drawing.Point(4, 22);
            this.uTabPreSQLStatement.Name = "uTabPreSQLStatement";
            this.uTabPreSQLStatement.Size = new System.Drawing.Size(909, 371);
            this.uTabPreSQLStatement.TabIndex = 2;
            this.uTabPreSQLStatement.Text = "Pre SQL Statement";
            this.uTabPreSQLStatement.UseVisualStyleBackColor = true;
            // 
            // uTabPostSQLStatement
            // 
            this.uTabPostSQLStatement.Controls.Add(this.tbPostSql);
            this.uTabPostSQLStatement.Controls.Add(this.lblVariablesPostSql);
            this.uTabPostSQLStatement.Controls.Add(this.pnlVariablesPostSql);
            this.uTabPostSQLStatement.Controls.Add(this.btnInsertVariablePostSql);
            this.uTabPostSQLStatement.Location = new System.Drawing.Point(4, 22);
            this.uTabPostSQLStatement.Name = "uTabPostSQLStatement";
            this.uTabPostSQLStatement.Size = new System.Drawing.Size(909, 371);
            this.uTabPostSQLStatement.TabIndex = 3;
            this.uTabPostSQLStatement.Text = "Post SQL Statement";
            this.uTabPostSQLStatement.UseVisualStyleBackColor = true;
            // 
            // uTabSQLPreview
            // 
            this.uTabSQLPreview.Controls.Add(this.tbSqlPreview);
            this.uTabSQLPreview.Controls.Add(this.btnSqlPreview);
            this.uTabSQLPreview.Location = new System.Drawing.Point(4, 22);
            this.uTabSQLPreview.Name = "uTabSQLPreview";
            this.uTabSQLPreview.Size = new System.Drawing.Size(909, 371);
            this.uTabSQLPreview.TabIndex = 4;
            this.uTabSQLPreview.Text = "SQL Preview";
            this.uTabSQLPreview.UseVisualStyleBackColor = true;
            // 
            // uTabCustomCommand
            // 
            this.uTabCustomCommand.Controls.Add(this._checkUseCustomCommand);
            this.uTabCustomCommand.Controls.Add(this.tbCustomMergeCommand);
            this.uTabCustomCommand.Controls.Add(this.btnInsertDefaultMergeCommand);
            this.uTabCustomCommand.Controls.Add(this.pnlVariablesCustomCommand);
            this.uTabCustomCommand.Controls.Add(this.label4);
            this.uTabCustomCommand.Controls.Add(this.btnInsertVarCustomCommand);
            this.uTabCustomCommand.Location = new System.Drawing.Point(4, 22);
            this.uTabCustomCommand.Name = "uTabCustomCommand";
            this.uTabCustomCommand.Size = new System.Drawing.Size(909, 371);
            this.uTabCustomCommand.TabIndex = 5;
            this.uTabCustomCommand.Text = "Custom Command";
            this.uTabCustomCommand.UseVisualStyleBackColor = true;
            // 
            // uTabLogging
            // 
            this.uTabLogging.Controls.Add(this.label7);
            this.uTabLogging.Controls.Add(this.tbMessage);
            this.uTabLogging.Controls.Add(this.numLogLevel1);
            this.uTabLogging.Controls.Add(this.pnlVariableChooserLog);
            this.uTabLogging.Controls.Add(this.btnInsert);
            this.uTabLogging.Location = new System.Drawing.Point(4, 22);
            this.uTabLogging.Name = "uTabLogging";
            this.uTabLogging.Size = new System.Drawing.Size(909, 371);
            this.uTabLogging.TabIndex = 6;
            this.uTabLogging.Text = "Logging";
            this.uTabLogging.UseVisualStyleBackColor = true;
            // 
            // frmTableLoaderUI
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(927, 453);
            this.Controls.Add(this.uTabConfig);
            this.Controls.Add(this.cmbLayoutMapping);
            this.Controls.Add(this.lblLayoutMapping);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(943, 491);
            this.Name = "frmTableLoaderUI";
            this.Text = "Table Loader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTableLoaderUI_FormClosing);
            this.Load += new System.EventHandler(this.frmTableLoaderUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpStandardConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpChunkSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHelpTransactions)).EndInit();
            this.uTabConfig.ResumeLayout(false);
            this.uTabConfiguration.ResumeLayout(false);
            this.uTabConfiguration.PerformLayout();
            this.uTabMapping.ResumeLayout(false);
            this.uTabMapping.PerformLayout();
            this.uTabPreSQLStatement.ResumeLayout(false);
            this.uTabPreSQLStatement.PerformLayout();
            this.uTabPostSQLStatement.ResumeLayout(false);
            this.uTabPostSQLStatement.PerformLayout();
            this.uTabSQLPreview.ResumeLayout(false);
            this.uTabSQLPreview.PerformLayout();
            this.uTabCustomCommand.ResumeLayout(false);
            this.uTabCustomCommand.PerformLayout();
            this.uTabLogging.ResumeLayout(false);
            this.uTabLogging.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox imgHelpTransactions;
        private System.Windows.Forms.Button btnAutoMap;
        private System.Windows.Forms.Panel pnlDGV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel pnlConnMgrBulk;
        private System.Windows.Forms.Panel pnlConnMgrMain;
        private System.Windows.Forms.Label lblTransaction;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblChunkSizeBulk;
        private System.Windows.Forms.Panel pnlDbCommand;
        private System.Windows.Forms.Label lbDbCommand;
        private System.Windows.Forms.Panel pnlTransaction;
        private System.Windows.Forms.Panel pnlVariablesPreSql;
        private System.Windows.Forms.Panel pnlVariablesPostSql;
        private System.Windows.Forms.PictureBox imgHelpChunkSize;
        private System.Windows.Forms.Label lblMaxThreadCount;
        private System.Windows.Forms.Panel pnlVariablesCustomCommand;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlCmbStandardConfig;
        private System.Windows.Forms.PictureBox imgHelpStandardConfig;
        private System.Windows.Forms.Label lblTlType;
        private System.Windows.Forms.Label lblChunkSizeDbCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlVariableChooserLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblExcludePreSqlFromTransaction;
        private System.Windows.Forms.Label lblLayoutMapping;
        private System.Windows.Forms.Label lblConMgrBulk;
        private System.Windows.Forms.CheckBox checkDisableTablock;
        private System.Windows.Forms.TextBox tbTimeout;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox tbChunkSizeDbCommand;
        private System.Windows.Forms.TextBox tbChunkSizeBulk;
        private System.Windows.Forms.TextBox tbMaxThreadCount;
        private System.Windows.Forms.TextBox tbReattempts;
        private System.Windows.Forms.Button btnCreateScdTable;
        private System.Windows.Forms.Button btnCreateTable;
        private System.Windows.Forms.Button btnAlterTable;
        private System.Windows.Forms.Panel pnlTableLoaderType;
        private System.Windows.Forms.CheckBox _checkStandardConfigAuto;
        private System.Windows.Forms.Button btnInsertTruncatePreSql;
        private System.Windows.Forms.Button btnInsertVariablePreSql;
        private System.Windows.Forms.Button btnInsertVariablePostSql;
        private System.Windows.Forms.Button btnInsertVarCustomCommand;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.CheckBox _checkExcludePreSqlFromTransaction;
        private System.Windows.Forms.TextBox tbPreSql;
        private System.Windows.Forms.Label lblVariablesPreSql;
        private System.Windows.Forms.TextBox tbPostSql;
        private System.Windows.Forms.Label lblVariablesPostSql;
        private System.Windows.Forms.Panel pnlDestinationTanble;
        private System.Windows.Forms.TextBox tbSqlPreview;
        private System.Windows.Forms.Button btnSqlPreview;
        private System.Windows.Forms.Button btnInsertDefaultMergeCommand;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbCustomMergeCommand;
        private System.Windows.Forms.CheckBox _checkUseCustomCommand;
        private System.Windows.Forms.DomainUpDown numLogLevel1;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPrefixOutput;
        private System.Windows.Forms.TextBox tbPrefixInput;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnDeSelect;
        private System.Windows.Forms.Button btnRemoveRow;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.ComboBox cmbLayoutMapping;
        private System.Windows.Forms.TabControl uTabConfig;
        private System.Windows.Forms.TabPage uTabConfiguration;
        private System.Windows.Forms.TabPage uTabMapping;
        private System.Windows.Forms.TabPage uTabPreSQLStatement;
        private System.Windows.Forms.TabPage uTabPostSQLStatement;
        private System.Windows.Forms.TabPage uTabSQLPreview;
        private System.Windows.Forms.TabPage uTabCustomCommand;
        private System.Windows.Forms.TabPage uTabLogging;
       
  
       
    }
}