using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Diagnostics;

namespace TableLoader
{
    public class CustomProperties
    {

        public string version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
                return fvi.FileVersion;
                //return String.Format("{0}.{1}", fvi.ProductMajorPart, fvi.ProductMinorPart);
            }
        }
        public string propVersion
        {
            get { return ComponentMetaData.CustomPropertyCollection[Constants.PROP_VERSION].Value.ToString(); }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_VERSION].Value = value; }
        }

        private IDTSComponentMetaData100 _componentMetaData;
        public IDTSComponentMetaData100 ComponentMetaData
        {
            get { return _componentMetaData; }
            set { _componentMetaData = value; }
        }


        #region Properties

        public string CustomMergeCommand
        {
            get { return ComponentMetaData.CustomPropertyCollection[Constants.PROP_CUSTOM_MERGE_COMMAND].Value.ToString(); }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_CUSTOM_MERGE_COMMAND].Value = value; }
        }

        public string TempTableName(string guid)
        {
            return "[##tempTable" + guid + "]";
        }
        public string DestTableName
        {
            get { return ComponentMetaData.CustomPropertyCollection[Constants.PROP_DEST_TABLE].Value.ToString(); }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_DEST_TABLE].Value = value; }
        }

        public long ChunkSizeBulk
        {
            get { return Convert.ToInt64(ComponentMetaData.CustomPropertyCollection[Constants.PROP_CHUNK_SIZE_BULK].Value); }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_CHUNK_SIZE_BULK].Value = value; }
        }
        public void SetChunkSizeBulk(object value)
        {
            try
            {
                ChunkSizeBulk = Convert.ToInt64(value);
            }
            catch (Exception)
            {
                MessageBox.Show("Ungültige Chunk Size (Bulk), es wird " + Constants.CHUNK_SIZE_BULK_DEFAULT + " eingesetzt.");
                ChunkSizeBulk = Constants.CHUNK_SIZE_BULK_DEFAULT;
            }
        }

        public long ChunkSizeDbCommand
        {
            get { return Convert.ToInt64(ComponentMetaData.CustomPropertyCollection[Constants.PROP_CHUNK_SIZE_DB_COMMAND].Value); }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_CHUNK_SIZE_DB_COMMAND].Value = value; }
        }
        public void SetChunkSizeDbCommand(object value)
        {
            try
            {
                ChunkSizeDbCommand = Convert.ToInt64(value);
            }
            catch (Exception)
            {
                MessageBox.Show("Ungültige Chunk Size (DB Command), es wird " + Constants.CHUNK_SIZE_DB_COMMAND_DEFAULT + " eingesetzt.");
                ChunkSizeDbCommand = Constants.CHUNK_SIZE_DB_COMMAND_DEFAULT;
            }
        }

        public string DBCommand
        {
            get
            {
                return ComponentMetaData.CustomPropertyCollection[Constants.PROP_DB_COMMAND].Value.ToString();
            }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_DB_COMMAND].Value = value; }
        }

        public int DBTimeout
        {
            get { return (int)ComponentMetaData.CustomPropertyCollection[Constants.PROP_DB_TIMEOUT].Value; }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_DB_TIMEOUT].Value = value; }
        }
        public void SetDBTimeout(object value)
        {
            try
            {
                DBTimeout = Convert.ToInt32(value);
            }
            catch (Exception)
            {
                MessageBox.Show("Ungültiger DB Timeout wert, es wird " + Constants.DB_TIMEOUT_DEFAULT + " eingesetzt.");
                DBTimeout = Constants.DB_TIMEOUT_DEFAULT;
            }
        }

        public string PostSQL
        {
            get
            {
                return ComponentMetaData.CustomPropertyCollection[Constants.PROP_POST_SQL].Value.ToString();
            }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_POST_SQL].Value = value; }
        }
        public string PreSQL
        {
            get
            {
                return ComponentMetaData.CustomPropertyCollection[Constants.PROP_PRE_SQL].Value.ToString();
            }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_PRE_SQL].Value = value; }
        }

        public string PrefixInput
        {
            get
            {
                if (ComponentMetaData.CustomPropertyCollection[Constants.PROP_PREFIX_INPUT].Value == null) return null;
                else return ComponentMetaData.CustomPropertyCollection[Constants.PROP_PREFIX_INPUT].Value.ToString();
            }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_PREFIX_INPUT].Value = value; }
        }
        public string PrefixOutput
        {
            get
            {
                if (ComponentMetaData.CustomPropertyCollection[Constants.PROP_PREFIX_OUTPUT].Value == null) return "";
                else return ComponentMetaData.CustomPropertyCollection[Constants.PROP_PREFIX_OUTPUT].Value.ToString();
            }
            set
            {
                ComponentMetaData.CustomPropertyCollection[Constants.PROP_PREFIX_OUTPUT].Value = value;
            }
        }


        public string Transaction
        {
            get
            {
                if (ComponentMetaData.CustomPropertyCollection[Constants.PROP_USE_OWN_TRANSACTION].Value == null) return Constants.TRANSACTION_INTERNAL;
                else return ComponentMetaData.CustomPropertyCollection[Constants.PROP_USE_OWN_TRANSACTION].Value.ToString();
            }

            set
            {
                ComponentMetaData.CustomPropertyCollection[Constants.PROP_USE_OWN_TRANSACTION].Value = value;
            }

        }
        //public bool UseOwnTransaction
        //{
        //    get
        //    {
        //        if (ComponentMetaData.CustomPropertyCollection[Constants.PROP_USE_OWN_TRANSACTION].Value == null) return true;
        //        else return (bool)ComponentMetaData.CustomPropertyCollection[Constants.PROP_USE_OWN_TRANSACTION].Value;
        //    }
        //    set
        //    {
        //        ComponentMetaData.CustomPropertyCollection[Constants.PROP_USE_OWN_TRANSACTION].Value = value;
        //    }
        //}

        #endregion

        #region Mappings

        public object[] GetMappingRow(int index)
        {
            return (object[])ComponentMetaData.CustomPropertyCollection[Constants.PROP_MAPPING_CONFIGURATION + index.ToString()].Value;
        }

        public void UpdateMappingRow(int index, object[] row)
        {
            ComponentMetaData.CustomPropertyCollection[Constants.PROP_MAPPING_CONFIGURATION + index.ToString()].Value = row;
        }

        public int MappingRowCount
        {
            get { return (int)ComponentMetaData.CustomPropertyCollection[Constants.PROP_MAPPING_CONFIGURATION_COUNT].Value; }
            set { ComponentMetaData.CustomPropertyCollection[Constants.PROP_MAPPING_CONFIGURATION_COUNT].Value = value; }
        }

        public void RemoveMappings()
        {
            RemoveMappings(Constants.PROP_MAPPING_CONFIGURATION, Constants.PROP_MAPPING_CONFIGURATION_COUNT,
                             MappingRowCount);
        }

        private void RemoveMappings(string propName, string propRowCountName, int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                RemovePropertyByName(propName + i.ToString());
            }

            ComponentMetaData.CustomPropertyCollection[propRowCountName].Value = 0;
        }

        public void AddMappingProperty(object value)
        {

            AddMappingProperty(Constants.PROP_MAPPING_CONFIGURATION, Constants.PROP_MAPPING_CONFIGURATION_COUNT,
                        MappingRowCount, value);
        }

        private void AddMappingProperty(string propName, string rowCountName, int rowCount, object value)
        {
            IDTSCustomProperty100 prop = ComponentMetaData.CustomPropertyCollection.New();
            prop.Name = propName + rowCount.ToString();
            prop.Value = value;

            prop = ComponentMetaData.CustomPropertyCollection[rowCountName];
            prop.Value = rowCount + 1;
        }

        public void RebuildMappings(IDTSVirtualInputColumnCollection100 virtualInputColumns, IDTSInputColumnCollection100 inputColumns)
        {
            Hashtable mappingsInput = new Hashtable();
            List<object[]> mappingsWithoutInputs = new List<object[]>();
            List<object[]> newMappings = new List<object[]>();

            //Speichern der bisherigen Mappings in 2 Listen (1x mit Input-Mapping, 1x ohne)
            for (int i = 0; i < this.MappingRowCount; i++)
            {
                object[] row = this.GetMappingRow(i);


                if (row[Constants.MAPPING_IDX_INPUT_COL_NAME] != null &&
                    row[Constants.MAPPING_IDX_INPUT_COL_NAME].ToString() != "")
                    mappingsInput.Add(row[Constants.MAPPING_IDX_INPUT_COLUMN_ID], row);
                else mappingsWithoutInputs.Add(row);

            }


            //Generieren von neuen MappingRows anhand der InputColumns
            foreach (IDTSInputColumn100 inputCol in inputColumns)
            {
                object[] row;

                if (mappingsInput.Contains(inputCol.ID))
                {
                    row = (object[])mappingsInput[inputCol.ID];
                    row[Constants.MAPPING_IDX_INPUT_COL_NAME] = inputCol.Name;
                    row[Constants.MAPPING_IDX_DATATYPE_INPUT] = SqlCreator.GetSQLServerDataTypeFromInput(inputCol);//inputCol.DataType.ToString();
                }
                else
                {
                    row = new object[Constants.MAPPING_COUNT];

                    row[Constants.MAPPING_IDX_USE_INSERT] = true;
                    row[Constants.MAPPING_IDX_USE_UPDATE] = true;
                    row[Constants.MAPPING_IDX_KEY] = false;
                    row[Constants.MAPPING_IDX_INPUT_COL_NAME] = inputCol.Name;
                    row[Constants.MAPPING_IDX_OUTPUT_COL_NAME] = Constants.MAPPING_NOT_ASSIGNED;
                    row[Constants.MAPPING_IDX_DEFAULT] = "";
                    row[Constants.MAPPING_IDX_DATATYPE_INPUT] = SqlCreator.GetSQLServerDataTypeFromInput(inputCol);//inputCol.DataType.ToString();
                    row[Constants.MAPPING_IDX_INPUT_COLUMN_ID] = inputCol.ID;
                }

                newMappings.Add(row);
            }

            //Aufbauen der neuen Mapping Properties
            this.RemoveMappings();

            foreach (object[] row in newMappings)
            {
                this.AddMappingProperty(row);
            }
            foreach (object[] row in mappingsWithoutInputs)
            {
                this.AddMappingProperty(row);
            }
        }
        #endregion

        #region Helper

        public void RemovePropertyByName(string name)
        {
            int ID = (int)ComponentMetaData.CustomPropertyCollection[name].ID;
            ComponentMetaData.CustomPropertyCollection.RemoveObjectByID(ID);
        }

        public bool getMappingBoolValue(int rowIndex, int colIndex)
        {
            object result = GetMappingRow(rowIndex)[colIndex];
            if (result == null) result = true;

            return (bool)result;
        }

        public bool isInputColumnUsed(string name)
        {
            object[] row = GetMappingRowByInputColumnName(name);

            if (row != null)
            {

                return ((bool)row[Constants.MAPPING_IDX_USE_INSERT] ||
                        (bool)row[Constants.MAPPING_IDX_USE_UPDATE] ||
                        (bool)row[Constants.MAPPING_IDX_KEY]);
            }

            return false;
        }

        private object[] GetMappingRowByInputColumnName(string name)
        {

            for (int i = 0; i < MappingRowCount; i++)
            {
                object[] row = GetMappingRow(i);

                if (row[Constants.MAPPING_IDX_INPUT_COL_NAME].ToString() == name) return row;
            }

            return null;
        }

        public string GetOutputColumnNameByInputColumnName(string name)
        {
            object[] row = GetMappingRowByInputColumnName(name);

            if (row != null) return row[Constants.MAPPING_IDX_OUTPUT_COL_NAME].ToString();

            return "";

        }
        #endregion

        public bool UseExternalTransaction
        {
            get
            {
                return (Transaction == Constants.TRANSACTION_EXTERNAL);
            }

        }

        public bool UseInternalTransaction
        {
            get
            {
                return (Transaction == Constants.TRANSACTION_INTERNAL);
            }

        }
    }
}
