using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TableLoader.SCD {
    /// <summary>
    /// List of definition for slowly changing dimension handling
    /// </summary>
    public class SCDList {
        /// <summary>
        /// Template for creating temporary table
        /// </summary>
        public static string TEMPLATE_CREATE_TEMP_TABLE =
           @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'#<tempTableName_scd>') AND type in (N'U'))" + Environment.NewLine +
           "DROP TABLE #<tempTableName_scd>" + Environment.NewLine + Environment.NewLine +
           "CREATE TABLE #<tempTableName_scd>(" + Environment.NewLine +
           "  MergeAction [nvarchar](255)" + Environment.NewLine +
           "  ,<FK_ID> INT NOT NULL" + Environment.NewLine +
           "<bk>" +
           "<attributes>" +
           "<valid_from>" +
           ")";

        /// <summary>
        /// Template for creating index on temporary table
        /// </summary>
        public static string TEMPLATE_CREATE_INDEX_ON_TEMP_TABLE =
            "CREATE <clustered_type> INDEX [IX_scdTemp_<index_name>] ON #<tempTableName_scd>" + Environment.NewLine +
            "(" + Environment.NewLine +
            "<index_columns>" +
            ") ";

        /// <summary>
        /// Template for creating a nonclusterd index on temporary table
        /// </summary>
        public static string TEMPLATE_CREATE_NONCLUSTERD_INDEX_ON_TEMP_TABLE =
            TEMPLATE_CREATE_INDEX_ON_TEMP_TABLE.Replace("<clustered_type>", "NONCLUSTERED");

        /// <summary>
        /// Template for creating a clusterd index on temporary table
        /// </summary>
        public static string TEMPLATE_CREATE_CLUSTERD_INDEX_ON_TEMP_TABLE =
            TEMPLATE_CREATE_INDEX_ON_TEMP_TABLE
                .Replace("<clustered_type>", "CLUSTERED")
                .Replace("<index_name>", "BK");


        /// <summary>
        /// Template for OUTPUT part of merge statement
        /// </summary>
        public static string TEMPLATE_OUTPUT =
           @"OUTPUT" + Environment.NewLine +
           "  $action as MergeAction" + Environment.NewLine +
           "  ,INSERTED.ID" + Environment.NewLine +
           "<bk_output>" +
           "<attribute_select>" +
           "<vaild_from_select>" + Environment.NewLine +
           "INTO #<tempTableName_scd>" +
           "(" + Environment.NewLine +
           "  MergeAction" + Environment.NewLine +
           "  ,<FK_ID>" + Environment.NewLine +
            "<bk_into>" +
           "<attribute_insert>" +
           "<vaild_from_insert>" + Environment.NewLine +
           ");";

        /// <summary>
        /// Template for creating SCD table
        /// </summary>
        public static string TEMPLATE_CREATE_SCD_TABLE =
          @"CREATE TABLE <tableName>(" + Environment.NewLine +
          "   [ID] [int] IDENTITY(1,1) NOT NULL" + Environment.NewLine +
          "   ,<FK_ID> INT NOT NULL" + Environment.NewLine +
          "<bks>" +
          "<attributes>" +
          "  ,[VALID_FROM] [int] NOT NULL" + Environment.NewLine +
          "  ,[VALID_TO] [int] NOT NULL" + Environment.NewLine +
          "  ,[IsActive] [tinyint] NOT NULL" + Environment.NewLine +
          "  ,[MergeAction] [nvarchar](255) NULL" + Environment.NewLine +
          "  ,[isETL_DMR] [datetime] NULL" + Environment.NewLine +
          "  ,[isETL_DCR] [datetime] NULL" + Environment.NewLine +
          " CONSTRAINT [PK_<tableName>] PRIMARY KEY CLUSTERED ([ID])" + Environment.NewLine +
          ")" + Environment.NewLine +
          "GO" + Environment.NewLine + Environment.NewLine +
          "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_VALID_TO]  DEFAULT ((<GranularityMaxValue>)) FOR [VALID_TO]" + Environment.NewLine +
          "GO" + Environment.NewLine + Environment.NewLine +
          "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_IsActive]  DEFAULT ((0)) FOR [IsActive]" + Environment.NewLine +
          "GO" + Environment.NewLine + Environment.NewLine +
          "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_isETL_DMR]  DEFAULT (getdate()) FOR [isETL_DMR]" + Environment.NewLine +
          "GO" + Environment.NewLine + Environment.NewLine +
          "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_isETL_DCR]  DEFAULT (getdate()) FOR [isETL_DCR]" + Environment.NewLine +
          "GO";

        /// <summary>
        /// Dictionary of table names (key) and SCDs (values)
        /// </summary>
        Dictionary<string, SCDConfiguration> _scdList = new Dictionary<string, SCDConfiguration>();

        /// <summary>
        /// Prefix for foreign key
        /// </summary>
        private string _prefixFK = "";

        /// <summary>
        /// Sets foreign key column prefix (=FK_tablename_)
        /// </summary>
        /// <param name="tableLoaderDestinationTable">TableLoader destination table (not SCD table)</param>
        private void SetFKColumnPrefix(string tableLoaderDestinationTable)
        {
            string tableName = tableLoaderDestinationTable;

            if (tableName.Contains("."))
                tableName = tableName.Split(".".ToCharArray())[1];
            if (tableName.ToUpper().StartsWith("TBL"))
                tableName = tableName.Substring(3);

            _prefixFK = "FK_" + tableName + "_";
        }

        /// <summary>
        /// Creates a new list of SCDs (slowly changing dimension)
        /// </summary>
        /// <param name="columnConfigList">Column config list</param>
        /// <param name="tableLoaderDestinationTable">TableLoader destination table (not SCD table)</param>
        public SCDList(BindingList<ColumnConfig> columnConfigList, string tableLoaderDestinationTable)
        {
            if (tableLoaderDestinationTable == null)
                tableLoaderDestinationTable = string.Empty;

            SetFKColumnPrefix(tableLoaderDestinationTable);

            List<SCDColumn> bkList = new List<SCDColumn>();

            foreach (ColumnConfig config in columnConfigList)
            {
                if (config.Key)
                {
                    bkList.Add(new SCDColumn(config.OutputColumnName, config.BulkDataType));
                }
            }

            foreach (ColumnConfig config in columnConfigList)
            {
                if (!string.IsNullOrEmpty(config.ScdTable))
                {
                    string[] scdTables = config.ScdTable.Split(",".ToCharArray());
                    for (int i = 0; i < scdTables.Length; i++)
                    {
                        string tableName = scdTables[i].Trim();
                        SCDConfiguration scd = GetSCD(tableName, config.ScdTimeStampGranularity);

                        scd.BkList = bkList;
                        scd.TableName = tableName;
                        scd.PrefixFK = _prefixFK;

                        if (config.IsScdColumn)
                        {
                            scd.AttributeList.Add(new SCDColumn(config.BulkColumnName, config.BulkDataType));
                        }

                        if (config.IsScdValidFrom)
                        {
                            scd.ValidFrom = new SCDColumn(config.BulkColumnName, config.BulkDataType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Are all SCD configuration valid?
        /// </summary>
        /// <param name="message">Errors in a SCD configuration will be written into this ref parameter</param>
        /// <returns>Are all SCD configuration valid?</returns>
        public bool IsValid(ref string message)
        {
            message = "";
            bool isValid = true;

            foreach (string tableName in _scdList.Keys)
            {
                SCDConfiguration scd = _scdList[tableName];
                string scdMessage = "";
                isValid = isValid & scd.IsValid(ref scdMessage);
                if (scdMessage != "")
                {
                    if (message != "")
                        message += Environment.NewLine;
                    message += scdMessage;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Creates sql command for creating the SCD table
        /// </summary>
        /// <returns>sql command for creating the SCD </returns>
        public string GetCreateScdTables()
        {
            string result = "";

            foreach (string tableName in _scdList.Keys)
            {
                SCDConfiguration scd = _scdList[tableName];
                string bks = SCDHelper.GetSqlBkListWithDataType(scd.BkList, _prefixFK, "", 2);
                string attributes = SCDHelper.GetSqlColumnListWithDataType(scd.AttributeList, "", "", 2, false, "NULL", "");


                string create = TEMPLATE_CREATE_SCD_TABLE.Replace("<tableName>", tableName);
                create = create.Replace("<GranularityMaxValue>", scd.GranularityMaxValue);
                create = create.Replace("<bks>", bks);
                create = create.Replace("<attributes>", attributes);
                create = create.Replace("<FK_ID>", _prefixFK + "ID");
                create += Environment.NewLine + Environment.NewLine;

                result += create;
            }

            return result;
        }

        /// <summary>
        /// Get SCD configuration for a SCD table name
        /// </summary>
        /// <param name="tableName">SCD table name</param>
        /// <param name="timeStampGranularity">timeStampGranularity</param>
        /// <returns>SCDConfiguration</returns>
        private SCDConfiguration GetSCD(string tableName, ColumnConfig.ScdTimeStampGranularityType timeStampGranularity)
        {
            if (_scdList.ContainsKey(tableName))
                return _scdList[tableName];

            SCDConfiguration newScd = new SCDConfiguration(timeStampGranularity);
            _scdList.Add(tableName, newScd);

            return newScd;
        }

        /// <summary>
        /// Insert merge statement into TableLoaders database (merge) command
        /// </summary>
        /// <param name="merge">TableLoaders database (merge) command</param>
        /// <param name="properties">Components custom properties</param>
        /// <param name="tempTableName">TableLoader temporary table</param>
        /// <returns>TableLoaders database (merge) command with merge statement for SCD</returns>
        public string InsertIntoMergeStatement(string merge, IsagCustomProperties properties, string tempTableName)
        {
            string tempTableScd = "SCD_" + tempTableName;

            merge = merge.Trim();
            merge = merge.Substring(0, merge.Length - 1); //Semikolon entfernen

            string result = GetCreateTempTable(tempTableScd) + Environment.NewLine + Environment.NewLine;
            result += merge + Environment.NewLine + Environment.NewLine;
            result += GetOutputPart(tempTableScd) + Environment.NewLine + Environment.NewLine;
            foreach (string scdTableName in _scdList.Keys)
            {
                result += _scdList[scdTableName].GetMergeStatement(tempTableScd) + Environment.NewLine + Environment.NewLine;
                ;
            }

            result += GetDropTempTable(tempTableScd);

            return result;
        }

        /// <summary>
        /// Get sql command for dropping temprary table
        /// </summary>
        /// <param name="tempTableName">SCD temporary table name</param>
        /// <returns>Sql command for dropping SCD temprary table</returns>
        private string GetDropTempTable(string tempTableName)
        {
            return "DROP TABLE #" + tempTableName;

        }

        /// <summary>
        /// Get sql coomand for creating SCD temporary table
        /// </summary>
        /// <param name="tempTableName">SCD temporary table name</param>
        /// <returns>Sql coomand for creating SCD temporary table</returns>
        private string GetCreateTempTable(string tempTableName)
        {
            string result = TEMPLATE_CREATE_TEMP_TABLE.Replace("<tempTableName_scd>", tempTableName);
            string indexBk = TEMPLATE_CREATE_CLUSTERD_INDEX_ON_TEMP_TABLE.Replace("<tempTableName_scd>", tempTableName);
            string indexAttributes = "";

            ScdColumnList scdListBKs = new ScdColumnList();
            ScdColumnList scdListAttributes = new ScdColumnList();
            foreach (string scdTableName in _scdList.Keys)
            {
                scdListBKs.AddList(_scdList[scdTableName].BkList);
                scdListAttributes.AddList(_scdList[scdTableName].AttributeList);
            }

            result = result.Replace("<bk>", SCDHelper.GetSqlBkListWithDataType(scdListBKs.ScdColumns, _prefixFK, "", 2));
            result = result.Replace("<attributes>", SCDHelper.GetSqlAttributeListWithDataType(scdListAttributes.ScdColumns, SCDConfiguration.POSTFIX_COLUMN_QUELLE, SCDConfiguration.POSTFIX_COLUMN_DWH_VOR_UPDATE, 2));
            result = result.Replace("<valid_from>", SCDHelper.GetSqlValidFromWithDataType(_scdList, 2));
            result = result.Replace("<FK_ID>", _prefixFK + "ID");

            indexBk = indexBk.Replace("<index_columns>", "  " +
                SCDHelper.GetSqlBkList(scdListBKs.ScdColumns, _prefixFK, "", 2).Substring(3)); //Substring(3):  Komma an 3. Stelle entfernen 

            foreach (SCDColumn scdColumn in scdListAttributes.ScdColumns)
            {
                string index = TEMPLATE_CREATE_NONCLUSTERD_INDEX_ON_TEMP_TABLE
                                .Replace("<tempTableName_scd>", tempTableName) //.TrimStart("[".ToCharArray()).TrimEnd("]".ToCharArray()))
                                .Replace("<index_name>", scdColumn.ColumnName);

                List<SCDColumn> scdColumns = new List<SCDColumn>();
                scdColumns.Add(scdColumn);

                string columns = SCDHelper.GetSqlAttributeListDoubled(scdColumns, "", "", SCDConfiguration.POSTFIX_COLUMN_QUELLE, SCDConfiguration.POSTFIX_COLUMN_DWH_VOR_UPDATE, 2);
                columns = "  " + columns.Substring(3); // Komma an 3. Stelle entfernen 
                index = index.Replace("<index_columns>", columns);
                indexAttributes += index + Environment.NewLine + Environment.NewLine;
            }

            return result + Environment.NewLine + Environment.NewLine + indexBk + Environment.NewLine + Environment.NewLine + indexAttributes;
        }

        /// <summary>
        /// Get output part of SCD sql command 
        /// </summary>
        /// <param name="tempTableName">SCD temporary table name</param>
        /// <returns>Output part of SCD sql command</returns>
        private string GetOutputPart(string tempTableName)
        {
            string result = TEMPLATE_OUTPUT;

            ScdColumnList scdListBKs = new ScdColumnList();
            ScdColumnList scdListAttributes = new ScdColumnList();
            foreach (string scdTableName in _scdList.Keys)
            {
                scdListBKs.AddList(_scdList[scdTableName].BkList);
                scdListAttributes.AddList(_scdList[scdTableName].AttributeList);
            }

            result = result.Replace("<tempTableName_scd>", tempTableName);
            result = result.Replace("<attribute_select>", SCDHelper.GetSqlAttributeListDoubled(scdListAttributes.ScdColumns, "src.", "DELETED.", "", "", 2));
            result = result.Replace("<vaild_from_select>", SCDHelper.GetSqlValidFrom(_scdList, false, "src.", 2));
            result = result.Replace("<attribute_insert>", SCDHelper.GetSqlAttributeListDoubled(scdListAttributes.ScdColumns, "", "",
            SCDConfiguration.POSTFIX_COLUMN_QUELLE, SCDConfiguration.POSTFIX_COLUMN_DWH_VOR_UPDATE, 2));
            result = result.Replace("<vaild_from_insert>", SCDHelper.GetSqlValidFrom(_scdList, true, "", 2));
            result = result.Replace("<bk_output>", SCDHelper.GetSqlBkList(scdListBKs.ScdColumns, "INSERTED.", 2));
            result = result.Replace("<bk_into>", SCDHelper.GetSqlBkList(scdListBKs.ScdColumns, _prefixFK, "", 2));
            result = result.Replace("<FK_ID>", _prefixFK + "ID");

            return result;
        }
    }
}
