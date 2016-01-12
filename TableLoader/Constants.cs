using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    public static class Constants
    {
        /// <summary>
        /// SSIS custom property name for TableLoader configuration
        /// </summary>
        public const string PROP_CONFIG = "TableLoader Configuration";
   
        /// <summary>
        /// Main connectionmanager name
        /// </summary>
        public const string CONNECTION_MANAGER_NAME_MAIN = "TableLoader ConnectionManager Main";
        /// <summary>
        /// Bulk connectionmanager name
        /// </summary>
        public const string CONNECTION_MANAGER_NAME_BULK = "TableLoader ConnectionManager Bulk";
        /// <summary>
        /// Standard configuration connectionmanager name
        /// </summary>
        public const string CONNECTION_MANAGER_NAME_CONFIG = "TableLoader ConnectionManager Config";
        /// <summary>
        /// Henry OLE DB connection Manager name for CN_CONFIG
        /// </summary>
        public const string CONNECTION_MANAGER_OLEDB_NAME_CONFIG = "CN_CONFIG";
        /// <summary>
        /// Henry ADO.NET connection Manager name for CN_CONFIG_ADO
        /// </summary>
        public const string CONNECTION_MANAGER_ADO_NAME_CONFIG = "CN_CONFIG_ADO";

        /// <summary>
        /// Database transaction name
        /// </summary>
        public const string DB_TRANSACTION_NAME = "TableLoader Transaction";
        /// <summary>
        /// Default value for sql timeeout
        /// </summary>
        public const int DB_TIMEOUT_DEFAULT = 10000;
        /// <summary>
        /// Default value for number of reattempts (execute sql)
        /// </summary>
        public static readonly int REATTEMPTS_DEFAULT = 5;
        /// <summary>
        /// Stored procedure name for spTempInsertByCursor
        /// </summary>
        public const string SP_INSERT_BY_CURSOR = "spTempInsertByCursor";
        /// <summary>
        /// Stored procedure name for spTempUpdateByCursor
        /// </summary>
        public const string SP_UPDATE_BY_CURSOR = "spTempUpdateByCursor";
        /// <summary>
        /// SSIS input name
        /// </summary>
        public const string INPUT_NAME = "input";
        /// <summary>
        /// Placeholder (sql statements) for temporary table name
        /// </summary>
        public const string TEMP_TABLE_PLACEHOLDER = "<tempTableName>";
        /// <summary>
        /// Placeholder with brackets (sql statements) for temporary table name
        /// </summary>
        public const string TEMP_TABLE_PLACEHOLDER_BRACKETS = "[" + TEMP_TABLE_PLACEHOLDER + "]";

        /// <summary>
        /// Placeholder used for pseudo merge command (SQL Server 2005)
        /// </summary>
        public const string TEMP_UPD_TABLE_PLACEHOLDER = "<Updated_Ids>";


        //Default Chunk Size
        //Chunk Size=Number Buffer-Rows written to a temporary table
        public const int CHUNK_SIZE_BULK_DEFAULT = 100000;
        /// <summary>
        /// Maximum number of concurrent threads
        /// </summary>
        public const int MAX_THREAD_COUNT_DEFAULT = 10;



    


        /// <summary>
        /// Standard Configuration sql table name
        /// </summary>
        public const string CFG_TABLE_STANDARD = "TL_CFG_STD"; 

        /// <summary>
        /// Sql create statement template for temporary tables
        /// </summary>
        public static readonly string CREATE_INDEX = "CREATE UNIQUE NONCLUSTERED INDEX [idx_keys] ON <table> " +
            "(<columns> " +
            ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, " +
            "ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";


       

    }


}
