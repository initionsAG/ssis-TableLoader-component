using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using TableLoader.ComponentFramework.Mapping;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace TableLoader
{
    public class ColumnConfig : IXmlSerializable
    {
        #region Properties

        private SqlColumnList _sqlColumns;

        private bool _insert;
        [DisplayName("Use\n(Insert)")]
        public bool Insert
        {
            get { return _insert; }
            set { _insert = value;}
        }

        private bool _update;
        [DisplayName("Use\n(Update)")]
        public bool Update
        {
            get { return _update; }
            set { _update = value; }
        }

        private bool _key;
        [DisplayName("Key")]
        public bool Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _inputColumnName;
        [DisplayName("Input Column"), ReadOnly(true)]
        public string InputColumnName
        {
            get { return _inputColumnName; }
            set { _inputColumnName = value; }
        }

        private string _outputColumnName;
        [DisplayName("Output Column")]
        public string OutputColumnName
        {
            get { return _outputColumnName; }
            set
            {
                _outputColumnName = value;
                if (_sqlColumns != null) SetOutputColumnDefinition();
            }
        }

        private string _default;
        [DisplayName("Default")]
        public string Default
        {
            get { return _default; }
            set {
                if (value == null) _default = "";
                else _default = value; 
            }
        }

        private string _function;
        [DisplayName("Function")]
        public string Function
        {
            get { return _function; }
            set
            {
                if (value == null) _function = "";
                else _function = value;
            }
        }


        private string _dataTypeInput;
        [DisplayName("DataType\n(Input)"), ReadOnly(true)]
        public string DataTypeInput
        {
            get { return _dataTypeInput; }
            set { _dataTypeInput = value; }
        }

        private string _dataTypeOutput;
        [DisplayName("DataType\n(Output)"), ReadOnly(true)]
        public string DataTypeOutput
        {
            get { return _dataTypeOutput; }
            set { _dataTypeOutput = value; }
        }

        private string _dataTypeOutputNet;
        [BrowsableAttribute(false), ReadOnly(true)]
        public string DataTypeOutputNet
        {
            get { return _dataTypeOutputNet; }
            set { _dataTypeOutputNet = value; }
        }

        private bool _isOutputPrimaryKey;
        [DisplayName("PK\n(Output)"), ReadOnly(true)]
        public bool IsOutputPrimaryKey
        {
            get { return _isOutputPrimaryKey; }
            set { _isOutputPrimaryKey = value; }
        }

        private bool _allowOutputDbNull;
        [DisplayName("Null\n(Output)"), ReadOnly(true)]
        public bool AllowOutputDbNull
        {
            get { return _allowOutputDbNull; }
            set { _allowOutputDbNull = value; }
        }

        private bool _isOutputAutoId;
        [DisplayName("AutoID\n(Output)"), ReadOnly(true)]
        public bool IsOutputAutoId
        {
            get { return _isOutputAutoId; }
            set { _isOutputAutoId = value;}
        }

        private int _inputColumnId;
        [BrowsableAttribute(false), ReadOnly(true)]
        public int InputColumnId
        {
            get { return _inputColumnId; }
            set { _inputColumnId = value; }
        }

        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasInput { get { return (InputColumnName != null && InputColumnName != ""); } }

        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasOutput { get { return (OutputColumnName != null && OutputColumnName != ""); } }

        [XmlIgnore, BrowsableAttribute(false)]
        public bool IsInputColumnUsed { get { return (Insert || Update || Key || IsScdValidFrom); } }

        [XmlIgnore, BrowsableAttribute(false)]
        public string BulkColumnName
        {
            get
            {
                if (HasInput) return InputColumnName;
                else return OutputColumnName;
            }
        }

        [XmlIgnore, BrowsableAttribute(false)]
        public string BulkDataType
        {
            get
            {
                if (HasInput) return DataTypeInput;
                else return DataTypeOutput;
            }
        }

        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasFunction
        {
            get
            {
                return (Function != null && Function != "");
            }
        }

        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasDefault
        {
            get
            {
                return (Default != null && Default != "");
            }
        }

        private string _customId;
        [BrowsableAttribute(false), ReadOnly(true)]
        public string CustomId
        {
            get { return _customId; }
            set { _customId = value; }
        }

        [DisplayName("SCD Column")]
        public bool IsScdColumn { get; set; }

        [DisplayName("SCD Table")]
        public string ScdTable { get; set; }

        [DisplayName("SCD ValidFrom")]
        public bool IsScdValidFrom { get; set; }


        [XmlIgnore, BrowsableAttribute(false), ReadOnly(true)]
        public bool HasScd
        {
            get { return IsScdColumn || IsScdValidFrom || !string.IsNullOrEmpty(ScdTable); }           
        }
        #endregion

        #region Constructor

        public ColumnConfig() { }

        public ColumnConfig(bool insert, bool update, bool key, string inputColumnName, string outputColumnName, string defaultValue,
                            string function, string dataTypeInput, string dataTypeOutput, string dataTypeOutputNet,
                            bool isOutputPrimaryKey, bool outputNullAllowed, bool isOutputAutoId, object inputColumnId)
        {
            _insert = insert;
            _update = update;
            _key = key;

            _inputColumnName = inputColumnName;
            _outputColumnName = outputColumnName;

            _default = defaultValue;
            _function = function;

            _dataTypeInput = dataTypeInput;
            _dataTypeOutput = dataTypeOutput;
            _dataTypeOutputNet = dataTypeOutputNet;

            _isOutputPrimaryKey = isOutputPrimaryKey;
            _allowOutputDbNull = outputNullAllowed;
            _isOutputAutoId = isOutputAutoId;

            if (inputColumnId != null) _inputColumnId = (int) inputColumnId;
        }


        public ColumnConfig(string inputColumnName, string dataTypeInput, IDTSInputColumn100 inputColumn)
        {
            _insert = false;
            _update = false;
            _key = false;

            _inputColumnName = inputColumnName;
            _outputColumnName = "";

            _default = "";
            _function = "";

            _dataTypeInput = dataTypeInput;
            _dataTypeOutput = "";
            _dataTypeOutputNet = "";

            _isOutputPrimaryKey = false;
            _allowOutputDbNull = false;
            _isOutputAutoId = false;
            
            _inputColumnId = inputColumn.ID;

            //ID Mapping
            _customId = Guid.NewGuid().ToString();
            Mapping.SetIdProperty(_customId, inputColumn.CustomPropertyCollection);
        }
        #endregion

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            reader.MoveToContent();

            try
            {
                _insert = (reader.GetAttribute("Insert") == "True");
                _update = (reader.GetAttribute("Update") == "True");
                _key = (reader.GetAttribute("Key") == "True");

                _inputColumnName = reader.GetAttribute("InputColumnName");
                _outputColumnName = reader.GetAttribute("OutputColumnName");


                _default = reader.GetAttribute("Default");
                _function = reader.GetAttribute("Function");

                _dataTypeInput = reader.GetAttribute("DataTypeInput");
                _dataTypeOutput = reader.GetAttribute("DataTypeOutput");
                _dataTypeOutputNet = reader.GetAttribute("DataTypeOutputNet");

                _isOutputPrimaryKey = (reader.GetAttribute("IsOutputPrimaryKey") == "True");
                _allowOutputDbNull = (reader.GetAttribute("AllowOutputDbNull") == "True");
                _isOutputAutoId = (reader.GetAttribute("IsOutputAutoId") == "True");

                _inputColumnId = Int32.Parse(reader.GetAttribute("InputColumnId"));

                _customId = reader.GetAttribute("CustomId");

                IsScdColumn = reader.GetAttribute("IsScdColumn") == "True";
                ScdTable = reader.GetAttribute("ScdTable");
                IsScdValidFrom = reader.GetAttribute("IsScdValidFrom") == "True";

                reader.Read();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                throw;
            }


        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            writer.WriteAttributeString("Insert", _insert.ToString());
            writer.WriteAttributeString("Update", _update.ToString());
            writer.WriteAttributeString("Key", _key.ToString());

            writer.WriteAttributeString("InputColumnName", _inputColumnName);
            writer.WriteAttributeString("OutputColumnName", _outputColumnName);

            writer.WriteAttributeString("Default", _default);
            writer.WriteAttributeString("Function", _function);

            writer.WriteAttributeString("DataTypeInput", _dataTypeInput);
            writer.WriteAttributeString("DataTypeOutput", _dataTypeOutput);
            writer.WriteAttributeString("DataTypeOutputNet", _dataTypeOutputNet);

            writer.WriteAttributeString("IsOutputPrimaryKey", _isOutputPrimaryKey.ToString());
            writer.WriteAttributeString("AllowOutputDbNull", _allowOutputDbNull.ToString());
            writer.WriteAttributeString("IsOutputAutoId", _isOutputAutoId.ToString());

            writer.WriteAttributeString("InputColumnId", _inputColumnId.ToString());

            writer.WriteAttributeString("CustomId", _customId);

            writer.WriteAttributeString("IsScdColumn", IsScdColumn.ToString());
            writer.WriteAttributeString("ScdTable", ScdTable);
            writer.WriteAttributeString("IsScdValidFrom", IsScdValidFrom.ToString());
        }

        #endregion

        public void SetSqlColumnDefinitions(SqlColumnList sqlColumns)
        {
            _sqlColumns = sqlColumns;
            SetOutputColumnDefinition();
        }

        private void SetOutputColumnDefinition()
        {
            if (this.OutputColumnName != null && _sqlColumns.ContainsKey(this.OutputColumnName))
            {
                this.DataTypeOutputNet = _sqlColumns[this.OutputColumnName].DataTypeNet;
                this.DataTypeOutput = _sqlColumns[this.OutputColumnName].GetDataTypeDescription();
                this.IsOutputPrimaryKey = _sqlColumns[this.OutputColumnName].IsPrimaryKey;
                this.IsOutputAutoId = _sqlColumns[this.OutputColumnName].IsAutoId;
                this.AllowOutputDbNull = _sqlColumns[this.OutputColumnName].AllowsDbNull;
            }
            else
            {
                this.DataTypeOutput = "";
                this.IsOutputPrimaryKey = false;
                this.IsOutputAutoId = false;
                this.AllowOutputDbNull = false;
            }
        }      

        public void RemoveOutput()
        {
            _outputColumnName = "";
            _dataTypeOutput = "";
            _isOutputPrimaryKey = false;
            _isOutputAutoId = false;
            _allowOutputDbNull = false;
            //_sqlColumns = null;
        }

        public void AutoMap(string inputPrefix, string outputPrefix)
        {
            if (_sqlColumns != null)
                OutputColumnName = _sqlColumns.GetMatchingColumnname(InputColumnName, inputPrefix, outputPrefix);
        }

        public string GetColumnExpression()
        {
            string result;

            if (HasDefault)
            {
                result = " isnull(cast(src." + SqlCreator.Brackets(BulkColumnName) + " as " + DataTypeOutput + ") ," + Default + ")";
            }
            else if (HasFunction)
            {
                result = Function + " ";
            }
            else
            { //kein Default Value vorhanden
                result = "src." + SqlCreator.Brackets(BulkColumnName) + " ";
            }

            return result;
        }

    }
}
