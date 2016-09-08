using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Windows.Forms;
using System.ComponentModel;
using TableLoader.SCD;

namespace TableLoader {
    /// <summary>
    /// Holds static methods for creating sql statements
    /// </summary>
    public static class SqlCreator {

        #region DB Commands

        /// <summary>
        /// Creates pseudo SQL merge command
        /// (merge statement is not available for SQL Server < 2008)
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns></returns>
        public static string GetSqlMerge2005(IsagCustomProperties properties, string tempTableName)
        {
            return GetSqlMerge2005(properties, tempTableName, false);
        }


        /// <summary>
        /// Creates pseudo SQL merge command
        /// (merge statement is not available for SQL Server < 2008)
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="overrideCustomMergeCommand">If true, only the updated_id tablename is replaced (used for preview) </param>
        /// <returns>pseudo sql merge command</returns>
        public static string GetSqlMerge2005(IsagCustomProperties properties, string tempTableName, bool overrideCustomMergeCommand)
        {
            if (properties.UseCustomMergeCommand && !overrideCustomMergeCommand)
            {
                return properties.CustomMergeCommand
                    .Replace(Constants.TEMP_UPD_TABLE_PLACEHOLDER, "[#updated_ids_" + Guid.NewGuid().ToString() + "]");
            }
            else
            {
                string varUpdated_ids;
                if (properties.UseCustomMergeCommand)
                    varUpdated_ids = Constants.TEMP_UPD_TABLE_PLACEHOLDER;
                else
                    varUpdated_ids = "[#updated_ids_" + Guid.NewGuid().ToString() + "]";

                string destTable = properties.DestinationTable;
                string declare = "";

                //update
                string sqlUpdate = "";
                string sqlUpdateOutput = "";
                string sqlUpdateWhere = "";

                //insert
                string sqlInsert = "";
                string sqlInsertValues = "";
                string sqlInsertLeftJoinOn = "";
                string sqlInsertLeftJoinWhere = "";
                string sqlInsertValuesDefinition = "";



                foreach (ColumnConfig config in properties.ColumnConfigList)
                {
                    //Keys
                    if (config.Key)
                    {
                        //Declare
                        if (declare != "")
                            declare += ", ";
                        declare += config.OutputColumnName + " " + config.DataTypeOutput;

                        //Output
                        if (sqlUpdateOutput != "")
                            sqlUpdateOutput += ", ";
                        sqlUpdateOutput += "inserted." + Brackets(config.OutputColumnName);

                        //where (Update)
                        if (sqlUpdateWhere != "")
                            sqlUpdateWhere += " AND ";
                        sqlUpdateWhere += "src." + Brackets(config.BulkColumnName) + " = " + "dest." + Brackets(config.OutputColumnName);

                        //where (insert: Left Join On)
                        if (sqlInsertLeftJoinOn != "")
                            sqlInsertLeftJoinOn += " AND ";
                        sqlInsertLeftJoinOn += "upd." + Brackets(config.OutputColumnName) + " = " + "src." + Brackets(config.BulkColumnName);

                        //where (insert: Left Join WHere)
                        if (sqlInsertLeftJoinWhere != "")
                            sqlInsertLeftJoinWhere += " AND ";
                        sqlInsertLeftJoinWhere += "upd." + Brackets(config.OutputColumnName) + " IS NULL";
                    }

                    //Update
                    if (config.Update)
                    {
                        if (sqlUpdate != "")
                            sqlUpdate += ", ";


                        sqlUpdate += Brackets(config.OutputColumnName) + " = ";

                        if (config.HasFunction)
                            sqlUpdate += config.Function + " ";
                        else
                            sqlUpdate += "src." + Brackets(config.BulkColumnName) + " ";
                    }

                    //Insert
                    if (config.Insert)
                    {
                        if (sqlInsertValuesDefinition != "")
                            sqlInsertValuesDefinition += ", ";
                        if (sqlInsertValues != "")
                            sqlInsertValues += ", ";

                        sqlInsertValuesDefinition += Brackets(config.OutputColumnName);

                        if (config.HasDefault) //Default Values for Insert: isnull(<columnname>, <defaultValue>)
                        {
                            sqlInsertValues += " isnull(cast(src." + Brackets(config.BulkColumnName) + " as " + config.DataTypeOutput + ") ," + config.Default + ")";
                        }
                        else if (config.HasFunction)
                        {
                            sqlInsertValues += config.Function + " ";
                        }
                        else
                        { //no Default Value or function
                            sqlInsertValues += "src." + Brackets(config.BulkColumnName) + " ";
                        }
                    }
                }


                if (sqlUpdate != "")
                    declare = "CREATE TABLE " + varUpdated_ids + "  (" + declare + ");" + Environment.NewLine;
                else
                    declare = "";

                //update
                if (sqlUpdate != "")
                {
                    string sqlUpdateTmp = "UPDATE " + destTable + Environment.NewLine;
                    if (!properties.DisableTablock)
                        sqlUpdateTmp += "WITH (Tablockx)" + Environment.NewLine;
                    sqlUpdateTmp += "SET " + sqlUpdate + Environment.NewLine +
                                    "OUTPUT " + sqlUpdateOutput + " INTO " + varUpdated_ids + Environment.NewLine +
                                    "FROM " + destTable + " dest, " + tempTableName + " src" + Environment.NewLine +
                                    "WHERE " + sqlUpdateWhere + ";" + Environment.NewLine;
                    sqlUpdate = sqlUpdateTmp;
                }
                //insert
                sqlInsert = "INSERT INTO " + destTable;
                if (!properties.DisableTablock)
                    sqlInsert += " WITH (Tablockx)";
                sqlInsert += Environment.NewLine +
                             "(" + sqlInsertValuesDefinition + ")" + Environment.NewLine +
                             "SELECT " + sqlInsertValues + " FROM " + tempTableName + " src" + Environment.NewLine;
                if (sqlUpdate != "")
                    sqlInsert +=
                        "LEFT JOIN " + varUpdated_ids + " upd" + Environment.NewLine +
                        "ON " + sqlInsertLeftJoinOn + Environment.NewLine +
                        "WHERE " + sqlInsertLeftJoinWhere + Environment.NewLine;

                sqlInsert += ";";

                return ReplacePlaceHolderInputColumn(declare + sqlUpdate + sqlInsert, "src.");
            }
        }

