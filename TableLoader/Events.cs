using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace TableLoader
{

    public class IsagEvents
    {

        private IDTSComponentMetaData100 ComponentMetaData { get; set; }
        private IDTSVariableDispenser100 VariableDispenser { get; set; }
        private string DestinationTableName { get; set; }
        private string CustomEventValue { get; set; }
        private int _logLevel;


        public IsagEvents(IDTSComponentMetaData100 componentMetaData, IDTSVariableDispenser100 variableDispenser, string destinationTableName, string customEventTemplate, int logLEvel)
        {
            ComponentMetaData = componentMetaData;
            VariableDispenser = variableDispenser;
            DestinationTableName = destinationTableName;
            CustomEventValue = GetCustomEventValue(customEventTemplate);
            _logLevel = logLEvel;

        }


        public string GetCustomEventValue(string customEventTemplate)
        {
            try
            {
                if (customEventTemplate != null && customEventTemplate != "")
                {
                    while (customEventTemplate.Contains("@("))
                    {

                        int start = customEventTemplate.IndexOf("@(", 0);
                        int end = customEventTemplate.IndexOf(")", start);

                        string varName = customEventTemplate.Substring(start + 2, end - start - 2);

                        customEventTemplate = customEventTemplate.Replace("@(" + varName + ")", ComponentMetaDataTools.GetValueFromVariable(VariableDispenser, varName));
                    }
                }
            }
            catch (Exception ex)
            {
                Fire(IsagEventType.ErrorVariableNotFound, "[{0}]: Variable not found: {1}", new string[] { "LoggingCustomValue", ex.Message });
                throw ex;
            }

            return customEventTemplate;
        }

        private enum SSIS_EventType
        {
            Information = 0,
            Progress = 1,
            Warning = 2,
            Error = 3
        }


        public void Fire(IsagEventType eventType, string description)
        {
            Fire(eventType, description, new string[0]);
        }
        public void Fire(IsagEventType eventType, string description, string parameter)
        {
            Fire(eventType, description, new string[] { parameter });
        }

        public void FireError(string[] parameter)
        {
            string description = "Error in [";

            for (int i = 0; i < parameter.Length - 1; i++)
            {
                if (i > 0) description += ":";
                description += "{" + i + "}";
            }

            description += "]: {" + (parameter.Length - 1).ToString() + "}";

            Fire(IsagEventType.Error, description, parameter);
        }

        public void Fire(IsagEventType eventType, string description, string[] parameter)
        {
            if (!DoLog(eventType)) return; //Aufgrund des LogLevels nicht loggen

            bool cancel = false;

            description = string.Format(description, parameter);

            description += " | " + DestinationTableName + " | " + CustomEventValue;

            foreach (string param in parameter)
            {
                description += " | " + param;
            }

            switch (GetSSISEventType(eventType))
            {
                case SSIS_EventType.Information:
                    ComponentMetaData.FireInformation((int)eventType, ComponentMetaData.Name, description, "", 0, ref cancel);
                    break;
                case SSIS_EventType.Progress:
                    throw new NotImplementedException("Progress messages are not implemented");
                case SSIS_EventType.Warning:
                    ComponentMetaData.FireWarning((int)eventType, ComponentMetaData.Name, description, string.Empty, 0);
                    break;
                case SSIS_EventType.Error:
                    ComponentMetaData.FireError((int)eventType, ComponentMetaData.Name, description, string.Empty, 0, out cancel);
                    break;
                default:
                    ComponentMetaData.FireError((int)eventType, ComponentMetaData.Name, description, string.Empty, 0, out cancel);
                    break;
            }
        }

        private bool DoLog(IsagEventType errorType)
        {
            int code = (int)errorType;

            if (_logLevel == 3 || code >= 170) return true;
            else if (_logLevel == 2 && code < 140) return true;
            else if (_logLevel == 1 && code < 110) return true;
            else return false;
        }

        private static SSIS_EventType GetSSISEventType(IsagEventType type)
        {
            int n = (int)type;
            if (n >= 170 && n < 180) return SSIS_EventType.Error;
            else if (n >= 180) return SSIS_EventType.Warning;
            else return SSIS_EventType.Information;
        }

        public enum IsagEventType
        {
            //LogLevel = 1
            Sql = 100,
            BulkInsert = 101,
            SP = 102,
            Status = 103,
            // LogLevel = 2
            TransactionBegin = 110,
            TransactionCommit = 111,
            TransactionRollback = 116,
            TempTableCreate = 113,
            TempTableDrop = 117,
            PreSql = 120,
            PostSql = 121,
            SP_StartCreate = 126,
            SP_Drop = 127,
            //LogLevel = 3
            MergeBegin = 140,
            MergeEnd = 141,
            Merge2005Begin = 142,
            Merge2005End = 143,
            UpdateTblInsertRowBegin = 144,
            UpdateTblInsertRowEnd = 145,
            UpdateRowInsertRowBegin = 146,
            UpdateRowInsertRowEnd = 147,
            BulkInsertBegin = 148,
            BulkInsertEnd = 149,
            InsertBegin = 150,
            InsertEnd = 151,

            TempTableTruncate = 160,
            SP_FinishedCreate = 165,
            SP_ExecBegin = 166,
            SP_ExecFinished = 167,
            //Errrors
            Error = 170,
            ErrorWrongConnection = 171,
            ErrorConnectionNotInitialized = 172,
            ErrorMaxRowCountExeeded = 173,
            ErrorVariableNotFound = 174,
            ErrorSql = 175,

            //Warnings
            Warning = 190
        }
    }
}
