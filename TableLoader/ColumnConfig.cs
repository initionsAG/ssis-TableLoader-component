using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Runtime.CompilerServices;
using TableLoader.Framework.Mapping;

namespace TableLoader
{
    /// <summary>
    /// input column configuration
    /// </summary>
    public class ColumnConfig: IXmlSerializable, INotifyPropertyChanged
    {
        /// <summary>
        /// Interface for INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Triggers PropertyChangedEventHandler
        /// </summary>
        /// <param name="info">property name</param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        #region Properties

        /// <summary>
        /// List of columns of the destination table
        /// </summary>
        private SqlColumnList _sqlColumns;

        /// <summary>
        /// Insert column value into the destination table?
        /// </summary>
        private bool _insert;
        /// <summary>
        /// Insert column value into the destination table?
        /// </summary>
        [DisplayName("Use\n(Insert)")]
        public bool Insert
        {
            get { return _insert; }
            set
            {
                _insert = value;
                NotifyPropertyChanged("Insert");
            }
        }

        /// <summary>
        /// Update column value in the destination table?
        /// </summary>
        private bool _update;
        /// <summary>
        /// Update column value in the destination table?
        /// </summary>
        [DisplayName("Use\n(Update)")]
        public bool Update
        {
            get { return _update; }
            set
            {
                _update = value;
                NotifyPropertyChanged("Update");
            }
        }

        /// <summary>
        /// Use column as key for merge?
        /// </summary>
        private bool _key;
        /// <summary>
        /// Use column as key for merge?
        /// </summary>
        [DisplayName("Key")]
        public bool Key
        {
            get { return _key; }
            set
            {
                _key = value;
                NotifyPropertyChanged("Key");
            }
        }

        /// <summary>
        /// Input column name
        /// </summary>
        private string _inputColumnName;
        /// <summary>
        /// Input column name
        /// </summary>
        [DisplayName("Input Column"), ReadOnly(true)]
        public string InputColumnName
        {
            get { return _inputColumnName; }
            set
            {
                _inputColumnName = value;
                NotifyPropertyChanged("InputColumnName");
            }
        }

        /// <summary>
        /// Output column name
        /// </summary>
        private string _outputColumnName;
        /// <summary>
        /// Output column name
        /// </summary>
        [DisplayName("Output Column")]
        public string OutputColumnName
        {
            get { return _outputColumnName; }
            set
            {
                _outputColumnName = value;
                if (_sqlColumns != null)
                    SetOutputColumnDefinition();
                NotifyPropertyChanged("OutputColumnName");
            }
        }

        /// <summary>
        /// Default value if value is null
        /// </summary>
        private string _default;
        [DisplayName("Default")]
        public string Default
        {
            get { return _default; }
            set
            {
                if (value == null)
                    _default = "";
                else
                    _default = value;

                NotifyPropertyChanged("Default");
            }
        }

        /// <summary>
        /// function that return the value that has to be written to the destination table
        /// (no input column is used)
        /// </summary>
        private string _function;
        /// <summary>
        /// function that return the value that has to be written to the destination table
        /// (no input column is used)
        /// </summary>
        [DisplayName("Function")]
        public string Function
        {
            get { return _function; }
            set
            {
                if (value == null)
                    _function = "";
                else
                    _function = value;

                NotifyPropertyChanged("Function");
            }
        }

        /// <summary>
        /// Input datatype
        /// </summary>
        private string _dataTypeInput;
        /// <summary>
        /// Input datatype
        /// </summary>
        [DisplayName("DataType\n(Input)"), ReadOnly(true)]
        public string DataTypeInput
        {
            get { return _dataTypeInput; }
            set
            {
                _dataTypeInput = value;
                NotifyPropertyChanged("DataTypeInput");
            }
        }