        /// <summary>
        /// Creates SQL merge command (only SQL Server 2008 and above)
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns>sql merge command</returns>
        public static string GetSqlMerge(IsagCustomProperties properties, string tempTableName)
        {
            return GetSqlMerge(properties, tempTableName, false);
        }

        /// <summary>
        /// Creates SQL merge command (only SQL Server 2008 and above)
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="overrideCustomMergeCommand">If true, only custom merge template is returned (used for preview) </param>
        /// <returns>sql merge command</returns>
        public static string GetSqlMerge(IsagCustomProperties properties, string tempTableName, bool overrideCustomMergeCommand)
        {
            if (properties.UseCustomMergeCommand && !overrideCustomMergeCommand)
            {
                return properties.CustomMergeCommand;
            }
            else
            {
                string destTable = properties.DestinationTable;
                string tempTable = tempTableName;

                string result = "merge ";
                string sqlInto = "into " + Brackets(destTable); // +" with (tablockx) as dest ";
                if (!properties.DisableTablock)
                    sqlInto += " with (tablockx)";
                sqlInto += " as dest ";
                string sqlUsing = "using " + Brackets(tempTable) + " as src ";
                string sqlOn = "";
                string sqlUpdate = "";
                string sqlInsert = "";
                string sqlValues = "";

                foreach (ColumnConfig config in properties.ColumnConfigList)
                {
                    //Update
                    if (config.Update)
                    {
                        if (sqlUpdate != "")
                            sqlUpdate += ", ";
                        sqlUpdate += "dest." + Brackets(config.OutputColumnName) + " = ";
                        if (config.HasFunction)
                            sqlUpdate += config.Function + " ";
                        else
                            sqlUpdate += "src." + Brackets(config.BulkColumnName) + " ";

                    }

                    //Insert
                    if (config.Insert)
                    {
                        if (sqlInsert != "")
                            sqlInsert += ", ";
                        if (sqlValues != "")
                            sqlValues += ", ";

                        sqlInsert += Brackets(config.OutputColumnName);

                        //Default Values for Insert: isnull(<columnname>, <defaultValue>)
                        sqlValues += config.GetColumnExpression();
                    }

                    //Join
                    if (config.Key)
                    {
                        if (sqlOn != "")
                            sqlOn += " and ";
                        sqlOn += config.GetColumnExpression() + " = " + "dest." + Brackets(config.OutputColumnName) + " "; //"src." + Brackets(config.BulkColumnName)
                    }
                }

                sqlOn = "ON " + sqlOn;
                if (sqlUpdate != "")
                    sqlUpdate = "WHEN MATCHED THEN UPDATE SET " + sqlUpdate;
                if (sqlInsert != "")
                {
                    sqlInsert = "WHEN NOT MATCHED BY TARGET THEN INSERT (" + sqlInsert + ") ";
                    sqlValues = "VALUES (" + sqlValues + ")";
                }

                sqlValues += ";";

                result += sqlInto + Environment.NewLine + sqlUsing + Environment.NewLine + sqlOn + Environment.NewLine +
                          sqlUpdate + Environment.NewLine + sqlInsert + Environment.NewLine + sqlValues;


                result = ReplacePlaceHolderInputColumn(result, "src.");

                if (properties.HasScd)
                {
                    SCDList scd = new SCDList(properties.ColumnConfigList, properties.DestinationTable);
                    result = scd.InsertIntoMergeStatement(result, properties, tempTableName.Replace("#", "SCD_"));
                }

                return result;
            }
        }

