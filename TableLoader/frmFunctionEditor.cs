using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TableLoader
{
    /// <summary>
    /// Function editor
    /// </summary>
    public partial class frmFunctionEditor : Form
    {
        /// <summary>
        /// function 
        /// </summary>
        public string Value {
            get { return tbValue.Text;}
        }

        /// <summary>
        /// input column name
        /// </summary>
        private string _inputColumnName;

        /// <summary>
        /// Database command type
        /// </summary>
        private IsagCustomProperties.DbCommandType _dbCommand;

        /// <summary>
        /// output column datatype
        /// </summary>
        private string _outputColumnDataType;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inputColumnName">input column name</param>
        /// <param name="outputColumnDataType">output column datatype</param>
        /// <param name="dbCommand">database command type</param>
        /// <param name="functionValue">function</param>
        /// <param name="inputColumnNameList">input column name list</param>
        public frmFunctionEditor(string inputColumnName, string outputColumnDataType, IsagCustomProperties.DbCommandType dbCommand,
                                 string functionValue, string[] inputColumnNameList)
        {
            InitializeComponent();

            _inputColumnName = inputColumnName;
            _dbCommand = dbCommand;
            tbValue.Text = functionValue;
            _outputColumnDataType = outputColumnDataType;

            cbColumnList.Items.AddRange(inputColumnNameList);
            cbColumnList.Text = inputColumnName;
        }

        /// <summary>
        /// Get reference for input column
        /// </summary>
        /// <returns></returns>
        private string GetInputReference()
        {
            return "@(" + cbColumnList.Text + ")";
        }

        /// <summary>
        /// React on button Insert click: insert selected input columns reference into function
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertInputColumn_Click(object sender, EventArgs e)
        {
            Insert(GetInputReference());
        }

        /// <summary>
        /// React on button "Insert with default" click: insert selected input columns reference plus default value into function
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnInsertInputColumnWithDefault_Click(object sender, EventArgs e)
        {
            Insert("isnull(cast(" + GetInputReference() + " as " + _outputColumnDataType + "),  <DefaultValue>)");
        }

        /// <summary>
        /// Insert value into function
        /// </summary>
        /// <param name="value"></param>
        private void Insert(string value) 
        {
            tbValue.Text = tbValue.Text.Insert(tbValue.SelectionStart, value);
        }
    }
}
