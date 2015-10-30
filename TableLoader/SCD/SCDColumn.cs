using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableLoader.SCD {
    /// <summary>
    /// SCD (slowly changing dimension) column
    /// </summary>
    public class SCDColumn {

        /// <summary>
        /// Column name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Column Datatype
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="dataType">Datatype</param>
        public SCDColumn(string columnName, string dataType)
        {
            ColumnName = columnName;
            DataType = dataType;
        }
    }
}