        /// <summary>
        /// Creates SQL Update command
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns>sql update command</returns>
        public static string GetSqlUpdate(IsagCustomProperties properties, string tempTableName)
        {
            string destTable = Brackets(properties.DestinationTable);
            string tempTable = Brackets(tempTableName);

            string result = "update " + destTable;
            if (!properties.DisableTablock)
                result += " WITH (tablockx)";
            result += Environment.NewLine + "set ";
            string sqlSet = "";
            string sqlfrom = "from " + tempTable;
            string sqlWhere = "";

            foreach (ColumnConfig config in properties.ColumnConfigList)
            {

                if (config.Update)
                {
                    if (sqlSet != "")
                        sqlSet += "," + Environment.NewLine + "      ";

                    sqlSet += destTable + "." + Brackets(config.OutputColumnName) + "=";
                    if (config.HasFunction)
                        sqlSet += config.Function + " ";
                    else
                        sqlSet += tempTable + "." + Brackets(config.BulkColumnName);
                }
                //create where clause
                if (config.Key)
                {
                    if (sqlWhere != "")
                        sqlWhere += " and ";
                    sqlWhere += tempTable + "." + Brackets(config.BulkColumnName) + "=" + destTable + "." + Brackets(config.OutputColumnName);
                }
            }

            result += sqlSet + Environment.NewLine + sqlfrom + Environment.NewLine + "where " + sqlWhere;

            return ReplacePlaceHolderInputColumn(result, tempTable + ".");

        }

