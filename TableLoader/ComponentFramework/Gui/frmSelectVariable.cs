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
        /// <summary>
        /// dataview with variables
        /// </summary>
        private DataView _dvVariables;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="variables">SSIS variables</param>
        /// <param name="icon">window icon</param>
        public frmSelectVariable(Variables variables, Icon icon)
        {
            InitializeComponent();
            this.Icon = icon;

            createDataView(variables);
        }

        /// <summary>
        /// Gets the variables qualified name from the first selected row
        /// </summary>
        public object SelectedValue {
            get {

                foreach (DataGridViewRow row in dgvVariables.SelectedRows)
                {
                    return row.Cells["QualifiedName"].Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the variables qualified name from the first selected row (returns an empty string if value is null)
        /// </summary>
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

        /// <summary>
        /// creates a dataview from SSIS variables
        /// </summary>
        /// <param name="variables"></param>
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

        /// <summary>
        /// filters datagridview if filter has changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
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
