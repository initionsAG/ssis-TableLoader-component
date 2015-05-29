using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace TableLoader
{
    class TlDbCommand
    {
        private IsagCustomProperties _IsagCustomProperties;
        private IsagEvents _events;
        private IDTSComponentMetaData100 _componentMetaData;
        private IDTSVariableDispenser100 _variableDispenser;

        public TlDbCommand(IsagCustomProperties isagCustomProperties, IsagEvents events, 
                         IDTSComponentMetaData100 componentMetaData, IDTSVariableDispenser100 variableDispenser)
        {
            _IsagCustomProperties = isagCustomProperties;
            _events = events;
            _componentMetaData = componentMetaData;
            _variableDispenser = variableDispenser;
        }

        public void GetDbCommandDefinition(out IsagEvents.IsagEventType eventType, out string[] sqlTemplate)
        {
            switch (_IsagCustomProperties.DbCommand)
            {
                case IsagCustomProperties.DbCommandType.Merge:
                    eventType = IsagEvents.IsagEventType.MergeBegin;
                    sqlTemplate = new string[] { GetExecuteStatementFromTemplate(SqlCreator.GetSqlMerge(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS)) };
                    break;
                case IsagCustomProperties.DbCommandType.Merge2005:
                    eventType = IsagEvents.IsagEventType.Merge2005Begin;
                    sqlTemplate = new string[] { GetExecuteStatementFromTemplate(SqlCreator.GetSqlMerge2005(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS)) };
                    break;
                case IsagCustomProperties.DbCommandType.UpdateTblInsertRow:
                    eventType = IsagEvents.IsagEventType.UpdateTblInsertRowBegin;

                    string spInsertName = "[#" + Constants.SP_INSERT_BY_CURSOR + "_" + Guid.NewGuid().ToString() + "]";
                    string sqlSpInsert = SqlCreator.GetSqlInsertSP(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS).Replace("<SPName>", spInsertName);
                    string sqlUpdate = SqlCreator.GetSqlUpdate(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS);
                    string sqlExecSp = "EXEC " + spInsertName;
                    string sqlDropSpInsert = "DROP PROCEDURE " + spInsertName;

                    sqlTemplate = new string[] { sqlSpInsert, sqlUpdate, sqlExecSp };
                    break;
                case IsagCustomProperties.DbCommandType.UpdateRowInsertRow:
                    eventType = IsagEvents.IsagEventType.UpdateRowInsertRowBegin;

                    string spInsertName1 = "[#" + Constants.SP_INSERT_BY_CURSOR + "_" + _componentMetaData.Name + "_" + Guid.NewGuid().ToString() + "]";
                    string spUpdateName1 = "[#" + Constants.SP_UPDATE_BY_CURSOR + "_" + _componentMetaData.Name + "_" + Guid.NewGuid().ToString() + "]";
                    string sqlSpInsert1 = SqlCreator.GetSqlInsertSP(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS).Replace("<SPName>", spInsertName1);
                    string sqlSpUpdate1 = SqlCreator.GetSqlUpdateSP(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS).Replace("<SPName>", spUpdateName1);
                    string sqlExecInsertSp = "EXEC " + spInsertName1;
                    string sqlExecUpdateSp = "EXEC " + spUpdateName1;
                    string sqlDropSpInsert1 = "DROP PROCEDURE " + spInsertName1;
                    string sqlDropSpUpdate = "DROP PROCEDURE " + spUpdateName1;

                    sqlTemplate = new string[] { sqlSpInsert1, sqlSpUpdate1, sqlExecUpdateSp, sqlExecInsertSp };

                    break;
                case IsagCustomProperties.DbCommandType.Insert:
                    eventType = IsagEvents.IsagEventType.InsertBegin;
                    sqlTemplate = new string[] { GetExecuteStatementFromTemplate(SqlCreator.GetSqlInsert(_IsagCustomProperties, Constants.TEMP_TABLE_PLACEHOLDER_BRACKETS)) };
                    break;
                case IsagCustomProperties.DbCommandType.BulkInsert:
                    eventType = IsagEvents.IsagEventType.BulkInsert;
                    sqlTemplate = null;
                    break;
                case IsagCustomProperties.DbCommandType.BulkInsertRowLock:
                    eventType = IsagEvents.IsagEventType.BulkInsert;
                    sqlTemplate = null;
                    break;
                default:
                    _events.FireError(new string[] { "Es wurde kein gültiges DBCommand gewählt." });
                    throw new Exception("Es wurde kein gültiges DBCommand gewählt.");
            }
        }

        /// <summary>
        /// Die Pre- und PostSql Statements können Platzhalter für SSIS Variablen enthalten.
        /// Diese werden mit den aktuellen Werten der Variablen gefüllt.
        /// </summary>
        /// <param name="templateStatement">Das vom Benutzer eingegebene SQL Statement</param>
        /// <returns>Das ausführbare SQL Statement</returns>
        public string GetExecuteStatementFromTemplate(string templateStatement)
        {
            string result = templateStatement;
            string varName = "";

            try
            {
                if (result != "")
                {
                    while (result.Contains("@("))
                    {
                        IDTSVariables100 var = null;
                        int start = result.IndexOf("@(", 0);
                        int end = result.IndexOf(")", start);

                        varName = result.Substring(start + 2, end - start - 2);

                        _variableDispenser.LockOneForRead(varName, ref var);

                        result = result.Replace("@(" + varName + ")", var[varName].Value.ToString());
                        var.Unlock();
                    }
                }
            }
            catch (Exception ex)
            {
                _events.Fire(IsagEvents.IsagEventType.ErrorVariableNotFound, "[{0}]: Variable not found: {1}",
                            new string[] { "Pre-/PostSql", ex.Message });
                //Events.Fire(ComponentMetaData, Events.SSIS_EventType.Error, "Eine Variable im Pre-, bzw. PostSql Statement konnte nicht gefunden werden.");
            }
            return result;
        }
    }
}