        /// <summary>
        /// Creates SQL update (using stored procedure) command
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns>sql update (using stored procedure) command</returns>
        public static string GetSqlUpdateSP(IsagCustomProperties properties, string tempTableName)
        {
            string result = "";

            string spStart =
                "CREATE PROCEDURE " + "<SPName>" + Environment.NewLine +
                "as" + Environment.NewLine +
                "BEGIN" + Environment.NewLine +
                "  set nocount on" + Environment.NewLine + Environment.NewLine;
            string spEnd =
                Environment.NewLine +
                "  Close myUpdateCursor" + Environment.NewLine +
                "  Deallocate myUpdateCursor" + Environment.NewLine +
                "  set nocount off" + Environment.NewLine +
                "END" + Environment.NewLine;

            string initVar = "";
            string fetch = "";
            string initCursor = "";
            string initCursorLeftJoin = "";
            string initCursorWhere = "";
            string update = "";
            string updateSet = "";
            string updateWhere = "";

            foreach (ColumnConfig config in properties.ColumnConfigList)
            {

                if (config.Update)
                {
                    if (initCursor != "")
                        initCursor += ", ";
                    initCursor += Brackets(tempTableName) + "." + Brackets(config.BulkColumnName);

                    initVar += "  Declare @" + config.BulkColumnName +
                               " as " + config.BulkDataType + Environment.NewLine;

                    if (fetch != "")
                        fetch += ", ";
                    fetch += "  @" + config.BulkColumnName;

                    if (updateSet != "")
                        updateSet += "," + Environment.NewLine + "      ";
                    updateSet += Brackets(properties.DestinationTable) + "." + Brackets(config.OutputColumnName) +
                                 " = ";
                    if (config.HasFunction)
                        updateSet += config.Function;
                    else
                        updateSet += "@" + config.BulkColumnName;
                }

                if (config.Key)
                {
                    if (!config.Update)
                    {
                        if (initCursor != "")
                            initCursor += ", ";
                        initCursor += Brackets(tempTableName) + "." + config.BulkColumnName;

                        initVar += "  Declare @" + config.BulkColumnName +
                                   " as " + config.BulkDataType + Environment.NewLine;

                        if (fetch != "")
                            fetch += ", ";
                        fetch += "  @" + config.BulkColumnName;
                    }

                    if (initCursorLeftJoin != "")
                        initCursorLeftJoin += " and ";
                    initCursorLeftJoin += Brackets(tempTableName) + "." + config.BulkColumnName +
                                        " = " +
                                        Brackets(properties.DestinationTable) + "." + config.OutputColumnName;

                    if (initCursorWhere == "")
                    {
                        initCursorWhere = Brackets(properties.DestinationTable) + "." + config.OutputColumnName +
                                          " Is Null";
                    }

                    if (updateWhere != "")
                        updateWhere += "        and ";
                    updateWhere += "@" + config.BulkColumnName +
                                   " = " +
                                   Brackets(properties.DestinationTable) + "." + Brackets(config.OutputColumnName)
                                   + Environment.NewLine;
                }
            }

            initCursor = Environment.NewLine +
                         "  Declare myUpdateCursor cursor FORWARD_ONLY for" + Environment.NewLine +
                         "  Select " + initCursor + Environment.NewLine +
                         "  from " + Brackets(tempTableName) + Environment.NewLine + Environment.NewLine;

            update = "  Update " + Brackets(properties.DestinationTable) + Environment.NewLine +
                     "  Set " + updateSet + Environment.NewLine +
                     "  Where " + updateWhere;


            fetch = Environment.NewLine + Environment.NewLine +
                    "  FETCH NEXT from myUpdateCursor into" + Environment.NewLine + fetch + Environment.NewLine;

            result = spStart + initVar + initCursor + "  Open myUpdateCursor" + fetch + Environment.NewLine +
                     "  While @@fetch_status = 0 begin" + Environment.NewLine + Environment.NewLine + update +
                     fetch.Replace(Environment.NewLine, Environment.NewLine + "  ") + Environment.NewLine +
                     "  END --while Insert" + Environment.NewLine +
                     spEnd;

            return ReplacePlaceHolderInputColumn(result, "@");
        }

        /// <summary>
        /// Creates SQL insert command
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns>sql insert command</returns>
        public static string GetSqlInsert(IsagCustomProperties properties, string tempTableName)
        {
            return GetSqlInsert(properties, tempTableName, false);
        }