        /// <summary>
        /// Output datatype
        /// </summary>
        private string _dataTypeOutput;
        /// <summary>
        /// Output datatype
        /// </summary>
        [DisplayName("DataType\n(Output)"), ReadOnly(true)]
        public string DataTypeOutput
        {
            get { return _dataTypeOutput; }
            set
            {
                _dataTypeOutput = value;
                NotifyPropertyChanged("DataTypeOutput");
            }
        }

        /// <summary>
        /// Output .NET datatype 
        /// </summary>
        private string _dataTypeOutputNet;
        [BrowsableAttribute(false), ReadOnly(true)]
        public string DataTypeOutputNet
        {
            get { return _dataTypeOutputNet; }
            set
            {
                _dataTypeOutputNet = value;
                NotifyPropertyChanged("DataTypeOutputNet");
            }
        }

        /// <summary>
        /// Is ouput column primary key?
        /// </summary>
        private bool _isOutputPrimaryKey;
        /// <summary>
        /// Is ouput column primary key?
        /// </summary>
        [DisplayName("PK\n(Output)"), ReadOnly(true)]
        public bool IsOutputPrimaryKey
        {
            get { return _isOutputPrimaryKey; }
            set
            {
                _isOutputPrimaryKey = value;
                NotifyPropertyChanged("IsOutputPrimaryKey");
            }
        }

        /// <summary>
        /// Does output column allow nulls?
        /// </summary>
        private bool _allowOutputDbNull;
        [DisplayName("Null\n(Output)"), ReadOnly(true)]
        public bool AllowOutputDbNull
        {
            get { return _allowOutputDbNull; }
            set
            {
                _allowOutputDbNull = value;
                NotifyPropertyChanged("AllowOutputDbNull");
            }
        }

        /// <summary>
        /// Is output column Identity?
        /// </summary>
        private bool _isOutputAutoId;
        [DisplayName("AutoID\n(Output)"), ReadOnly(true)]
        public bool IsOutputAutoId
        {
            get { return _isOutputAutoId; }
            set
            {
                _isOutputAutoId = value;
                NotifyPropertyChanged("IsOutputAutoId");
            }
        }

        /// <summary>
        /// Input column Id
        /// </summary>
        private int _inputColumnId;
        /// <summary>
        /// Input column Id
        /// </summary>
        [BrowsableAttribute(false), ReadOnly(true)]
        public int InputColumnId
        {
            get { return _inputColumnId; }
            set
            {
                _inputColumnId = value;
                NotifyPropertyChanged("InputColumnId");
            }
        }

        /// <summary>
        /// Has configuration an input column?
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasInput { get { return (InputColumnName != null && InputColumnName != ""); } }

        /// <summary>
        /// Has configuration an output column?
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasOutput { get { return (OutputColumnName != null && OutputColumnName != ""); } }

        /// <summary>
        /// Is input column used?
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public bool IsInputColumnUsed { get { return (Insert || Update || Key || IsScdValidFrom); } }

        /// <summary>
        /// Column name for bulk copy
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public string BulkColumnName
        {
            get
            {
                if (HasInput)
                    return InputColumnName;
                else
                    return OutputColumnName;
            }
        }

        /// <summary>
        /// Dattype for bulk copy
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public string BulkDataType
        {
            get
            {
                if (HasInput)
                    return DataTypeInput;
                else
                    return DataTypeOutput;
            }
        }

        /// <summary>
        /// Has function?
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasFunction
        {
            get
            {
                return (Function != null && Function != "");
            }
        }

