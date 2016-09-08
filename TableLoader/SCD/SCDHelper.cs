using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader.SCD {
    /// <summary>
    /// Helper methods for slowly changing dimensions
    /// </summary>
    class SCDHelper {
        /// <summary>
        /// Adds brackets to pre/postfix, so that column will be enclosed by brackets
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="postfix">postfix</param>
        public static void AddBrackets(ref string prefix, ref string postfix)
        {
            prefix += "[";
            postfix += "]";
        }
        /// <summary>
        /// Adds brackets to a column
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>column name with brackets</returns>
        public static string AddBrackets(string columnName)
        {
            return "[" + columnName + "]";
        }

        /// <summary>
        /// Generates comma separated columns list (sql formated)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefix">prefix for a column</param>
        /// <param name="postfix">postfix for a column</param>
        /// <param name="spaces">ANumber of space at the beginnung of a row</param>
        /// <returns>comma separated columns list (sql formated)</returns>
        public static string GetSqlAttributeList(List<SCDColumn> scdColumns, string prefix, string postfix, int spaces)
        {
            string result = "";
            string newLine = spaces > 0 ? Environment.NewLine : "";

            AddBrackets(ref prefix, ref postfix);

            foreach (SCDColumn attribute in scdColumns)
            {

                result += (",").PadLeft(spaces + 1) + prefix + attribute.ColumnName + postfix + newLine;
            }

            return result;
        }

        /// <summary>
        /// Gets sql attribute column list twice (differs in pre/posfix)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefix1">Prefix first column</param>
        /// <param name="prefix2">Prefix second column</param>
        /// <param name="postfix1">Postfix first column</param>
        /// <param name="postfix2">Postfix second column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>sql attribute column list</returns>
        public static string GetSqlAttributeListDoubled(List<SCDColumn> scdColumns, string prefix1, string prefix2, string postfix1, string postfix2, int spaces)
        {
            return GetSqlColumnList(scdColumns, prefix1, prefix2, postfix1, postfix2, spaces, true, "");
        }

        /// <summary>
        /// Gets sql BK column list
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefix">prefix for a column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>sql BK column list</returns>
        public static string GetSqlBkList(List<SCDColumn> scdColumns, string prefix, int spaces)
        {
            return GetSqlColumnList(scdColumns, prefix, "", "", "", spaces, false, "");
        }

        /// <summary>
        /// Gets sql BK column list
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="columnPrefix">Column name prefix</param>
        /// <param name="prefix">prefix for a column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>sql BK column list</returns>
        public static string GetSqlBkList(List<SCDColumn> scdColumns, string columnPrefix, string prefix, int spaces)
        {
            return GetSqlColumnList(scdColumns, prefix, "", "", "", spaces, false, columnPrefix);
        }

        /// <summary>
        /// Generates comma separated SCD columns list (sql formated)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefix1">Prefix first column</param>
        /// <param name="prefix2">Prefix second column</param>
        /// <param name="postfix1">Postfix first column</param>
        /// <param name="postfix2">Postfix second column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <param name="generate2ndColumn">Generate 2nd column?</param>
        /// <param name="columnPrefix">Column name prefix</param>
        /// <returns>comma separated SCD columns list</returns>
        public static string GetSqlColumnList(List<SCDColumn> columnList, string prefix1, string prefix2, string postfix1, string postfix2, int spaces, bool generate2ndColumn, string columnPrefix)
        {
            string result = "";
            string newLine = spaces > 0 ? Environment.NewLine : "";

            AddBrackets(ref prefix1, ref postfix1);
            AddBrackets(ref prefix2, ref postfix2);

            foreach (SCDColumn scdColumn in columnList)
            {
                result += (",").PadLeft(spaces + 1) + prefix1 + columnPrefix + scdColumn.ColumnName + postfix1 + newLine;
                if (generate2ndColumn)
                    result += (",").PadLeft(spaces + 1) + prefix2 + columnPrefix + scdColumn.ColumnName + postfix2 + newLine;
            }

            return result;
        }

        /// <summary>
        /// Gets "On" part of a merge statement
        /// </summary>
        /// <param name="scdColumns">SCD BK column list</param>
        /// <returns>"On" part of a merge statement</returns>
        public static string GetBkOnList(List<SCDColumn> scdColumns, string columnPrefix)
        {
            string result = "";


            foreach (SCDColumn column in scdColumns)
            {
                if (result != "")
                    result += " AND ";
                result += "A.[" + columnPrefix + column.ColumnName + "] = MR.[" + columnPrefix + column.ColumnName + "]";
            }

            return result;
        }

        /// <summary>
        /// Gets ValidFrom column (sql formatted) 
        /// </summary>
        /// <param name="tableName">If not empty table name is part of column name</param>
        /// <param name="scdColumn">SCD ValidFrom Column</param>
        /// <param name="prefix">column prefix</param>
        /// <param name="postfix">column postfix</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>ValidFrom column</returns>
        public static string GetSqlValidFrom(string tableName, SCDColumn scdColumn, string prefix, string postfix, int spaces)
        {
            string result = "";
            string columnName = tableName == "" ? scdColumn.ColumnName : tableName + "_" + scdColumn.ColumnName;

            AddBrackets(ref prefix, ref postfix);

            result = prefix + columnName + postfix;
            result = result.PadLeft(result.Length + spaces);

            return result;
        }

        /// <summary>
        /// Gets compare statement of "_QUELLE" column and "DWH_VOR_UPDATE" column for where clause
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefix">column prefix</param>
        /// <returns>compare statement of "_QUELLE" column and "DWH_VOR_UPDATE" column for where clause</returns>
        public static string GetSqlAttributeWhere(List<SCDColumn> scdColumns, string prefix)
        {
            string result = "";

            string postfix = "";

            AddBrackets(ref prefix, ref postfix);

            foreach (SCDColumn attribute in scdColumns)
            {
                if (result != "")
                    result += " OR " + Environment.NewLine;
                result += prefix + attribute.ColumnName + SCDConfiguration.POSTFIX_COLUMN_QUELLE + postfix + " <> " + prefix + attribute.ColumnName + SCDConfiguration.POSTFIX_COLUMN_DWH_VOR_UPDATE + postfix;
                result += " OR (NOT " + prefix + attribute.ColumnName + SCDConfiguration.POSTFIX_COLUMN_QUELLE + postfix + " IS NULL"
                          + " AND " + prefix + attribute.ColumnName + SCDConfiguration.POSTFIX_COLUMN_DWH_VOR_UPDATE + postfix + " IS NULL)";
            }

            return result;
        }

        /// <summary>
        /// Gets list of SCD columns with datatypes (sql formatted)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="postfix1">postfix first column</param>
        /// <param name="postfix2">postfix second column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>sql attribute list with datatypes</returns>
        public static string GetSqlAttributeListWithDataType(List<SCDColumn> scdColumnList, string postfix1, string postfix2, int spaces)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix1, postfix2, spaces, true);
        }


        /// <summary>
        /// Gets list of SCD columns with datatypes (sql formatted)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="postfix">postfix first column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>list of SCD columns with datatypes (sql formatted)</returns>
        public static string GetSqlBkListWithDataType(List<SCDColumn> scdColumnList, string postfix, int spaces)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix, "", spaces, false, "", "");
        }

        /// <summary>
        /// Gets list of SCD columns with datatypes (sql formatted)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefixColumnName">Prefix for columnname</param>
        /// <param name="postfix">postfix first column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>list of SCD columns with datatypes (sql formatted)</returns>
        public static string GetSqlBkListWithDataType(List<SCDColumn> scdColumnList, string prefixColumnName, string postfix, int spaces)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix, "", spaces, false, "", prefixColumnName);
        }

        /// <summary>
        /// Gets list of SCD columns with datatypes (sql formatted)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="prefixColumnName">Prefix for columnname</param>
        /// <param name="postfix">postfix first column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <param name="afterDataType">Text that is inserted after datatype (i.e. NULL)</param>
        /// <returns>list of SCD columns with datatypes (sql formatted)</returns>
        public static string GetSqlBkListWithDataType(List<SCDColumn> scdColumnList, string postfix, int spaces, string afterDataType)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix, "", spaces, false, afterDataType, "");
        }

        /// <summary>
        /// Gets list of SCD columns with datatypes (sql formatted)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="postfix1">postfix first column</param>
        /// <param name="postfix2">postfix second column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <param name="generate2ndColumn">Generate 2nd column?</param>
        /// <returns>list of SCD columns with datatypes (sql formatted)</returns>
        public static string GetSqlColumnListWithDataType(List<SCDColumn> scdColumnList, string postfix1, string postfix2, int spaces, bool generate2ndColumn)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix1, postfix2, spaces, generate2ndColumn, "", "");
        }

        /// <summary>
        /// Gets list of SCD columns with datatypes (sql formatted)
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="postfix1">postfix first column</param>
        /// <param name="postfix2">postfix second column</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <param name="generate2ndColumn">Generate 2nd column?</param>
        /// <param name="afterDataType">Text that is inserted after datatype (i.e. NULL)</param>
        /// <param name="prefixColumnName">Prefix for columnname</param>
        /// <returns>list of SCD columns with datatypes (sql formatted)</returns>
        public static string GetSqlColumnListWithDataType(List<SCDColumn> scdColumnList, string postfix1, string postfix2, int spaces, bool generate2ndColumn, string afterDataType, string prefixColumnName)
        {
            string result = "";
            string newLine = spaces > 0 ? Environment.NewLine : "";
            string prefix1 = "";
            string prefix2 = "";

            SCDHelper.AddBrackets(ref prefix1, ref postfix1);
            SCDHelper.AddBrackets(ref prefix2, ref postfix2);

            foreach (SCDColumn column in scdColumnList)
            {
                result += (",").PadLeft(spaces + 1) + prefix1 + prefixColumnName + column.ColumnName + postfix1 + " " + column.DataType + " " + afterDataType + newLine;
                if (generate2ndColumn)
                    result += (",").PadLeft(spaces + 1) + prefix2 + prefixColumnName + column.ColumnName + postfix2 + " " + column.DataType + " " + afterDataType + newLine;
            }

            return result;
        }

        /// <summary>
        /// Gets ValidFrom column (sql formatted) 
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>ValidFrom column</returns>
        public static string GetSqlValidFromWithDataType(Dictionary<string, SCDConfiguration> scdList, int spaces)
        {
            string newLine = spaces > 0 ? Environment.NewLine : "";
            string result = "";

            foreach (SCDConfiguration scd in scdList.Values)
            {
                string columnName = SCDHelper.AddBrackets(scd.TableName + "_" + scd.ValidFrom.ColumnName);

                result += (",").PadLeft(spaces + 1) + columnName + " " + scd.ValidFrom.DataType + newLine;
            }

            return result;
        }

        /// <summary>
        /// Gets ValidFrom column (sql formatted) 
        /// </summary>
        /// <param name="scdColumns">SCD column list</param>
        /// <param name="addTableNamePrefix">Add table name prefix?</param>
        /// <param name="prefix">Column prefix</param>
        /// <param name="spaces">Number of space at the beginnung of a row</param>
        /// <returns>ValidFrom column</returns>
        public static string GetSqlValidFrom(Dictionary<string, SCDConfiguration> scdList, bool addTableNamePrefix, string prefix, int spaces)
        {
            string newLine = spaces > 0 ? Environment.NewLine : "";
            string result = "";

            foreach (SCDConfiguration scd in scdList.Values)
            {
                string columnName = addTableNamePrefix ? scd.TableName + "_" + scd.ValidFrom.ColumnName : scd.ValidFrom.ColumnName;
                columnName = prefix + SCDHelper.AddBrackets(columnName);

                result += (",").PadLeft(spaces + 1) + columnName + newLine;
            }

            return result;
        }


    }
}