        /// <summary>
        /// Creates SQL insert command
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="overrideCustomMergeCommand">If true, only custom sql template is returned (used for preview) </param>
        /// <returns></returns>
        public static string GetSqlInsert(IsagCustomProperties properties, string tempTableName, bool overrideCustomMergeCommand)
        {
            if (properties.UseCustomMergeCommand && !overrideCustomMergeCommand)
            {
                return properties.CustomMergeCommand;
            }
            else
            {
                string insert = "";
                string insertValues = "";
                string insertDefault;
                string insertValue;
                string placeholder;

                foreach (ColumnConfig config in properties.ColumnConfigList)
                {

                    if (config.Insert)
                    {
                        if (insert != "")
                            insert += ", ";
                        if (insertValues != "")
                            insertValues += ", ";
                        insert += Brackets(config.OutputColumnName);


                        insertValue = Brackets(config.BulkColumnName);
                        if (config.HasDefault)
                        {
                            insertDefault = "isnull(cast(" + insertValue + " as " + config.DataTypeOutput + ") , "
                                            + config.Default + ")";
                            if (config.HasFunction)
                            {
                                placeholder = "@(" + config.BulkColumnName + ")";
                                insertValues += config.Function.Replace(placeholder, insertDefault);
                            }
                            else
                                insertValues += insertDefault;
                        }
                        else if (config.HasFunction)
                        {
                            placeholder = "@(" + config.BulkColumnName + ")";
                            insertValues += config.Function.Replace(placeholder, config.BulkColumnName);
                        }
                        else
                            insertValues += insertValue;
                    }
                }

                string insertTmp = "INSERT INTO " + Brackets(properties.DestinationTable);
                if (!properties.DisableTablock)
                    insertTmp += " WITH (tablockx)";
                insertTmp += Environment.NewLine +
                             "  (" + insert + ")" + Environment.NewLine +
                             "SELECT " + insertValues + Environment.NewLine +
                             "FROM " + Brackets(tempTableName);
                insert = insertTmp;

                return ReplacePlaceHolderInputColumn(insert, "");
            }
        }

        /// <summary>
        /// Creates SQL insert (using stored procedure) command
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns>sql insert (using stored procedure) command</returns>
        public static string GetSqlInsertSP(IsagCustomProperties properties, string tempTableName)
        {
            string result = "";

            string spStart =
                "CREATE PROCEDURE " + "<SPName>" + Environment.NewLine +
                "as" + Environment.NewLine +
                "BEGIN" + Environment.NewLine +
                "  set nocount on" + Environment.NewLine + Environment.NewLine;
            string spEnd =
                Environment.NewLine +
                "  Close myInsertCursor" + Environment.NewLine +
                "  Deallocate myInsertCursor" + Environment.NewLine +
                "  set nocount off" + Environment.NewLine +
                "END" + Environment.NewLine;

            string initVar = "";
            string fetch = "";
            string initCursor = "";
            string initCursorLeftJoin = "";
            string initCursorWhere = "";
            string insert = "";
            string insertValues = "";

            string insertDefault;
            string insertValue;


            foreach (ColumnConfig config in properties.ColumnConfigList)
            {

                if (config.Insert)
                {
                    if (insert != "")
                        insert += ", ";
                    if (insertValues != "")
                        insertValues += ", ";
                    insert += config.OutputColumnName;


                    insertValue = "@(" + config.BulkColumnName + ")";
                    if (config.HasDefault)
                    {
                        insertDefault = "isnull(cast(" + insertValue + " as " + config.DataTypeOutput + ") , "
                                        + config.Default + ")";
                        if (config.HasFunction)
                            insertValues += config.Function.Replace(insertValue, insertDefault);
                        else
                            insertValues += insertDefault;
                    }
                    else if (config.HasFunction)
                        insertValues += config.Function;
                    else
                        insertValues += insertValue;

                    if (initCursor != "")
                        initCursor += ", ";
                    initCursor += Brackets(tempTableName) + "." + Brackets(config.BulkColumnName);

                    initVar += "  Declare @" + config.BulkColumnName +
                               " as " + config.BulkDataType + Environment.NewLine;

                    if (fetch != "")
                        fetch += ", ";
                    fetch += "  @" + config.BulkColumnName;
                }

                if (config.Key)
                {
                    if (initCursorLeftJoin != "")
                        initCursorLeftJoin += " and ";
                    initCursorLeftJoin += Brackets(tempTableName) + "." + Brackets(config.BulkColumnName) +
                                        " = " +
                                        Brackets(properties.DestinationTable) + "." + Brackets(config.OutputColumnName);

                    if (initCursorWhere == "")
                    {
                        initCursorWhere = Brackets(properties.DestinationTable) + "." + Brackets(config.OutputColumnName) +
                                          " Is Null";
                    }

                }
            }

            insert = "    insert into " + Brackets(properties.DestinationTable) + Environment.NewLine +
                     "      (" + insert + ")" + Environment.NewLine +
                     "    Values (" + insertValues + ")";

            initCursor = Environment.NewLine +
                         "  Declare myInsertCursor cursor FORWARD_ONLY for" + Environment.NewLine +
                         "  Select " + initCursor + Environment.NewLine +
                         "  from " + Brackets(tempTableName) + Environment.NewLine +
                         "  Left Join " + Brackets(properties.DestinationTable) + Environment.NewLine +
                         "  On " + initCursorLeftJoin + Environment.NewLine +
                         "  Where " + initCursorWhere + Environment.NewLine + Environment.NewLine;

            fetch = Environment.NewLine + Environment.NewLine +
                    "  FETCH NEXT from myInsertCursor into" + Environment.NewLine + fetch + Environment.NewLine;

            result = spStart + initVar + initCursor + "  Open myInsertCursor" + fetch + Environment.NewLine +
                     "  While @@fetch_status = 0 begin" + Environment.NewLine + Environment.NewLine + insert +
                     fetch.Replace(Environment.NewLine, Environment.NewLine + "  ") + Environment.NewLine +
                     "  END --while Insert" + Environment.NewLine +
                     spEnd;

            return ReplacePlaceHolderInputColumn(result, "@");
        }

