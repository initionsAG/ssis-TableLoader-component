namespace TableLoader
{
    partial class frmFunctionEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFunctionEditor));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.btnInsertInputColumn = new System.Windows.Forms.Button();
            this.btnInsertInputColumnWithDefault1 = new System.Windows.Forms.Button();
            this.cbColumnList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(572, 420);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(653, 420);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tbValue
            // 
            this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValue.Location = new System.Drawing.Point(12, 12);
            this.tbValue.Multiline = true;
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(716, 402);
            this.tbValue.TabIndex = 15;
            // 
            // btnInsertInputColumn
            // 
            this.btnInsertInputColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertInputColumn.Location = new System.Drawing.Point(12, 420);
            this.btnInsertInputColumn.Name = "btnInsertInputColumn";
            this.btnInsertInputColumn.Size = new System.Drawing.Size(75, 23);
            this.btnInsertInputColumn.TabIndex = 16;
            this.btnInsertInputColumn.Text = "Insert InputColumn";
            this.btnInsertInputColumn.UseVisualStyleBackColor = true;
            this.btnInsertInputColumn.Click += new System.EventHandler(this.btnInsertInputColumn_Click);
            // 
            // btnInsertInputColumnWithDefault1
            // 
            this.btnInsertInputColumnWithDefault1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsertInputColumnWithDefault1.Location = new System.Drawing.Point(93, 420);
            this.btnInsertInputColumnWithDefault1.Name = "btnInsertInputColumnWithDefault1";
            this.btnInsertInputColumnWithDefault1.Size = new System.Drawing.Size(172, 23);
            this.btnInsertInputColumnWithDefault1.TabIndex = 16;
            this.btnInsertInputColumnWithDefault1.Text = "Insert InputColumn with Default";
            this.btnInsertInputColumnWithDefault1.UseVisualStyleBackColor = true;
            this.btnInsertInputColumnWithDefault1.Click += new System.EventHandler(this.btnInsertInputColumnWithDefault_Click);
            // 
            // cbColumnList
            // 
            this.cbColumnList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbColumnList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColumnList.FormattingEnabled = true;
            this.cbColumnList.Location = new System.Drawing.Point(297, 421);
            this.cbColumnList.Name = "cbColumnList";
            this.cbColumnList.Size = new System.Drawing.Size(144, 21);
            this.cbColumnList.Sorted = true;
            this.cbColumnList.TabIndex = 17;
            // 
            // frmFunctionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 455);
            this.Controls.Add(this.cbColumnList);
            this.Controls.Add(this.btnInsertInputColumnWithDefault1);
            this.Controls.Add(this.btnInsertInputColumn);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFunctionEditor";
            this.Text = "TableLoader: Function Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Button btnInsertInputColumn;
        private System.Windows.Forms.Button btnInsertInputColumnWithDefault1;
        private System.Windows.Forms.ComboBox cbColumnList;

    }
}