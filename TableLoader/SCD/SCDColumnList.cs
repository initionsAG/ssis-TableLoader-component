using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableLoader.SCD {
    /// <summary>
    /// SCD column list
    /// </summary>
    public class ScdColumnList {
        /// <summary>
        /// SCD column list
        /// </summary>
        public List<SCDColumn> ScdColumns { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public ScdColumnList()
        {
            ScdColumns = new List<SCDColumn>();
        }

        /// <summary>
        /// Adds all columns from a list of SCD column
        /// (exception: SCD column list already contains the column)
        /// </summary>
        /// <param name="scdList">SCD column list</param>
        public void AddList(List<SCDColumn> scdList)
        {
            foreach (SCDColumn newColumn in scdList)
            {
                if (!ContainsSCDColumn(newColumn))
                    ScdColumns.Add(newColumn);
            }
        }

        /// <summary>
        /// Does the SCD column list contain the specified column?
        /// </summary>
        /// <param name="scdColumn">SCD column</param>
        /// <returns>Does the SCD column list contain the specified column?</returns>
        private bool ContainsSCDColumn(SCDColumn scdColumn)
        {
            foreach (SCDColumn col in ScdColumns)
            {
                if (col.ColumnName == scdColumn.ColumnName)
                    return true;
            }

            return false;
        }
    }
}
