﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Reflection;
using System.Diagnostics;

namespace TableLoader.ComponentFramework.Mapping
{
    /// <summary>
    /// Since SSIS 2012 lineageIDs are not stored in the DTSX file, but still used at design- and runtime.
    /// Also lineage IDs may change if package is reopened. So mapping  for custom configurations for columns is achieved via GUIDS.
    /// </summary>
    public class Mapping
    {
        public static string IdPropertyName = "CustomID";

        /// <summary>
        /// Returns if mapping is nececarry
        /// </summary>
        /// <returns></returns>
        public static bool NeedsMapping()
        {
#if     (SQL2008) 
            return false;      
#else 
            return true;
#endif

        }

        #region Input

        public static void UpdateInputIdProperties(IDTSComponentMetaData100 _componentMetaData, IsagCustomProperties _isagCustomProperties)
        {
            if (!NeedsMapping())
                return;

            UpdateInputIdProperties(_componentMetaData.InputCollection[0], _componentMetaData, _isagCustomProperties);
        }

        private static void UpdateInputIdProperties(IDTSInput100 input, IDTSComponentMetaData100 _componentMetaData, IsagCustomProperties _isagCustomProperties)
        {
            foreach (IDTSInputColumn100 col in input.InputColumnCollection)
            {
                UpdateInputIdProperty(col, _componentMetaData, _isagCustomProperties);
            }
        }

        private static void UpdateInputIdProperty(IDTSInputColumn100 col, IDTSComponentMetaData100 _componentMetaData, IsagCustomProperties _isagCustomProperties)
        {
            if (HasIdProperty(col.CustomPropertyCollection))
            {
                IDTSCustomProperty100 prop = col.CustomPropertyCollection[IdPropertyName];
                string guid = (string) col.CustomPropertyCollection[IdPropertyName].Value;

                foreach (ColumnConfig config in _isagCustomProperties.ColumnConfigList)
                {
                    if (AreIdsEqual(config, guid))
                        UpdateColumnConfig(config, col, _componentMetaData, _isagCustomProperties);
                }
            }
        }


        private static bool AreIdsEqual(ColumnConfig config, string guid)
        {
            return (config.CustomId == guid.ToString());
        }

        //TL spezifisch
        private static void UpdateColumnConfig(ColumnConfig config, IDTSInputColumn100 col, IDTSComponentMetaData100 _componentMetaData, IsagCustomProperties _isagCustomProperties)
        {
            config.InputColumnId = col.ID;
            _isagCustomProperties.Save(_componentMetaData);
        }



        #endregion

        private static void AddIdProperty(string ID, IDTSCustomPropertyCollection100 propCollection)
        {
            IDTSCustomProperty100 prop = propCollection.New();
            prop.Name = IdPropertyName;
            prop.Value = ID;
        }

        public static void SetIdProperty(string ID, IDTSCustomPropertyCollection100 propCollection)
        {
            if (!NeedsMapping())
                return;

            try
            {
                propCollection[IdPropertyName].Value = ID;
            }
            catch
            {
                AddIdProperty(ID, propCollection);
            }
        }

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