namespace ComponentFramework.Gui
{
    partial class IsagConnectionManager
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbConMgr = new System.Windows.Forms.ComboBox();
            this.btnNewConMgr = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbConMgr
            // 
            this.cmbConMgr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbConMgr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConMgr.FormattingEnabled = true;
            this.cmbConMgr.Location = new System.Drawing.Point(0, 6);
            this.cmbConMgr.Name = "cmbConMgr";
            this.cmbConMgr.Size = new System.Drawing.Size(524, 21);
            this.cmbConMgr.TabIndex = 0;
            this.cmbConMgr.SelectedValueChanged += new System.EventHandler(this.cmbConMgr_SelectedValueChanged);
            // 
            // btnNewConMgr
            // 
            this.btnNewConMgr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewConMgr.Location = new System.Drawing.Point(533, 6);
            this.btnNewConMgr.Name = "btnNewConMgr";
            this.btnNewConMgr.Size = new System.Drawing.Size(38, 21);
            this.btnNewConMgr.TabIndex = 1;
            this.btnNewConMgr.Text = "New";
            this.btnNewConMgr.UseVisualStyleBackColor = true;
            this.btnNewConMgr.Click += new System.EventHandler(this.btnNewConMgr_Click);
            // 
            // IsagConnectionManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNewConMgr);
            this.Controls.Add(this.cmbConMgr);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "IsagConnectionManager";
            this.Size = new System.Drawing.Size(571, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbConMgr;
        private System.Windows.Forms.Button btnNewConMgr;
    }
}