        #endregion

        #region Table

        /// <summary>
        /// Create SQL statement for creating temporary table (if merge command is used, an index is created) 
        /// </summary>
        /// <returns>sql create temporary table command</returns>
        public static string GetCreateTempTable( IsagCustomProperties properties, string tempTableName)
        {
            string returnValue = "";
            string indexColumns = "";

            foreach (ColumnConfig config in properties.BulkCopyColumnConfigLIst)
            {
                if (returnValue != "")
                    returnValue += ", ";

                returnValue += Brackets(config.BulkColumnName) + " ";
                returnValue += config.BulkDataType + Environment.NewLine;

                if (properties.UseMerge && config.Key)
                {
                    if (indexColumns != "")
                        indexColumns += ", ";
                    indexColumns += Brackets(config.BulkColumnName) + " ASC";
                }
            }

            returnValue = "CREATE TABLE " + tempTableName + Environment.NewLine +
                          "( " + returnValue + " )";

            if (properties.UseMerge)
                returnValue += ";" + Environment.NewLine + Constants.CREATE_INDEX.Replace("<table>", tempTableName).Replace("<columns>", indexColumns);

            return returnValue;
        }

        /// <summary>
        /// Create SQL statement for creating destination table
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <returns>sql statement for creating destination table</returns>
        public static string GetCreateDestinationTable(IsagCustomProperties properties)
        {
            StringBuilder result = new StringBuilder("CREATE TABLE [TableLoader Destination] (" + Environment.NewLine);

            bool appendComma = false;

            foreach (ColumnConfig config in properties.ColumnConfigList)
            {

                if (config.HasInput)
                {
                    if (appendComma)
                        result.Append("," + Environment.NewLine);
                    appendComma = true;
                    result.Append("  ");
                    result.Append(Brackets(config.InputColumnName));
                    result.Append(" ");
                    result.Append(config.DataTypeInput);
                }
            }

            result.Append(Environment.NewLine);
            result.Append(")");

            return result.ToString();
        }

