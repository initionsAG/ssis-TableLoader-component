namespace TableLoader.Framework.Gui
{
    partial class IsagVariableChooser
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
            this.cmbVariableChooser = new System.Windows.Forms.ComboBox();
            this.btnSelectVariable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbVariableChooser
            // 
            this.cmbVariableChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbVariableChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariableChooser.FormattingEnabled = true;
            this.cmbVariableChooser.Location = new System.Drawing.Point(0, 0);
            this.cmbVariableChooser.Margin = new System.Windows.Forms.Padding(0);
            this.cmbVariableChooser.Name = "cmbVariableChooser";
            this.cmbVariableChooser.Size = new System.Drawing.Size(200, 21);
            this.cmbVariableChooser.Sorted = true;
            this.cmbVariableChooser.TabIndex = 0;
            // 
            // btnSelectVariable
            // 
            this.btnSelectVariable.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSelectVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectVariable.Font = new System.Drawing.Font("Microsoft Sans Serif", 3F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectVariable.Location = new System.Drawing.Point(180, 0);
            this.btnSelectVariable.Margin = new System.Windows.Forms.Padding(0);
            this.btnSelectVariable.MaximumSize = new System.Drawing.Size(20, 21);
            this.btnSelectVariable.MinimumSize = new System.Drawing.Size(20, 21);
            this.btnSelectVariable.Name = "btnSelectVariable";
            this.btnSelectVariable.Size = new System.Drawing.Size(20, 21);
            this.btnSelectVariable.TabIndex = 1;
            this.btnSelectVariable.Text = ". . .";
            this.btnSelectVariable.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSelectVariable.UseVisualStyleBackColor = true;
            this.btnSelectVariable.Click += new System.EventHandler(this.btnSelectVariable_Click);
            // 
            // IsagVariableChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSelectVariable);
            this.Controls.Add(this.cmbVariableChooser);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "IsagVariableChooser";
            this.Size = new System.Drawing.Size(200, 22);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbVariableChooser;
        private System.Windows.Forms.Button btnSelectVariable;
    }
}
