using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    public class SqlColumnList : Dictionary<string, SqlColumn>
    {       

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
    public class SqlColumn
    {
        public string DataType { get; set; }
        public string DataTypeNet { get; set; }
        public int ColumnSize { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }

        public bool IsPrimaryKey { get; set; }
        public bool IsAutoId { get; set; }
        public bool AllowsDbNull { get; set; }

        public string GetDataTypeDescription()
        {
            return SqlCreator.GetSQLServerDataType(DataType, ColumnSize.ToString(), NumericPrecision.ToString(), NumericScale.ToString());
        }
    }
}
