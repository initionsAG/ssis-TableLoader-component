using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace TableLoader.Framework.Gui
{
    /// <summary>
    /// a GUI selecting a SSIS variable
    /// (used by the IsagVariableChooser)
    /// </summary>
    public partial class frmSelectVariable : Form
    {
        DataView _dvVariables;

        public frmSelectVariable(Variables variables, Icon icon)
        {
            InitializeComponent();
            this.Icon = icon;

            createDataView(variables);
        }

        public object SelectedValue {
            get {

                foreach (DataGridViewRow row in dgvVariables.SelectedRows)
                {
                    return row.Cells["QualifiedName"].Value;
                }

                return null;
            }
        }

        public string SelectedVariable
        {
            get
            {

                foreach (DataGridViewRow row in dgvVariables.SelectedRows)
                {
                    string selectedVariable;
                    selectedVariable = row.Cells["QualifiedName"].Value == null ? "" : row.Cells["QualifiedName"].Value.ToString();
                    return selectedVariable;
                }

                return "";
            }
        }

        private void createDataView(Variables variables)
        {
            _dvVariables = new DataView();
            DataTable dt = new DataTable("Variables");

            dt.Columns.Add("Namespace", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Systemvariable", typeof(bool));
            dt.Columns.Add("Value", typeof(object));            
            dt.Columns.Add("DataType", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("QualifiedName", typeof(string));

            foreach (Variable var in variables)
            {
                DataRow row = dt.NewRow();

                row[0] = var.Namespace;
                row[1] = var.Name;
                row[2] = var.SystemVariable;
                row[3] = var.Value;                
                row[4] = var.DataType;
                row[5] = var.Description;
                row[6] = var.QualifiedName;

                dt.Rows.Add(row);
            }
            
            _dvVariables.Table = dt;
            _dvVariables.Sort = "Name";
            dgvVariables.DataSource = _dvVariables;
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            string filter = "";

            if (!(checkSystemVar.Checked && CheckUserVar.Checked)) filter = "Systemvariable = " + (checkSystemVar.Checked).ToString() + " And ";
            if ((!checkSystemVar.Checked && !CheckUserVar.Checked)) filter = "False = True And ";
            filter += String.Format("Name LIKE '*{0}*'", tbFilter.Text);

            _dvVariables.RowFilter = filter;
        }

        

    }
}
