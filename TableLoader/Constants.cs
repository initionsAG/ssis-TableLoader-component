using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    public static class Constants
    {
        public const string PROP_CONFIG = "TableLoader Configuration";
        public const string PROP_GUID = "GUID";
        public const string PROP_ORIGINAL_GUID = "Original GUID";
        public const string CONNECTION_MANAGER_NAME_MAIN = "TableLoader ConnectionManager Main";
        public const string CONNECTION_MANAGER_NAME_BULK = "TableLoader ConnectionManager Bulk";
        public const string CONNECTION_MANAGER_NAME_CONFIG = "TableLoader ConnectionManager Config";
        public const string CONNECTION_MANAGER_OLEDB_NAME_CONFIG = "CN_CONFIG";
        public const string CONNECTION_MANAGER_ADO_NAME_CONFIG = "CN_CONFIG_ADO";
        public const string DB_TRANSACTION_NAME = "TableLoader Transaction";
        public const int DB_TIMEOUT_DEFAULT = 10000;
        public static readonly int REATTEMPTS_DEFAULT = 5;
        public const string SP_INSERT_BY_CURSOR = "spTempInsertByCursor";
        public const string SP_UPDATE_BY_CURSOR = "spTempUpdateByCursor";
        public const string INPUT_NAME = "input";
        public const string TEMP_TABLE_PLACEHOLDER = "<tempTableName>";
        public const string TEMP_TABLE_PLACEHOLDER_BRACKETS = "[" + TEMP_TABLE_PLACEHOLDER + "]";
        public const string TEMP_UPD_TABLE_PLACEHOLDER = "<Updated_Ids>";


        //Default Chunk Size
        //Chunk Size=Anzahl der Buffer-Zeilen die zusammen in die temporäre Tabelle geschrieben werden
        public const int CHUNK_SIZE_BULK_DEFAULT = 100000;
        public const int MAX_THREAD_COUNT_DEFAULT = 10;



        //SSIS Codes
        public const uint DTS_PIPELINE_CTR_ROWSREAD = 101;
        public const uint DTS_PIPELINE_CTR_ROWSWRITTEN = 103;
        public const uint DTS_PIPELINE_CTR_BLOBBYTESREAD = 116;
        public const uint DTS_PIPELINE_CTR_BLOBBYTESWRITTEN = 118;


        //On Information Event Codes
        public const int INFO_NONE = 0;
        public const int INFO_PRESQL = 1;
        public const int INFO_POSTSQL = 2;
        public const int INFO_MERGE = 3;
        public const int INFO_DELETE = 4;
        public const int INFO_INSERT = 5;
        public const int INFO_UPDATE = 6;
        public const int INFO_CREATE = 7;
        public const int INFO_SP = 8;

        public static readonly string[] INFO_NAME = {"NONE","PRE SQL", "POST SQL", "Merge", "Delete", "Insert", 
                                                     "Update", "Create", "Stored Procedure"};

        //Standard Configuration
        //public const string CFG_INSTANCE = "TL_CFG_INSTANCE"; //Variablenname
        //public const string CFG_DATABASE = "TL_CFG_CATALOG"; //Variablenname
        //public const string CFG_USER = "TL_CFG_USER"; //Variablenname
        //public const string CFG_PWD = "TL_CFG_PWD"; //Variablenname
        //public const string CFG_TABLE = "TL_CFG"; //TableName
        public const string CFG_TABLE_STANDARD = "TL_CFG_STD"; //TableName

        public static readonly string CREATE_INDEX = "CREATE NONCLUSTERED INDEX [idx_keys] ON <table> " +
            "(<columns> " +
            ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, " +
            "ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";


        

        #region Nur noch für PerformUpgrad notwendig

        //Property Namen
        public const string PROP_VERSION = "TableLoader Version";
        public const string PROP_MAPPING_CONFIGURATION = "MappingConfiguration";
        public const string PROP_MAPPING_CONFIGURATION_COUNT = "MappingConfigurationCount";
        public const string PROP_DEST_TABLE = "DestinationTable";
        public const string PROP_TEMP_TABLE = "TempTable";
        public const string PROP_CON_MANAGER_ID = "conManagerID";
        public const string PROP_CHUNK_SIZE_BULK = "ChunkSizeBulk";
        public const string PROP_CHUNK_SIZE_DB_COMMAND = "ChunkSizeDbCommand";
        public const string PROP_DB_TIMEOUT = "DB_Timeout";
        public const string PROP_PREFIX_INPUT = "Prefix Input";
        public const string PROP_PREFIX_OUTPUT = "Prefix Output";
        public const string PROP_POST_SQL = "Pre SQL";
        public const string PROP_PRE_SQL = "Post SQL";
        public const string PROP_DB_COMMAND = "DB Command";
        public const string PROP_USE_OWN_TRANSACTION = "Use Own Transaction";
        public const string PROP_CUSTOM_MERGE_COMMAND = "Custom Merge Command";


        //Mapping Konstanten 
        //Je Spalte wird ein Konfigurations-Array genutzt.
        //Die idx-Konstanten geben die Position einer Einstellung innerhalb des Arrays an.
        public const int MAPPING_IDX_USE_INSERT = 0;
        public const int MAPPING_IDX_USE_UPDATE = 1;
        public const int MAPPING_IDX_KEY = 2;
        public const int MAPPING_IDX_INPUT_COL_NAME = 3;
        public const int MAPPING_IDX_OUTPUT_COL_NAME = 4;
        public const int MAPPING_IDX_DEFAULT = 5;
        public const int MAPPING_IDX_FUNCTION = 6;
        public const int MAPPING_IDX_DATATYPE_INPUT = 7;
        public const int MAPPING_IDX_DATATYPE_OUTPUT = 8;
        public const int MAPPING_IDX_DATATYPE_OUTPUT_NET = 9;
        public const int MAPPING_IDX_OUTPUT_ID = 10;
        public const int MAPPING_IDX_OUTPUT_AUTOID = 11;
        public const int MAPPING_IDX_OUTPUT_NULL = 12;
        public const int MAPPING_IDX_INPUT_COLUMN_ID = 13;
        public const int MAPPING_COUNT = 14; //Anzahl Elemente der Mapping-Config

        //DB Command Types Komponente
        public const string DB_COMMAND_MERGE = "Merge (table based)";
        public const string DB_COMMAND_MERGE_2005 = "Merge (table based - SQL Server 2005)";
        public const string DB_COMMAND_UPDATE_TBL_INSERT_ROW_SP = "Update (table based) - Insert(row based)";
        public const string DB_COMMAND_UPDATE_ROW_INSERT_ROW_SP = "Update (row based) - Insert(row based)";
        public const string DB_COMMAND_BULK_INSERT = "Bulk Insert";
        public const string DB_COMMAND_INSERT = "Insert";

        //Transaction

        public const string TRANSACTION_INTERNAL = "intern";
        public const string TRANSACTION_EXTERNAL = "extern";
        public const string TRANSACTION_NONE = "keine";

        #endregion

    }


}
