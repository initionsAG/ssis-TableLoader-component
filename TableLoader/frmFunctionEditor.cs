using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;

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

            ValueList valueList = new ValueList();
            foreach (string columnName in inputColumnNameList) valueList.ValueListItems.Add(columnName);
            cbColumnList.ValueList =  valueList;
            cbColumnList.Text = inputColumnName;
        }

        private string GetInputReference()
        {
            return "@(" + cbColumnList.Text + ")";

            //string inputReference = "";

            //switch (_dbCommand)
            //{
            //    case IsagCustomProperties.DbCommandType.Merge:
            //        inputReference = "src." + _inputColumnName;
            //        break;
            //    case IsagCustomProperties.DbCommandType.Merge2005:
            //        inputReference = "src." + _inputColumnName;
            //        break;
            //    case IsagCustomProperties.DbCommandType.UpdateTblInsertRow:
            //        inputReference = "@" + _inputColumnName;
            //        break;
            //    case IsagCustomProperties.DbCommandType.UpdateRowInsertRow:
            //        inputReference = "@" + _inputColumnName;
            //        break;
            //    case IsagCustomProperties.DbCommandType.BulkInsert:
            //        break;
            //    case IsagCustomProperties.DbCommandType.Insert:
            //        inputReference = _inputColumnName;
            //        break;
            //    default:
            //        break;
            //}

            //return inputReference;
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
            System.Windows.Forms.Clipboard.SetDataObject(value, true);
            tbValue.EditInfo.Paste();
        }
    }
}
