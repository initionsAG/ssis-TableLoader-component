using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace TableLoader.Log
{
    /// <summary>
    /// Handles SSIS Events
    /// </summary>
    public class IsagEvents
    {
        /// <summary>
        /// SSIS component metadata
        /// </summary>
        private IDTSComponentMetaData100 ComponentMetaData { get; set; }

        /// <summary>
        /// SSIS variable dispenser
        /// </summary>
        private IDTSVariableDispenser100 VariableDispenser { get; set; }

        /// <summary>
        /// destination table name
        /// </summary>
        private string DestinationTableName { get; set; }

        /// <summary>
        /// custom event value (event message text)
        /// </summary>
        private string CustomEventValue { get; set; }

        /// <summary>
        /// log level (1-3)
        /// </summary>
        private int _logLevel;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="componentMetaData">SSIS component metadata</param>
        /// <param name="variableDispenser">SSIS variable dispenser</param>
        /// <param name="destinationTableName">destination table name</param>
        /// <param name="customEventTemplate">template for the custom event</param>
        /// <param name="logLEvel">log level</param>
        public IsagEvents(IDTSComponentMetaData100 componentMetaData, IDTSVariableDispenser100 variableDispenser, string destinationTableName, string customEventTemplate, int logLEvel)
        {
            ComponentMetaData = componentMetaData;
            VariableDispenser = variableDispenser;
            DestinationTableName = destinationTableName;
            CustomEventValue = GetCustomEventValue(customEventTemplate);
            _logLevel = logLEvel;

        }

        /// <summary>
        /// Get custom event value from template
        /// (replaces variables with their values)
        /// </summary>
        /// <param name="customEventTemplate">custom event template</param>
        /// <returns>custom event value</returns>
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

        /// <summary>
        /// Event type 
        /// (determines how many events are fired)
        /// </summary>
        private enum SSIS_EventType
        {
            Information = 0,
            Progress = 1,
            Warning = 2,
            Error = 3
        }

        /// <summary>
        /// Fire event
        /// </summary>
        /// <param name="eventType">event type</param>
        /// <param name="description">event description</param>
        public void Fire(IsagEventType eventType, string description)
        {
            Fire(eventType, description, new string[0]);
        }

        /// <summary>
        /// Fire event
        /// </summary>
        /// <param name="eventType">event type</param>
        /// <param name="description">event description</param>
        /// <param name="parameter">event parameter</param>
        public void Fire(IsagEventType eventType, string description, string parameter)
        {
            Fire(eventType, description, new string[] { parameter });
        }

        /// <summary>
        /// Fire error event
        /// </summary>
        /// <param name="parameter">event parameter list</param>
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

        /// <summary>
        /// Fire event
        /// </summary>
        /// <param name="eventType">event type</param>
        /// <param name="description">event description</param>
        /// <param name="parameter">event parameter list</param>
        public void Fire(IsagEventType eventType, string description, string[] parameter)
        {
            if (!DoLog(eventType)) return; //loglevel disables logging

            bool cancel = false;

            //insert parameter into description
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

        /// <summary>
        /// Log event for the given error type and log level?
        /// </summary>
        /// <param name="errorType">error type and log level</param>
        /// <returns>Log event for the given error type?</returns>
        private bool DoLog(IsagEventType errorType)
        {
            int code = (int)errorType;

            if (_logLevel == 3 || code >= 170) return true;
            else if (_logLevel == 2 && code < 140) return true;
            else if (_logLevel == 1 && code < 110) return true;
            else return false;
        }

        /// <summary>
        /// Get SSIS event type by isag event type
        /// </summary>
        /// <param name="type">idag event type</param>
        /// <returns>SSIS event type</returns>
        private static SSIS_EventType GetSSISEventType(IsagEventType type)
        {
            int n = (int)type;
            if (n >= 170 && n < 180) return SSIS_EventType.Error;
            else if (n >= 180) return SSIS_EventType.Warning;
            else return SSIS_EventType.Information;
        }

        /// <summary>
        /// Isag event type
        /// </summary>
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
