﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Reflection;
using System.Diagnostics;

namespace TableLoader.Framework.Mapping {

    /// <summary>
    /// Since SSIS 2012 lineageIDs are not stored in the DTSX file, but still used at design- and runtime.
    /// Also lineage IDs may change if package is reopened. So mapping  for custom configurations for columns is achieved via GUIDS.
    /// 
    /// This componenet stores SSIS properties for columns in a single xml which is stored in a single custom property.
    /// To be able to map custom column properties to the SSIS column this component stores lineage IDs in the xml property.
    /// Because these lineage IDs may change when reopneing a package, GUIDs are used for the mapping.
    /// These GUIDs are stored as a custom property for SSIS columns and inside the xml.
    /// </summary>
    public class LineageMapping {
        /// <summary>
        /// The SSIS column custom property name. The property contains the GUIDs
        /// </summary>
        public static string IdPropertyName = "CustomID";

        /// <summary>
        /// Is this mapping necessary?
        /// </summary>
        /// <returns>Is this mapping necessary?</returns>
        public static bool NeedsMapping()
        {
#if     (SQL2008) 
            return false;      
#else
            return true;
#endif

        }

        #region Input

        /// <summary>
        /// Updates input columns custom properties (GUIDs)
        /// </summary>
        /// <param name="componentMetaData">the components metadata</param>
        /// <param name="isagCustomProperties">the components custom properties</param>
        public static void UpdateInputIdProperties(IDTSComponentMetaData100 componentMetaData, IsagCustomProperties isagCustomProperties)
        {
            if (!NeedsMapping())
                return;

            UpdateInputIdProperties(componentMetaData.InputCollection[0], componentMetaData, isagCustomProperties);
        }


        /// <summary>
        /// Updates input columns custom properties (GUIDs)
        /// </summary>
        /// <param name="input">the components input</param>
        /// <param name="componentMetaData">>the components metadata</param>
        /// <param name="isagCustomProperties">the components custom properties</param>
        private static void UpdateInputIdProperties(IDTSInput100 input, IDTSComponentMetaData100 componentMetaData, IsagCustomProperties isagCustomProperties)
        {
            foreach (IDTSInputColumn100 col in input.InputColumnCollection)
            {
                UpdateInputIdProperty(col, componentMetaData, isagCustomProperties);
            }
        }

        /// <summary>
        /// Updates input column custom property (GUIDs)
        /// </summary>
        /// <param name="col">the SSIS input column</param>
        /// <param name="componentMetaData">>the components metadata</param>
        /// <param name="isagCustomProperties">the components custom properties</param>
        private static void UpdateInputIdProperty(IDTSInputColumn100 col, IDTSComponentMetaData100 componentMetaData, IsagCustomProperties isagCustomProperties)
        {
            if (HasIdProperty(col.CustomPropertyCollection))
            {
                IDTSCustomProperty100 prop = col.CustomPropertyCollection[IdPropertyName];
                string guid = (string) col.CustomPropertyCollection[IdPropertyName].Value;

                foreach (ColumnConfig config in isagCustomProperties.ColumnConfigList)
                {
                    if (AreIdsEqual(config, guid))
                        UpdateColumnConfig(config, col, componentMetaData, isagCustomProperties);
                }
            }
        }

        /// <summary>
        /// Is the ColumnConfig's CustomId eqal to the gui`d?
        /// </summary>
        /// <param name="config">ColumnConfig</param>
        /// <param name="guid">GUID</param>
        /// <returns></returns>
        private static bool AreIdsEqual(ColumnConfig config, string guid)
        {
            return (config.CustomId == guid.ToString());
        }

        /// <summary>
        /// Updates a ColumnConfig and saves the components custom properties
        /// </summary>
        /// <param name="config">ColumnConfig</param>
        /// <param name="col">SSIS column</param>
        /// <param name="componentMetaData">componets metadata</param>
        /// <param name="isagCustomProperties">components custom properties</param>
        private static void UpdateColumnConfig(ColumnConfig config, IDTSInputColumn100 col, IDTSComponentMetaData100 componentMetaData, IsagCustomProperties isagCustomProperties)
        {
            config.InputColumnId = col.ID;
            isagCustomProperties.Save(componentMetaData);
        }



        #endregion

        /// <summary>
        /// Adds the custom property that contains the GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="propCollection">custom property collection of the input column</param>
        private static void AddIdProperty(string guid, IDTSCustomPropertyCollection100 propCollection)
        {
            IDTSCustomProperty100 prop = propCollection.New();
            prop.Name = IdPropertyName;
            prop.Value = guid;
        }

        /// <summary>
        /// Sets the GUID
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="propCollection">custom property collection of the input column</param>
        public static void SetIdProperty(string guid, IDTSCustomPropertyCollection100 propCollection)
        {
            if (!NeedsMapping())
                return;

            try
            {
                propCollection[IdPropertyName].Value = guid;
            }
            catch
            {
                AddIdProperty(guid, propCollection);
            }
        }

        /// <summary>
        /// Does the propCollection contain the custom property for GUIDs?
        /// </summary>
        /// <param name="propCollection">custom property collection of the input column</param>
        /// <returns>Does the propCollection contain the custom property for GUIDs?</returns>
        public static bool HasIdProperty(IDTSCustomPropertyCollection100 propCollection)
        {
            if (!NeedsMapping())
                return false;

            try
            {
                object value = propCollection[IdPropertyName].Value;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