        /// <summary>
        /// Has default value? (for null replacement)
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false)]
        public bool HasDefault
        {
            get
            {
                return (Default != null && Default != "");
            }
        }

        /// <summary>
        /// custom id (GUID) ofr lineage id mapping
        /// </summary>
        private string _customId;
        /// <summary>
        /// custom id (GUID) ofr lineage id mapping
        /// </summary>
        [BrowsableAttribute(false), ReadOnly(true)]
        public string CustomId
        {
            get { return _customId; }
            set { _customId = value; }
        }

        /// <summary>
        /// Is column used as an SCD (slowly changing dimension) column?
        /// </summary>
        [DisplayName("SCD Column")]
        public bool IsScdColumn { get; set; }

        /// <summary>
        /// SCD (slowly changing dimension) table name?
        /// </summary>
        [DisplayName("SCD Table")]
        public string ScdTable { get; set; }
        /// <summary>
        /// Is column used as an SCD (slowly changing dimension) validFrom column?
        /// </summary>
        [DisplayName("SCD ValidFrom")]
        public bool IsScdValidFrom { get; set; }

        /// <summary>
        /// Is column used for a SCD?
        /// </summary>
        [XmlIgnore, BrowsableAttribute(false), ReadOnly(true)]
        public bool HasScd
        {
            get { return IsScdColumn || IsScdValidFrom || !string.IsNullOrEmpty(ScdTable); }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        public ColumnConfig() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="insert">Insert column?</param>
        /// <param name="update">Update column</param>
        /// <param name="key">Use column as key?</param>
        /// <param name="inputColumnName">input column name</param>
        /// <param name="outputColumnName">output column nmme</param>
        /// <param name="defaultValue">default value for null values</param>
        /// <param name="function">function used to get value</param>
        /// <param name="dataTypeInput">input datatype</param>
        /// <param name="dataTypeOutput">output datatype</param>
        /// <param name="dataTypeOutputNet">output .NET datatype</param>
        /// <param name="isOutputPrimaryKey">Is output column primary key?</param>
        /// <param name="outputNullAllowed">Does output column allow nulls?</param>
        /// <param name="isOutputAutoId">Is output column identity?</param>
        /// <param name="inputColumnId">input column id</param>
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

            if (inputColumnId != null)
                _inputColumnId = (int) inputColumnId;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inputColumnName">input column name</param>
        /// <param name="dataTypeInput">input datatype</param>
        /// <param name="inputColumn">SSIS input column</param>
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
            LineageMapping.SetIdProperty(_customId, inputColumn.CustomPropertyCollection);
        }
        #endregion

        #region IXmlSerializable

        /// <summary>
        /// Gets XML schema (not used, so null is returned)
        /// </summary>
        /// <returns>XmlSchema (null)</returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        /// <summary>
        /// Reads a ColumnConfig from an XML reader
        /// </summary>
        /// <param name="reader">xml reader</param>
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

        /// <summary>
        /// Writes this ColumnConfig to an XML writer
        /// </summary>
        /// <param name="writer">xml writer</param>
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

        /// <summary>
        /// Sets list of description of sql columns
        /// </summary>
        /// <param name="sqlColumns">sql columns</param>
        public void SetSqlColumnDefinitions(SqlColumnList sqlColumns)
        {
            _sqlColumns = sqlColumns;
            SetOutputColumnDefinition();
        }

        /// <summary>
        /// Sets output column properties from sql column properties
        /// </summary>
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

        /// <summary>
        /// Removes output from column
        /// </summary>
        public void RemoveOutput()
        {
            _outputColumnName = "";
            _dataTypeOutput = "";
            _isOutputPrimaryKey = false;
            _isOutputAutoId = false;
            _allowOutputDbNull = false;
            //_sqlColumns = null;
        }

        /// <summary>
        /// Sets output column by using input column name + input- and output prefix
        /// </summary>
        /// <param name="inputPrefix">input column prefix</param>
        /// <param name="outputPrefix">output column prefix</param>
        public void AutoMap(string inputPrefix, string outputPrefix)
        {
            if (_sqlColumns != null)
                OutputColumnName = _sqlColumns.GetMatchingColumnname(InputColumnName, inputPrefix, outputPrefix);
        }

        /// <summary>
        /// Gets sql expression for column 
        /// (uses default value or function if available)
        /// </summary>
        /// <returns></returns>
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
