using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace TableLoader.Framework.Gui
{
    /// <summary>
    /// a control for choosing a ssis variable
    /// (needs to be initialzed by Initialize(Variables variables, Icon icon)
    /// </summary>
    public partial class IsagVariableChooser : UserControl
    {
        /// <summary>
        /// SSIS variables
        /// </summary>
        private Variables _variables;

        /// <summary>
        /// the GUI (own window) for selecting a variable
        /// </summary>
        private frmSelectVariable _frmSelectVariable = null;

        /// <summary>
        /// the GUIs window icon
        /// </summary>
        private Icon _icon;

        /// <summary>
        /// datasource that contains variables and descriptions
        /// </summary>
        private DataView _dataView;

        /// <summary>
        ///the selected variable
        /// </summary>
        public string SelectedVariable
        {
            get
            {
                return cmbVariableChooser.Text; 
            }
        }

        /// <summary>
        /// the selected variable enclosed by "@()"
        /// </summary>
        public string SelectedFormattedVariable
        {
            get
            {
                return "@(" + SelectedVariable + ")"; 
            }
        }

        /// <summary>
        /// the constructor
        /// </summary>
        public IsagVariableChooser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control
        /// </summary>
        /// <param name="variables">the ssis variable collection</param>
        /// <param name="icon">the components icon for frmSelectVariable</param>
        public void Initialize(Variables variables, Icon icon)
        {
            _icon = icon;
            _variables = variables;
            _dataView = CreateDataSource(_variables);
            cmbVariableChooser.DisplayMember = "QualifiedName";
            cmbVariableChooser.DataSource = _dataView;
        }

        /// <summary>
        /// Shows the GUI for selecting variables
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnSelectVariable_Click(object sender, EventArgs e)
        {
            if (_frmSelectVariable == null) _frmSelectVariable = new frmSelectVariable(_variables, _icon);

            if (_frmSelectVariable.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                cmbVariableChooser.Text = _frmSelectVariable.SelectedVariable;
            }
        }

        /// <summary>
        /// creates datasource for combobox and GUI (datagridview)
        /// </summary>
        /// <param name="variables"></param>
        /// <returns>returns the dataource as a dataview </returns>
        private DataView CreateDataSource(Variables variables)
        {
            DataView result = new DataView();
            DataTable dt = new DataTable("Variables");
            List<string> varList  = new List<string>();

            dt.Columns.Add("Namespace", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Systemvariable", typeof(bool));
            dt.Columns.Add("Value", typeof(object));
            dt.Columns.Add("DataType", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("QualifiedName", typeof(string));

            foreach (Variable var in variables)
            {
                if (!varList.Contains(var.QualifiedName))
                {
                    varList.Add(var.QualifiedName);
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
            }

            result.Table = dt;
            result.Sort = "Name";
            return result;
        }

    }
}