        /// <summary>
        /// Create SQL statement for altering destination table
        /// </summary>
        /// <param name="properties">componets custom properties</param>
        /// <param name="tempTableName">temporary table name</param>
        /// <returns>sql statement for altering destination table</returns>
        public static string GetAlterDestinationTable(IsagCustomProperties properties, SqlColumnList sqlColumns)
        {
            string result = "";

            if (properties.HasDestinationTable)
            {
                foreach (ColumnConfig config in properties.ColumnConfigList)
                {
                    string outputColumnName = sqlColumns.GetMatchingColumnname(config.InputColumnName, properties.PrefixInput, properties.PrefixOutput);
                    if (config.HasInput && outputColumnName == "")
                    {
                        if (result == "")
                            result += "  ADD ";
                        else
                            result += "," + Environment.NewLine + "      ";
                        result += Brackets(config.InputColumnName) + " " + config.DataTypeInput;
                    }
                }
            }

            return "ALTER TABLE " + Brackets(properties.DestinationTable) + Environment.NewLine + result;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Surrounds sql parts (tablename, schema,...) with brackets
        /// </summary>
        /// <param name="value">s</param>
        /// <returns>sql parts (tablename, schema,...) with brackets</returns>
        public static string Brackets(string value)
        {
            string result = "";

            if (value.Contains("["))
                return value;

            if (!value.Contains("."))
            {
                result = "[" + value + "]";
            }
            else
            {
                string[] split = value.Split(".".ToCharArray());

                for (int i = 0; i < split.Length; i++)
                {
                    result += Brackets(split[i]) + ".";
                }
                result = result.Trim(".".ToCharArray());
            }

            return result;
        }

        /// <summary>
        /// Gets sql datatype for an SSIS input column
        /// </summary>
        /// <param name="inputCol">SSIS input column</param>
        /// <returns>sql datatype for an SSIS input column</returns>
        public static string GetSQLServerDataTypeFromInput(IDTSInputColumn100 inputCol)
        {
            return GetSQLServerDataTypeFromInput(inputCol.DataType, inputCol.Length.ToString(),
                                        inputCol.Precision.ToString(), inputCol.Scale.ToString());
        }

        /// <summary>
        /// Gets sql datatype for SSIS input columns datatype porperties
        /// </summary>
        /// <param name="dataType">SSIS datatype</param>
        /// <param name="ColumnSize">column size</param>
        /// <param name="NumericPrecision">numeric precision</param>
        /// <param name="NumericScale">numeric scale</param>
        /// <returns>sql datatype for SSIS input columns datatype porperties</returns>
        public static string GetSQLServerDataTypeFromInput(DataType dataType, string ColumnSize,
                                         string NumericPrecision, string NumericScale)
        {
            switch (dataType)
            {
                case DataType.DT_BOOL:
                    return "bit";
                case DataType.DT_BYTES:
                    return "varbinary(" + ColumnSize + ")";
                case DataType.DT_CY:
                    return "money";
                case DataType.DT_DATE:
                    return "datetime2";
                case DataType.DT_DBDATE:
                    return "date";
                case DataType.DT_DBTIME:
                    return "time";
                case DataType.DT_DBTIME2:
                    return "time";
                case DataType.DT_DBTIMESTAMP:
                    return "datetime";
                case DataType.DT_DBTIMESTAMP2:
                    return "datetime2";
                case DataType.DT_DBTIMESTAMPOFFSET:
                    return "datetimeoffset";
                case DataType.DT_DECIMAL:
                    return "decimal(29," + NumericScale + ")";
                case DataType.DT_GUID:
                    return "uniqueidentifier";
                case DataType.DT_I1:
                    return "smallint";
                case DataType.DT_I2:
                    return "smallint";
                case DataType.DT_I4:
                    return "int";
                case DataType.DT_I8:
                    return "bigint";
                case DataType.DT_IMAGE:
                    return "image";
                case DataType.DT_NTEXT:
                    return "ntext";
                case DataType.DT_NUMERIC:
                    return "decimal(" + NumericPrecision + "," + NumericScale + ")";
                case DataType.DT_R4:
                    return "real";
                case DataType.DT_R8:
                    return "real";
                case DataType.DT_STR:
                    return "varchar(" + ColumnSize + ")";
                case DataType.DT_TEXT:
                    return "text";
                case DataType.DT_UI1:
                    return "tinyint";
                case DataType.DT_UI2:
                    return "int";
                case DataType.DT_UI4:
                    return "bigint";
                case DataType.DT_UI8:
                    return "bigint";
                case DataType.DT_WSTR:
                    return "nvarchar(" + ColumnSize + ")";
                default:
                    System.Windows.Forms.MessageBox.Show("Unsupported DataType: " + dataType.ToString());
                    throw new Exception();
            }
        }

        /// <summary>
        /// Gets .NET datatype from SSIS datatype
        /// </summary>
        /// <param name="dataType">SSIS datatype</param>
        /// <returns>.NET datatype</returns>
        public static System.Type GetNetDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.DT_BOOL:
                    return typeof(bool);
                case DataType.DT_GUID:
                    return typeof(Guid);
                case DataType.DT_IMAGE:
                    return typeof(System.Byte[]);
                case DataType.DT_BYTES:
                    return typeof(System.Byte[]);
                case DataType.DT_NUMERIC:
                    return typeof(System.Decimal);
                case DataType.DT_DECIMAL:
                    return typeof(System.Decimal);
                case DataType.DT_DATE:
                    return typeof(DateTime);
                case DataType.DT_DBDATE:
                    return typeof(DateTime);
                case DataType.DT_DBTIME:
                    return typeof(DateTime);
                case DataType.DT_DBTIME2:
                    return typeof(DateTime);
                case DataType.DT_DBTIMESTAMP:
                    return typeof(DateTime);
                case DataType.DT_DBTIMESTAMP2:
                    return typeof(DateTime);
                case DataType.DT_DBTIMESTAMPOFFSET:
                    return typeof(DateTimeOffset);
                case DataType.DT_I1:
                    return typeof(System.Byte);
                case DataType.DT_I2:
                    return typeof(Int16);
                case DataType.DT_I4:
                    return typeof(Int32);
                case DataType.DT_I8:
                    return typeof(Int64);
                case DataType.DT_R4:
                    return typeof(Single);
                case DataType.DT_R8:
                    return typeof(Double);
                case DataType.DT_UI1:
                    return typeof(System.Byte);
                case DataType.DT_UI2:
                    return typeof(System.UInt16);
                case DataType.DT_UI4:
                    return typeof(System.UInt32);
                case DataType.DT_UI8:
                    return typeof(UInt64);
                case DataType.DT_WSTR:
                    return typeof(System.String);
                case DataType.DT_STR:
                    return typeof(System.String);
                case DataType.DT_TEXT:
                    return typeof(System.String);
                case DataType.DT_NTEXT:
                    return typeof(System.String);
                case DataType.DT_CY:
                    return typeof(System.Decimal);
                default:
                    System.Windows.Forms.MessageBox.Show("Unsupported DataType: " + dataType.ToString());
                    throw new Exception();
            }
        }

