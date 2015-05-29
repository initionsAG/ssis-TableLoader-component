using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    public class ColumnMapping
    {
        public string InputColumnName { get; set; }
        public string OutputColumnName { get; set; }

        public ColumnMapping(string inputColumnName, string outputColumnName)
        {
            InputColumnName = inputColumnName;
            OutputColumnName = outputColumnName;
        }
    }
}
