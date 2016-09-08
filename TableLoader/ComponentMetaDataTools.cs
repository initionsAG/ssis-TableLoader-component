using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace TableLoader
{
    /// <summary>
    /// Helper methods for SSIS API
    /// </summary>
    public class ComponentMetaDataTools
    {
        /// <summary>
        /// Set usage type of all virtual input columns to readonly
        /// </summary>
        /// <param name="virtualInput">SSIS virtual input</param>
        public static void SetUsageTypeReadOnly(IDTSVirtualInput100 virtualInput)
        {
            for (int i = 0; i < virtualInput.VirtualInputColumnCollection.Count; i++)
            {
                virtualInput.SetUsageType(virtualInput.VirtualInputColumnCollection[i].LineageID, DTSUsageType.UT_READONLY);
            }
        }
      
        /// <summary>
        /// Sets metadata version to assemblies current version
        /// </summary>
        /// <param name="component">pipeline component</param>
        /// <param name="componentMetaData">components metdadata</param>
        public static void UpdateVersion(PipelineComponent component, IDTSComponentMetaData100 componentMetaData)
        {
            DtsPipelineComponentAttribute componentAttr =
                 (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(component.GetType(), typeof(DtsPipelineComponentAttribute), false);
            int binaryVersion = componentAttr.CurrentVersion;
            componentMetaData.Version = binaryVersion;
        }    

        /// <summary>
        /// Return value of SSIS variable
        /// </summary>
        /// <param name="variableDispenser">SSIS variable dispenser</param>
        /// <param name="variableName">variable name</param>
        /// <returns>variable value</returns>
        public static string GetValueFromVariable(IDTSVariableDispenser100 variableDispenser, string variableName)
        {
            string result;

            IDTSVariables100 var = null;
            variableDispenser.LockOneForRead(variableName, ref var);
            result = var[variableName].Value.ToString();
            var.Unlock();

            return result;
        }
        
       
    }
}
