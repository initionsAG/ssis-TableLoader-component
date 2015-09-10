using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TableLoader
{
    public partial class frmFunctionEditor : Form
    {
        public string Value {
            get { return tbValue.Text;}
        }

        private string _inputColumnName;
        private IsagCustomProperties.DbCommandType _dbCommand;
        private string _outputColumnDataType;

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

        private string GetInputReference()
        {
            return "@(" + cbColumnList.Text + ")";
        }
        private void btnInsertInputColumn_Click(object sender, EventArgs e)
        {
            Insert(GetInputReference());
        }

        private void btnInsertInputColumnWithDefault_Click(object sender, EventArgs e)
        {
            Insert("isnull(cast(" + GetInputReference() + " as " + _outputColumnDataType + "),  <DefaultValue>)");
        }

        private void Insert(string value) 
        {
            tbValue.Text = tbValue.Text.Insert(tbValue.SelectionStart, value);
        }
    }
}