        /// <summary>
        /// Gets sql datatype
        /// </summary>
        /// <param name="sqlDataType">sql datatype as string</param>
        /// <param name="ColumnSize">sql column size</param>
        /// <param name="NumericPrecision">sql numeric precision</param>
        /// <param name="NumericScale">sql numeric scale</param>
        /// <returns>sql datatype</returns>
        public static string GetSQLServerDataType(string sqlDataType, string ColumnSize,
                                                  string NumericPrecision, string NumericScale)
        {
            switch (sqlDataType)
            {
                case "nvarchar":
                    return sqlDataType + "(" + ColumnSize + ")";
                case "nchar":
                    return sqlDataType + "(" + ColumnSize + ")";
                case "varchar":
                    return sqlDataType + "(" + ColumnSize + ")";
                case "decimal":
                    return sqlDataType + "(" + NumericPrecision + "," + NumericScale + ")";
                case "numeric":
                    return sqlDataType + "(" + NumericPrecision + "," + NumericScale + ")";
                case "time":
                    return sqlDataType + "(" + NumericScale + ")";
                case "varbinary":
                    return sqlDataType + "(" + ColumnSize + ")";
                default:
                    return sqlDataType;
            }
        }

        /// <summary>
        /// Replaces placeholder like @(&lt;InputColumnName&gt;) with prefix + input column name
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="prefix">column prefix (i.e. "src.")</param>
        /// <returns>sql statement</returns>
        private static string ReplacePlaceHolderInputColumn(string sqlCommand, string prefix)
        {
            while (sqlCommand.Contains("@("))
            {

                int pos = sqlCommand.IndexOf("@(");
                int posEnd = sqlCommand.IndexOf(")", pos);
                if (posEnd == -1)
                    sqlCommand = "Function ist not valid!";
                else
                {
                    string placeHolder = sqlCommand.Substring(pos, posEnd - pos + 1);
                    string content = placeHolder.Substring(2, placeHolder.Length - 3);
                    sqlCommand = sqlCommand.Replace(placeHolder, prefix + content);
                }
            }

            return sqlCommand;
        }
        #endregion

    }


}
