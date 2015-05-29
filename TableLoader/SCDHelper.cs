using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    class SCDHelper
    {      
        /// <summary>
        /// Fügt dem pre/postfix eckige Klammern hinzu, so dass die Spalte entsprechend geklammert ist
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        public static void AddBrackets(ref string prefix, ref string postfix)
        {
            prefix += "[";
            postfix += "]";
        }
        /// <summary>
        /// Fügt einer Spalte eckige Klammern hinzu
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string AddBrackets(string columnName)
        {
            return "[" + columnName + "]";
        } 

        /// <summary>
        /// Generiert eine mit Kommas getrennte Spaltenliste (SQL Format) 
        /// </summary>
        /// <param name="scdColumns"></param>
        /// <param name="prefix">wird der Spalte vorangestellt</param>
        /// <param name="postfix">wird an die Spalte angefügt</param>
        /// <param name="spaces">Anzahl Leerzeichen, die einer Zeile vorangestellt werden</param>
        /// <returns></returns>
        public static string GetSqlAttributeList(List<SCDCOlumn> scdColumns, string prefix, string postfix, int spaces)
        {
            string result = "";
            string newLine = spaces > 0 ? Environment.NewLine : "";

            AddBrackets(ref prefix, ref postfix);

            foreach (SCDCOlumn attribute in scdColumns)
            {

                result += (",").PadLeft(spaces + 1) + prefix + attribute.ColumnName + postfix + newLine;
            }

            return result;
        }


        //private static List<string> GetColumnList(List<SCDCOlumn> columnList)
        //{
        //    return GetColumnList(columnList, "");
        //}
        //private static List<string> GetColumnList(List<SCDCOlumn> columnList, string columnPrefix)
        //{
        //    List<string> result = new List<string>();

        //    foreach (SCDCOlumn scdColumn in columnList)
        //    {
        //        result.Add(columnPrefix + scdColumn.ColumnName);
        //    }

        //    return result;
        //}

        public static string GetSqlAttributeListDoubled(List<SCDCOlumn> scdColumns, string prefix1, string prefix2, string postfix1, string postfix2, int spaces)
        {
            return GetSqlColumnList(scdColumns, prefix1, prefix2, postfix1, postfix2, spaces, true, "");
        }

        public static string GetSqlBkList(List<SCDCOlumn> scdColumns, string prefix, int spaces)
        {
            return GetSqlColumnList(scdColumns, prefix, "", "", "", spaces, false, "");
        }

        public static string GetSqlBkList(List<SCDCOlumn> scdColumns, string columnPrefix, string prefix, int spaces)
        {
            return GetSqlColumnList(scdColumns, prefix, "", "", "", spaces, false, columnPrefix);
        }
       
        /// <summary>
        ///     Liefert eine mit Kommas getrennte Liste von SCD Columns zurück. (im SQL Format)
        /// </summary>
        /// <param name="scdColumnList"></param>
        /// <param name="prefix1">Prefix für Spalte1</param>
        /// <param name="prefix2">Prefix für Spalte2</param>
        /// <param name="postfix1">Postfix für Spalte1</param>
        /// <param name="postfix2">Postfix für Spalte2</param>
        /// <param name="spaces">Anzahl Leerzeichen, die einer Zeile vorangestellt werden</param>
        /// <param name="generate2ndColumn">Soll eine zweite Spalte erzeugt werden?</param>
        /// <returns></returns>
        public static string GetSqlColumnList(List<SCDCOlumn> columnList, string prefix1, string prefix2, string postfix1, string postfix2, int spaces, bool generate2ndColumn, string columnPrefix)
        {
            string result = "";
            string newLine = spaces > 0 ? Environment.NewLine : "";

            AddBrackets(ref prefix1, ref postfix1);
            AddBrackets(ref prefix2, ref postfix2);

            foreach (SCDCOlumn scdColumn in columnList)
            {
                result += (",").PadLeft(spaces + 1) + prefix1 + columnPrefix + scdColumn.ColumnName + postfix1 + newLine;
                if (generate2ndColumn) result += (",").PadLeft(spaces + 1) + prefix2 + columnPrefix + scdColumn.ColumnName + postfix2 + newLine;
            }

            return result;
        }

        /// <summary>
        /// Liefert den "On"-Teil eines Merge Befehls (SQL Format)
        /// </summary>
        /// <param name="scdColumns">Liste von SCD Columns (BKs)</param>
        /// <returns></returns>
        public static string GetBkOnList(List<SCDCOlumn> scdColumns, string columnPrefix)
        {
            string result = "";


            foreach (SCDCOlumn column in scdColumns)
            {
                if (result != "") result += " AND ";
                result += "A.[" + columnPrefix + column.ColumnName + "] = MR.[" + columnPrefix + column.ColumnName + "]";
            }

            return result;
        }

        /// <summary>
        /// Liefert die ValidFrom Spalte (SQL Format) 
        /// </summary>
        /// <param name="tableName">sofern angegeben, ist der Tabellenname Teil des Spaltennamens</param>
        /// <param name="scdColumn">die SCD Column, die die ValidFrom Spalte enthält</param>
        /// <param name="prefix">Prefix für die Spalte</param>
        /// <param name="postfix">Postfix für die Spalte</param>
        /// <param name="spaces">Anzahl Leerzeichen, die einer Zeile vorangestellt werden</param>
        /// <returns></returns>
        public static string GetSqlValidFrom(string tableName, SCDCOlumn scdColumn, string prefix, string postfix, int spaces)
        {
            string result = "";
            string columnName = tableName == "" ? scdColumn.ColumnName : tableName + "_" + scdColumn.ColumnName;
 
            AddBrackets(ref prefix, ref postfix);

            result = prefix + columnName + postfix;
            result = result.PadLeft(result.Length + spaces);


            return result;
        }

        /// <summary>
        /// Liefert den Vergleich der "_QUELLE"-Spalte und der "DWH_VOR_UPDATE" Spalte für die Where Clause
        /// </summary>
        /// <param name="scdColumns"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetSqlAttributeWhere(List<SCDCOlumn> scdColumns, string prefix)
        {
            string result = "";

            string postfix = "";

            AddBrackets(ref prefix, ref postfix);

            foreach (SCDCOlumn attribute in scdColumns)
            {
                if (result != "") result += " OR " + Environment.NewLine;
                result += prefix + attribute.ColumnName + SCD.POSTFIX_COLUMN_QUELLE + postfix + " <> " + prefix + attribute.ColumnName + SCD.POSTFIX_COLUMN_DWH_VOR_UPDATE + postfix;
                result += " OR (NOT " + prefix + attribute.ColumnName + SCD.POSTFIX_COLUMN_QUELLE + postfix + " IS NULL"
                          + " AND " + prefix + attribute.ColumnName + SCD.POSTFIX_COLUMN_DWH_VOR_UPDATE + postfix + " IS NULL)";
            }

            return result;
        }

        public static string GetSqlAttributeListWithDataType(List<SCDCOlumn> scdColumnList, string postfix1, string postfix2, int spaces)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix1, postfix2, spaces, true);
        }
        public static string GetSqlBkListWithDataType(List<SCDCOlumn> scdColumnList, string postfix, int spaces)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix, "", spaces, false, "", "");
        }

        public static string GetSqlBkListWithDataType(List<SCDCOlumn> scdColumnList, string prefixColumnName, string postfix, int spaces)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix, "", spaces, false, "", prefixColumnName);
        }

        public static string GetSqlBkListWithDataType(List<SCDCOlumn> scdColumnList, string postfix, int spaces, string afterDataType)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix, "", spaces, false, afterDataType, "");
        }
        public static string GetSqlColumnListWithDataType(List<SCDCOlumn> scdColumnList, string postfix1, string postfix2, int spaces, bool generate2ndColumn)
        {
            return GetSqlColumnListWithDataType(scdColumnList, postfix1, postfix2, spaces, generate2ndColumn, "", "");
        }
        /// <summary>
        /// Liefert eine Liste von SCD Columns mit SQL Datentypen zurück. (im SQL Format)
        /// </summary>
        /// <param name="scdColumnList">SCD Column Liste</param>
        /// <param name="postfix1">Postfix für Spalte1</param>
        /// <param name="postfix2">Postfix für Spalte2</param>
        /// <param name="spaces">Anzahl Leerzeichen, die einer Zeile vorangestellt werden</param>
        /// <param name="generate2ndColumn">Soll eine zweite Spalte erzeugt werden?</param>
        /// <param name="afterDataType">string, der nach dem Datatype eingefügt wird, z.B. "NULL"</param>
        /// <returns></returns>
        public static string GetSqlColumnListWithDataType(List<SCDCOlumn> scdColumnList, string postfix1, string postfix2, int spaces, bool generate2ndColumn, string afterDataType, string prefixColumnName)
        {
            string result = "";
            string newLine = spaces > 0 ? Environment.NewLine : "";
            string prefix1 = "";
            string prefix2 = "";

            SCDHelper.AddBrackets(ref prefix1, ref postfix1);
            SCDHelper.AddBrackets(ref prefix2, ref postfix2);

            foreach (SCDCOlumn column in scdColumnList)
            {                
                result += (",").PadLeft(spaces + 1) + prefix1 + prefixColumnName + column.ColumnName + postfix1 + " " + column.DataType + " " + afterDataType + newLine;
                if (generate2ndColumn) result += (",").PadLeft(spaces + 1) + prefix2 + prefixColumnName + column.ColumnName + postfix2 + " " + column.DataType + " " + afterDataType + newLine;
            }

            return result;
        }

        /// <summary>
        /// Liefert alle ValidFrom Columns(Komma separiert, SQL FORMAT)
        /// </summary>
        /// <param name="scdList">Liste aller SCDs</param>
        /// <param name="spaces">Anzahl Leerzeichen, die einer Zeile vorangestellt werden</param>
        /// <returns></returns>
        public static string GetSqlValidFromWithDataType(Dictionary<string, SCD> scdList, int spaces)
        {
            string newLine = spaces > 0 ? Environment.NewLine : "";
            string result = "";

            foreach (SCD scd in scdList.Values)
            {
                string columnName = SCDHelper.AddBrackets(scd.TableName + "_" + scd.ValidFrom.ColumnName);

                result += (",").PadLeft(spaces + 1) + columnName + " " + scd.ValidFrom.DataType + newLine;
            }

            return result;
        }

        public static string GetSqlValidFrom(Dictionary<string, SCD> scdList, bool addTableNamePrefix, string prefix, int spaces)
        {
            string newLine = spaces > 0 ? Environment.NewLine : "";
            string result = "";

            foreach (SCD scd in scdList.Values)
            {
                string columnName = addTableNamePrefix ? scd.TableName + "_" + scd.ValidFrom.ColumnName : scd.ValidFrom.ColumnName;
                columnName = prefix + SCDHelper.AddBrackets(columnName);

                result += (",").PadLeft(spaces + 1) + columnName + newLine;
            }

            return result;
        }

        
    }
}
