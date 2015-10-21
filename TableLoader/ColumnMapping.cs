using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    /// <summary>
    /// Mapping of input and output columns
    /// </summary>
    public class ColumnMapping
    {
        /// <summary>
        /// input column name
        /// </summary>
        public string InputColumnName { get; set; }
        /// <summary>
        /// output column name
        /// </summary>
        public string OutputColumnName { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inputColumnName">input column name</param>
        /// <param name="outputColumnName">output column name</param>
        public ColumnMapping(string inputColumnName, string outputColumnName)
        {
            InputColumnName = inputColumnName;
            OutputColumnName = outputColumnName;
        }
    }
}
