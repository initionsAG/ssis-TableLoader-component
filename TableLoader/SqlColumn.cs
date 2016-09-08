using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    /// <summary>
    /// Dictionary
    /// Key: sql column name
    /// Value: sql column properties
    /// </summary>
    public class SqlColumnList : Dictionary<string, SqlColumn>
    {       
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="columnName">sql column name</param>
        /// <param name="dataType">sql datatype</param>
        /// <param name="dataTypeNet">.NET datatype</param>
        /// <param name="columnSize">column size</param>
        /// <param name="numericPrecision">cumeric precision</param>
        /// <param name="numericScale">numeric scale</param>
        /// <param name="isPrimaryKey">Is primary key?</param>
        /// <param name="isAutoId">Is identity?</param>
        /// <param name="allowsDbNUll">Are null values allowed</param>
        public void Add(string columnName, string dataType, string dataTypeNet, int columnSize, int numericPrecision, int numericScale, 
                        bool isPrimaryKey, bool isAutoId, bool allowsDbNUll) 
        { 
            this.Add(columnName, 
                     new SqlColumn() 
                     {
                        DataType = dataType,
                        DataTypeNet = dataTypeNet,
                        ColumnSize = columnSize,
                        NumericPrecision = numericPrecision,
                        NumericScale = numericScale,
                        IsPrimaryKey = isPrimaryKey,
                        IsAutoId = isAutoId,
                        AllowsDbNull = allowsDbNUll
                     });        
        }

        /// <summary>
        /// Searches for an sql column by comparing sql column name with input column name.
        /// Also prefixes are used to find matches. 
        /// </summary>
        /// <param name="inputColName">SSIS input column name</param>
        /// <param name="inputPrefix">prefix for SSIS imput column name</param>
        /// <param name="outputPrefix">prefix for sql column name</param>
        /// <returns>sql column name (empty string if no match)</returns>
        public string GetMatchingColumnname(string inputColName, string inputPrefix, string outputPrefix)
        {           
            if (this.ContainsKey(inputColName)) return inputColName;
            if (this.ContainsKey(inputColName.ToUpper())) return inputColName.ToUpper();
            if (this.ContainsKey(inputColName.ToLower())) return inputColName.ToLower();

            inputPrefix = inputPrefix.ToUpper();
            outputPrefix = outputPrefix.ToUpper();

            foreach (string outputColName in this.Keys)
            {
                string compareValue = outputColName.ToUpper();
                inputColName = inputColName.ToUpper();

                if (compareValue.StartsWith(outputPrefix)) compareValue = compareValue.Remove(0, outputPrefix.Length);
                if (inputColName.StartsWith(inputPrefix)) inputColName = inputColName.Remove(0, inputPrefix.Length);

                if (inputColName == compareValue) return outputColName;
            }

            return "";
        }

    }

    /// <summary>
    /// Sql Column properties
    /// </summary>
    public class SqlColumn
    {
        /// <summary>
        /// sql datatype
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// .NET datatype
        /// </summary>
        public string DataTypeNet { get; set; }
        /// <summary>
        /// column size
        /// </summary>
        public int ColumnSize { get; set; }
        /// <summary>
        /// numeric precision
        /// </summary>
        public int NumericPrecision { get; set; }
        /// <summary>
        /// numeric scale
        /// </summary>
        public int NumericScale { get; set; }

        /// <summary>
        /// Is primary key?
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// Is identity?
        /// </summary>
        public bool IsAutoId { get; set; }
        /// <summary>
        /// Are null values allowed?
        /// </summary>
        public bool AllowsDbNull { get; set; }

        /// <summary>
        /// Get datatype description
        /// </summary>
        /// <returns>datatype description</returns>
        public string GetDataTypeDescription()
        {
            return SqlCreator.GetSQLServerDataType(DataType, ColumnSize.ToString(), NumericPrecision.ToString(), NumericScale.ToString());
        }
    }
}
