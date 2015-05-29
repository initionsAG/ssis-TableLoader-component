using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace TableLoader
{
    public class ComponentMetaDataTools
    {
        public static IDTSInputColumn100 GetInputColumnByLineageId(IDTSInputColumnCollection100 inputColumns, int lineageId)
        {

            IDTSInputColumn100 result = null;

            for (int i = 0; i < inputColumns.Count; i++)
            {
                if (inputColumns[i].LineageID == lineageId) result = inputColumns[i];
            }

            return result;
        }

        public static IDTSInputColumn100 GetInputIdByColumnName(string columnName, IDTSComponentMetaData100 componentMetaData)
        {

            foreach (IDTSInputColumn100 column in componentMetaData.InputCollection[0].InputColumnCollection)
            {
                if (column.Name == columnName) return column;
            }

            return null;
        }

        public static IDTSOutputColumn100 GetOutputColumnByColumnName(string columnName, IDTSOutputColumnCollection100 outputColumns)
        {

            foreach (IDTSOutputColumn100 column in outputColumns)
            {
                if (column.Name == columnName) return column;
            }

            return null;
        }

        public static bool HasVirtualInputColumn(IDTSVirtualInput100 vInput, int lineageId)
        {
            bool result = false;

            foreach (IDTSVirtualInputColumn100 col in vInput.VirtualInputColumnCollection)
            {
                if (col.LineageID == lineageId) result = true;
            }

            return result;
        }

        public static void SetUsageTypeReadOnly(IDTSVirtualInput100 virtualInput)
        {
            for (int i = 0; i < virtualInput.VirtualInputColumnCollection.Count; i++)
            {
                virtualInput.SetUsageType(virtualInput.VirtualInputColumnCollection[i].LineageID, DTSUsageType.UT_READONLY);
            }
        }

        public static void RemoveCollections(IDTSComponentMetaData100 componentMetaData)
        {
            componentMetaData.InputCollection[0].InputColumnCollection.RemoveAll();
        }

        /// <summary>
        /// Metadaten Version auf DLL-Version setzen 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="componentMetaData"></param>
        public static void UpdateVersion(PipelineComponent component, IDTSComponentMetaData100 componentMetaData)
        {
            DtsPipelineComponentAttribute componentAttr =
                 (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(component.GetType(), typeof(DtsPipelineComponentAttribute), false);
            int binaryVersion = componentAttr.CurrentVersion;
            componentMetaData.Version = binaryVersion;
        }

        public static DataType GetDataType(string name)
        {
            switch (name)
            {
                case "DT_BOOL":
                    return DataType.DT_BOOL;
                case "DT_BYTES":
                    return DataType.DT_BYTES;
                case "DT_CY":
                    return DataType.DT_CY;
                case "DT_DATE":
                    return DataType.DT_DATE;
                case "DT_DBDATE":
                    return DataType.DT_DBDATE;
                case "DT_DBTIMESTAMP":
                    return DataType.DT_DBTIMESTAMP;
                case "DT_DECIMAL":
                    return DataType.DT_DECIMAL;
                case "DT_FILETIME":
                    return DataType.DT_FILETIME;
                case "DT_GUID":
                    return DataType.DT_GUID;
                case "DT_I1":
                    return DataType.DT_I1;
                case "DT_I2":
                    return DataType.DT_I2;
                case "DT_I4":
                    return DataType.DT_I4;
                case "DT_I8":
                    return DataType.DT_I8;
                case "DT_IMAGE":
                    return DataType.DT_IMAGE;
                case "DT_NTEXT":
                    return DataType.DT_NTEXT;
                case "DT_NULL":
                    return DataType.DT_NULL;
                case "DT_NUMERIC":
                    return DataType.DT_NUMERIC;
                case "DT_R4":
                    return DataType.DT_R4;
                case "DT_R8":
                    return DataType.DT_R8;
                case "DT_STR":
                    return DataType.DT_STR;
                case "DT_TEXT":
                    return DataType.DT_TEXT;
                case "DT_UI1":
                    return DataType.DT_UI1;
                case "DT_UI2":
                    return DataType.DT_UI2;
                case "DT_UI4":
                    return DataType.DT_UI4;
                case "DT_UI8":
                    return DataType.DT_UI8;
                case "DT_WSTR":
                    return DataType.DT_WSTR;
                default:
                    return DataType.DT_NULL; //Exception
            }
        }


        public static long GetPropertyLongValue(IDTSComponentMetaData100 componentMetaData, string propertyName, long defaultValue)
        {
            long result;
            try
            {
                result = Int64.Parse(componentMetaData.CustomPropertyCollection[propertyName].Value.ToString());
            }
            catch (Exception) { result = defaultValue; }

            return result;
        }

        public static string GetPropertyStringValue(IDTSComponentMetaData100 componentMetaData, string propertyName)
        {
            string result;
            try
            {
                result = componentMetaData.CustomPropertyCollection[propertyName].Value.ToString();
            }
            catch (Exception) { result = ""; }

            return result;
        }

        public static string GetPropertyStringValue(object[] row, int index)
        {
            string result;
            try
            {
                result = row[index].ToString();
            }
            catch (Exception) { result = ""; }

            return result;
        }

        public static bool GetPropertyBoolValue(object[] row, int index)
        {
            bool result;
            try
            {
                result = (bool)row[index];
            }
            catch (Exception) { result = false; }

            return result;
        }

        public static bool RemovePropertyByName(IDTSComponentMetaData100 componentMetaData, string name)
        {
            try
            {
                int ID = (int)componentMetaData.CustomPropertyCollection[name].ID;
                componentMetaData.CustomPropertyCollection.RemoveObjectByID(ID);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string GetValueFromVariable(IDTSVariableDispenser100 variableDispenser, string variableName)
        {
            string result;

            IDTSVariables100 var = null;
            variableDispenser.LockOneForRead(variableName, ref var);
            result = var[variableName].Value.ToString();
            var.Unlock();

            return result;
        }
        
        public static object GetValueFromBuffer(PipelineBuffer buffer, int bufferIndex, DataType dataType, IsagEvents events)
        {
            object result = null;

            if (buffer.IsNull(bufferIndex)) return null;
            
            switch (dataType)
            {
                case DataType.DT_BOOL:
                    result = buffer.GetBoolean(bufferIndex);
                    break;
                case DataType.DT_BYTES:
                    result = buffer.GetBytes(bufferIndex);
                    break;
                case DataType.DT_CY:
                    result = buffer.GetDecimal(bufferIndex);
                    break;
                case DataType.DT_DATE:
                    result = buffer.GetDateTime(bufferIndex);
                    break;
                case DataType.DT_DBDATE:
                    result = buffer.GetDateTime(bufferIndex);
                    break;
                case DataType.DT_DBTIME:
                    result = buffer.GetDateTime(bufferIndex);
                    break;
                case DataType.DT_DBTIMESTAMP:
                    result = buffer.GetDateTime(bufferIndex);
                    break;
                case DataType.DT_DECIMAL:
                    result = buffer.GetDecimal(bufferIndex);
                    break;
                case DataType.DT_FILETIME:
                    result = buffer[bufferIndex];
                    break;
                case DataType.DT_GUID:
                   result =  buffer.GetGuid(bufferIndex);
                    break;
                case DataType.DT_I1:
                    result =  buffer.GetSByte(bufferIndex);
                    break;
                case DataType.DT_I2:
                   result =  buffer.GetInt16(bufferIndex);
                    break;
                case DataType.DT_I4:
                    result = buffer.GetInt32(bufferIndex);
                    break;
                case DataType.DT_I8:
                    result = buffer.GetInt64(bufferIndex);
                    break;
                case DataType.DT_IMAGE:
                    result = buffer[bufferIndex];                   
                    break;
                case DataType.DT_NTEXT:
                    result = buffer.GetString(bufferIndex);                   
                    break;
                case DataType.DT_NULL:
                    result = null;
                    break;
                case DataType.DT_NUMERIC:
                    result = buffer.GetDecimal(bufferIndex);
                    break;
                case DataType.DT_R4:
                    buffer.GetSingle(bufferIndex);
                    break;
                case DataType.DT_R8:
                    buffer.GetDouble(bufferIndex);
                    break;
                case DataType.DT_STR:
                    result = buffer.GetString(bufferIndex);
                    break;
                case DataType.DT_TEXT:
                    result = buffer.GetString(bufferIndex);      
                    break;
                case DataType.DT_UI1:
                    result = buffer.GetByte(bufferIndex);
                    break;
                case DataType.DT_UI2:
                    result = buffer.GetUInt16(bufferIndex);
                    break;
                case DataType.DT_UI4:
                    result = buffer.GetUInt32(bufferIndex);
                    break;
                case DataType.DT_UI8:
                    result = buffer.GetUInt64(bufferIndex);
                    break;
                case DataType.DT_WSTR:
                    result = buffer.GetString(bufferIndex);
                    break;
                default:
                    events.FireError(new string[] {"Unsupported Datetype " + dataType.ToString()});
                    break;
            }

            return result;

        }
    }
}
