using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TableLoader.SCD
{



    /// <summary>
    /// SCD (slowly changing dimensions) configuration
    /// </summary>
    public class SCDConfiguration
    {
        /// <summary>
        /// Postfix for source column
        /// </summary>
        public static string POSTFIX_COLUMN_QUELLE = "_Quelle";

        /// <summary>
        /// Postfix for column containing the value before column has been updated
        /// </summary>
        public static string POSTFIX_COLUMN_DWH_VOR_UPDATE = "_DWH_Vor_Update";

        /// <summary>
        /// Insert statement for SCD table
        /// </summary>
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
           "  ,<GranularityMaxValue>" + Environment.NewLine +
           "  ,1)" + Environment.NewLine +
           "WHEN MATCHED AND A.IsActive = 1" + Environment.NewLine +
           "AND (<attributes_wehre>) THEN" + Environment.NewLine +
           "UPDATE SET A.IsActive = 0, A.VALID_TO = convert(char(10), CAST(<valid_from_output> AS BIGINT)-1)" + Environment.NewLine +
           "OUTPUT " + Environment.NewLine +
           "  $Action Action_Out" + Environment.NewLine +
           "  ,MR.<FK_ID>" + Environment.NewLine +
           "<bk_values>" +
           "<attributes_output>" +
           "  ,<valid_from_output> VALID_FROM" + Environment.NewLine +
           "  ,<GranularityMaxValue> VALID_TO" + Environment.NewLine +
           "  ,1 IsActive" + Environment.NewLine +
           ") AS MERGE_OUT" + Environment.NewLine +
           "WHERE MERGE_OUT.Action_Out = 'UPDATE'";


        /// <summary>
        /// SCD table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Attribute column list
        /// </summary>
        public List<SCDColumn> AttributeList { get; set; }

        /// <summary>
        /// BK column list
        /// </summary>
        public List<SCDColumn> BkList { get; set; }

        /// <summary>
        /// Foreign Key column list
        /// </summary>
        public List<SCDColumn> BkAsFkList { get; set; }

        /// <summary>
        /// Valif from column list
        /// </summary>
        public SCDColumn ValidFrom { get; set; }

        /// <summary>
        /// Foreign Key column prefix
        /// </summary>
        public string PrefixFK { get; set; }

        /// <summary>
        /// GranularityMaxValue (used for valid_to, means data is currentyl valid/used)
        /// </summary>
        public string GranularityMaxValue { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public SCDConfiguration(ColumnConfig.ScdTimeStampGranularityType timeStampGranularity)
        {
            AttributeList = new List<SCDColumn>();
            BkList = new List<SCDColumn>();
            SetranularityMaxValue(timeStampGranularity);
        }

        /// <summary>
        /// Set max value for timestamp granularity.
        /// If granulartity is YYYYMMDD, it is set to 99999999 (= length(YYYYMMDD) nines)
        /// </summary>
        /// <param name="timeStampGranularity"></param>
        private void SetranularityMaxValue(ColumnConfig.ScdTimeStampGranularityType timeStampGranularity)
        {
            GranularityMaxValue = string.Empty;
            if (timeStampGranularity != ColumnConfig.ScdTimeStampGranularityType.None)
            {
                for (int i = 0; i < timeStampGranularity.ToString().Length; i++)
                {
                    GranularityMaxValue += "9";
                }
            }
        }

        /// <summary>
        /// Is configuration valid?
        /// </summary>
        /// <param name="message">Errors in a SCD configuration will be written into this ref parameter</param>
        /// <returns>Is configuration valid?</returns>
        public bool IsValid(ref string message)
        {
            message = "";

            if (AttributeList.Count == 0)
            {
                message += "The Configuration for SCD table " + TableName + " needs at least 1 attribute.";
            }

            if (ValidFrom == null)
            {
                if (message != "")
                    message += Environment.NewLine;
                message += "The Configuration for SCD table " + TableName + " needs a VALID_FROM column.";
            }

            return message == "";
        }

        /// <summary>
        /// Get insert part of SCD merge command
        /// </summary>
        /// <param name="tempTableScd">SCD temporary table</param>
        /// <returns>Insert part of SCD sql command</returns>
        private string GetInsertPart(string tempTableScd)
        {
            string result = SCDConfiguration.TEMPLATE_INSERT;
            result = result.Replace("<GranularityMaxValue>", GranularityMaxValue);
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
            result = result.Replace("<bk_select_from_merge>", SCDHelper.GetSqlBkList(BkList, PrefixFK, "", 0));
            result = result.Replace("<bk_values>", SCDHelper.GetSqlBkList(BkList, PrefixFK, "MR.", 2));
            result = result.Replace("<merge_scd_bk_on>", SCDHelper.GetBkOnList(BkList, PrefixFK));
            result = result.Replace("<FK_ID>", PrefixFK + "ID");

            return result;
        }

        /// <summary>
        /// Get SCD sql merge command
        /// </summary>
        /// <param name="tempTableScd">SCD temporary table</param>
        /// <returns>SCD sql merge command</returns>
        public string GetMergeStatement(string tempTableScd)
        {
            string result = "";

            result += GetInsertPart(tempTableScd) + Environment.NewLine + Environment.NewLine;

            return result;
        }


    }







}
