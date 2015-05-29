using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TableLoader
{
    public class SCDList
    {
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

        public static string TEMPLATE_CREATE_INDEX_ON_TEMP_TABLE =
            "CREATE <clustered_type> INDEX [IX_scdTemp_<index_name>] ON #<tempTableName_scd>" + Environment.NewLine +
            "(" + Environment.NewLine +
            "<index_columns>" +
            ") ";

        public static string TEMPLATE_CREATE_NONCLUSTERD_INDEX_ON_TEMP_TABLE =
            TEMPLATE_CREATE_INDEX_ON_TEMP_TABLE.Replace("<clustered_type>", "NONCLUSTERED");

        public static string TEMPLATE_CREATE_CLUSTERD_INDEX_ON_TEMP_TABLE =
            TEMPLATE_CREATE_INDEX_ON_TEMP_TABLE
                .Replace("<clustered_type>", "CLUSTERED")
                .Replace("<index_name>", "BK");


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
           "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_VALID_TO]  DEFAULT ((99999999)) FOR [VALID_TO]" + Environment.NewLine +
           "GO" + Environment.NewLine + Environment.NewLine +
           "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_IsActive]  DEFAULT ((0)) FOR [IsActive]" + Environment.NewLine +
           "GO" + Environment.NewLine + Environment.NewLine +
           "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_isETL_DMR]  DEFAULT (getdate()) FOR [isETL_DMR]" + Environment.NewLine +
           "GO" + Environment.NewLine + Environment.NewLine +
           "ALTER TABLE <tableName> ADD  CONSTRAINT [DF_<tableName>_isETL_DCR]  DEFAULT (getdate()) FOR [isETL_DCR]" + Environment.NewLine +
           "GO";


        Dictionary<string, SCD> _scdList = new Dictionary<string, SCD>();

        private string _prefixFK = "";

        private void SetFKColumnPrefix(string tableLoaderDestinationTable)
        {
            string tableName = tableLoaderDestinationTable;

            if (tableName.Contains(".")) tableName = tableName.Split(".".ToCharArray())[1];
            if (tableName.ToUpper().StartsWith("TBL")) tableName = tableName.Substring(3);

           _prefixFK =  "FK_" + tableName + "_";        
        }

       
        public SCDList(BindingList<ColumnConfig> columnConfigList, string tableLoaderDestinationTable)
        {
            if (tableLoaderDestinationTable == null) tableLoaderDestinationTable = string.Empty;

           SetFKColumnPrefix(tableLoaderDestinationTable);

            List<SCDCOlumn> bkList = new List<SCDCOlumn>();

            foreach (ColumnConfig config in columnConfigList)
            {
                if (config.Key)
                {
                    bkList.Add(new SCDCOlumn(config.OutputColumnName, config.BulkDataType));
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
                        SCD scd = GetSCD(tableName);

                        scd.BkList = bkList;
                        scd.TableName = tableName;
                        scd.PrefixFK = _prefixFK;

                        if (config.IsScdColumn)
                        {
                            scd.AttributeList.Add(new SCDCOlumn(config.BulkColumnName, config.BulkDataType));
                        }

                        if (config.IsScdValidFrom)
                        {
                            scd.ValidFrom = new SCDCOlumn(config.BulkColumnName, config.BulkDataType);
                        }
                    }
                }
            }
        }

        public bool IsValid(ref string message)
        {
            message = "";
            bool isValid = true;

            foreach (string tableName in _scdList.Keys)
            {
                SCD scd = _scdList[tableName];
                string scdMessage = "";
                isValid = isValid & scd.IsValid(ref scdMessage);
                if (scdMessage != "")
                {
                    if (message != "") message += Environment.NewLine;
                    message += scdMessage;
                }
            }

            return isValid;
        }

        public string GetCreateScdTables()
        {
            string result = "";

            foreach (string tableName in _scdList.Keys)
            {
                SCD scd = _scdList[tableName];
                string bks = SCDHelper.GetSqlBkListWithDataType(scd.BkList, _prefixFK, "", 2);
                string attributes = SCDHelper.GetSqlColumnListWithDataType(scd.AttributeList, "", "", 2, false, "NULL", "");


                string create = TEMPLATE_CREATE_SCD_TABLE.Replace("<tableName>", tableName);
                create = create.Replace("<bks>", bks);                
                create = create.Replace("<attributes>", attributes);
                create = create.Replace("<FK_ID>", _prefixFK + "ID");
                create += Environment.NewLine + Environment.NewLine;

                result += create;
            }

            return result;
        }

        private SCD GetSCD(string tableName)
        {
            if (_scdList.ContainsKey(tableName)) return _scdList[tableName];

            SCD newScd = new SCD();
            _scdList.Add(tableName, newScd);

            return newScd;
        }

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
                result += _scdList[scdTableName].GetMergeStatement(tempTableScd) + Environment.NewLine + Environment.NewLine; ;
            }

            result += GetDropTempTable(tempTableScd);

            return result;
        }

        private string GetDropTempTable(string tempTableName)
        {
            return "DROP TABLE #" + tempTableName;

        }
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
            result = result.Replace("<attributes>", SCDHelper.GetSqlAttributeListWithDataType(scdListAttributes.ScdColumns, SCD.POSTFIX_COLUMN_QUELLE, SCD.POSTFIX_COLUMN_DWH_VOR_UPDATE, 2));
            result = result.Replace("<valid_from>", SCDHelper.GetSqlValidFromWithDataType(_scdList, 2 ));
            result = result.Replace("<FK_ID>", _prefixFK + "ID");

            indexBk = indexBk.Replace("<index_columns>", "  " + 
                SCDHelper.GetSqlBkList(scdListBKs.ScdColumns, _prefixFK, "", 2).Substring(3)); //Substring(3):  Komma an 3. Stelle entfernen 

            foreach (SCDCOlumn scdColumn in scdListAttributes.ScdColumns)
            {
                string index = TEMPLATE_CREATE_NONCLUSTERD_INDEX_ON_TEMP_TABLE
                                .Replace("<tempTableName_scd>", tempTableName) //.TrimStart("[".ToCharArray()).TrimEnd("]".ToCharArray()))
                                .Replace("<index_name>", scdColumn.ColumnName);

                List<SCDCOlumn> scdColumns = new List<SCDCOlumn>();
                scdColumns.Add(scdColumn);

                string columns = SCDHelper.GetSqlAttributeListDoubled(scdColumns, "", "", SCD.POSTFIX_COLUMN_QUELLE, SCD.POSTFIX_COLUMN_DWH_VOR_UPDATE, 2);
                columns = "  " + columns.Substring(3); // Komma an 3. Stelle entfernen 
                index = index.Replace("<index_columns>", columns);
                indexAttributes += index + Environment.NewLine + Environment.NewLine;
            }

            return result + Environment.NewLine + Environment.NewLine + indexBk + Environment.NewLine + Environment.NewLine + indexAttributes;
        }

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
            SCD.POSTFIX_COLUMN_QUELLE, SCD.POSTFIX_COLUMN_DWH_VOR_UPDATE, 2));
            result = result.Replace("<vaild_from_insert>", SCDHelper.GetSqlValidFrom(_scdList, true, "", 2));
            result = result.Replace("<bk_output>", SCDHelper.GetSqlBkList(scdListBKs.ScdColumns, "INSERTED.", 2));
            result = result.Replace("<bk_into>", SCDHelper.GetSqlBkList(scdListBKs.ScdColumns,_prefixFK, "", 2));
            result = result.Replace("<FK_ID>", _prefixFK + "ID");

            return result;
        }
    }



    public class SCD
    {
        public static string POSTFIX_COLUMN_QUELLE = "_Quelle";
        public static string POSTFIX_COLUMN_DWH_VOR_UPDATE = "_DWH_Vor_Update";

        public static string TEMPLATE_INSERT =
           "INSERT INTO <scd_tablename>" + Environment.NewLine +
           "(  [MergeAction]" + Environment.NewLine +
            "  ,<FK_ID>" + Environment.NewLine +
           "<bk_scd>" +
           "<attributes_scd_insert>" + Environment.NewLine +
           "  ,[VALID_FROM]" + Environment.NewLine +
           "  ,[VALID_TO]" + Environment.NewLine +
           "  ,[IsActive]" + Environment.NewLine +
           ")" + Environment.NewLine +

           "SELECT Action_Out , <FK_ID> <bk_select_from_merge><attributes_insert>, VALID_FROM, VALID_TO, IsActive" + Environment.NewLine +
           "FROM" + Environment.NewLine +

           "( MERGE <scd_tablename> A" + Environment.NewLine +
           "USING #<tempTableName_scd> MR" + Environment.NewLine +
           "ON (<merge_scd_bk_on>)" + Environment.NewLine +
           "WHEN NOT MATCHED THEN" + Environment.NewLine +
           "INSERT ([MergeAction] ,<FK_ID> <bk_select_from_merge><attributes_scd_mergeinsert>, [VALID_FROM],[VALID_TO],[IsActive])" + Environment.NewLine +
           "VALUES (" + Environment.NewLine +
           "  'INSERT'" + Environment.NewLine +
           "  ,MR.<FK_ID>" + Environment.NewLine +
           "<bk_values>" +
           "<attributes_values>" +
           "  ,<valid_from_values>" + Environment.NewLine +
           "  ,99999999" + Environment.NewLine +
           "  ,1)" + Environment.NewLine +
           "WHEN MATCHED AND A.IsActive = 1" + Environment.NewLine +
           "AND (<attributes_wehre>) THEN" + Environment.NewLine +
           "UPDATE SET A.IsActive = 0, A.VALID_TO = convert(char(10), getdate()-1, 112)" + Environment.NewLine +
           "OUTPUT " + Environment.NewLine +
           "  $Action Action_Out" + Environment.NewLine +
           "  ,MR.<FK_ID>" + Environment.NewLine +
           "<bk_values>" +
           "<attributes_output>" +
           "  ,<valid_from_output> VALID_FROM" + Environment.NewLine +
           "  ,99999999 VALID_TO" + Environment.NewLine +
           "  ,1 IsActive" + Environment.NewLine +
           ") AS MERGE_OUT" + Environment.NewLine +
           "WHERE MERGE_OUT.Action_Out = 'UPDATE'";


        public string TableName { get; set; }
        public List<SCDCOlumn> AttributeList { get; set; }
        public List<SCDCOlumn> BkList { get; set; }
        public List<SCDCOlumn> BkAsFkList { get; set; }
        public SCDCOlumn ValidFrom { get; set; }
        public string PrefixFK { get; set; }

        public SCD()
        {
            AttributeList = new List<SCDCOlumn>();
            BkList = new List<SCDCOlumn>();
        }

        public bool IsValid(ref string message)
        {
            message = "";

            if (AttributeList.Count == 0)
            {
                message += "The Configuration for SCD table " + TableName + " needs at least 1 attribute.";
            }

            if (ValidFrom == null)
            {
                if (message != "") message += Environment.NewLine;
                message += "The Configuration for SCD table " + TableName + " needs a VALID_FROM column.";
            }

            return message == "";
        }

        private string GetInsertPart(string tempTableScd)
        {
            string result = SCD.TEMPLATE_INSERT;
            result = result.Replace("<scd_tablename>", TableName);
            result = result.Replace("<tempTableName_scd>", tempTableScd);
            result = result.Replace("<attributes_insert>", SCDHelper.GetSqlAttributeList(AttributeList, "", POSTFIX_COLUMN_QUELLE, 0));
            result = result.Replace("<attributes_scd_insert>", SCDHelper.GetSqlAttributeList(AttributeList, "", "", 2));
            result = result.Replace("<attributes_scd_mergeinsert>", SCDHelper.GetSqlAttributeList(AttributeList, "", "", 0));
            result = result.Replace("<attributes_values>", SCDHelper.GetSqlAttributeList(AttributeList, "MR.", POSTFIX_COLUMN_QUELLE, 2));
            result = result.Replace("<valid_from_values>", SCDHelper.GetSqlValidFrom(TableName, ValidFrom, "MR.", "", 0));
            result = result.Replace("<attributes_wehre>", SCDHelper.GetSqlAttributeWhere(AttributeList, "MR."));
            result = result.Replace("<attributes_output>", SCDHelper.GetSqlAttributeList(AttributeList, "MR.", POSTFIX_COLUMN_QUELLE, 2));
            result = result.Replace("<valid_from_output>", SCDHelper.GetSqlValidFrom(TableName, ValidFrom, "MR.", "", 0));
            result = result.Replace("<bk_scd>", SCDHelper.GetSqlBkList(BkList, PrefixFK, "", 2));
            result = result.Replace("<bk_select_from_merge>", SCDHelper.GetSqlBkList(BkList,PrefixFK, "", 0));
            result = result.Replace("<bk_values>", SCDHelper.GetSqlBkList(BkList,PrefixFK, "MR.", 2));
            result = result.Replace("<merge_scd_bk_on>", SCDHelper.GetBkOnList(BkList, PrefixFK));
            result = result.Replace("<FK_ID>", PrefixFK + "ID");

            return result;
        }

        public string GetMergeStatement(string tempTableScd)
        {
            string result = "";

            //result += GetOutputPart(tempTableScd) + Environment.NewLine + Environment.NewLine;
            result += GetInsertPart(tempTableScd) + Environment.NewLine + Environment.NewLine;

            return result;
        }

       
    }




    public class SCDCOlumn
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }

        public SCDCOlumn(string columnName, string dataType)
        {
            ColumnName = columnName;
            DataType = dataType;
        }
    }

    public class ScdColumnList
    {
        public List<SCDCOlumn> ScdColumns { get; set; }

        public ScdColumnList()
        {
            ScdColumns = new List<SCDCOlumn>();
        }

        public void AddList(List<SCDCOlumn> scdList)
        {
            foreach (SCDCOlumn newColumn in scdList)
            {
                if (!ContainsSCDColumn(newColumn)) ScdColumns.Add(newColumn);
            }
        }

        private bool ContainsSCDColumn(SCDCOlumn scdColumn)
        {
            foreach (SCDCOlumn col in ScdColumns)
            {
                if (col.ColumnName == scdColumn.ColumnName) return true;
            }

            return false;
        }
    }

}
