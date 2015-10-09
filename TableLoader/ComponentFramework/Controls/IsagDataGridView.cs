using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ComponentFramework.Controls
{
    /// <summary>
    /// Enhanced DataGridView that accepts a standard DataSource (i.e. DataTable) and a DataSource (BindingList<string>) for the ItemList for a combobox cell 
    /// If a cell value is not included in the ItemList, the cell value is added to the ItemList and shwon in red (as the value is invalid).
    /// 
    /// This DataGridView needs a standard DataSource. Then it is prossible to add a "bounded" combobox column for each column contained in the DataSource.
    /// The "bounded" combobox column is visible while the column created from the DataSource is invisible. If the text property of the combobox column cell changes 
    /// the value of the invisible cell also changes.
    /// 
    /// Example:
    /// idgvOutputColumns.DataSource = _IsagCustomProperties.OutputConfigList;
    /// (set the DataSource for the DataGridVíew)
    /// idgvOutputColumns.AddCellBoundedComboBox("SqlColumn", _sqlColumnList);
    /// (set _sqlColumnList as the datsource for the ItemList of the combobox column,
    /// the column name of the DataGridView is "SqlColumn")
    /// 
    /// </summary>
    public partial class IsagDataGridView : DataGridView
    {
        public enum ComboboxConfigType { MARK_INVALID = 0, DISABLE = 1 }

        /// <summary>
        /// DataSources for the ItemLists
        /// int: ColumnIndex in the DataGridView
        /// BindingList<string>: DataSource for the ItemList
        /// </summary>
        private Dictionary<int, ComboBoxConfiguration> _cmbItemSources = new Dictionary<int, ComboBoxConfiguration>();

        /// <summary>
        /// ComboBox Columns gets the name of the column that it is bound to pls this prefix
        /// </summary>
        public const string CMB_COLUMN_PREFIX = "BoundedCombo_";

        /// <summary>
        /// the constructor
        /// </summary>
        public IsagDataGridView()
        {
            InitializeComponent();
            this.AllowUserToAddRows = false;

            this.DataBindingComplete += IsagDataGridView_DataBindingComplete;
            this.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            this.ColumnHeaderMouseClick += IsagDataGridView_ColumnHeaderMouseClick;
        }


        /// <summary>
        /// Sort Datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IsagDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            bool sortComboBoxCell = _cmbItemSources.ContainsKey(e.ColumnIndex);

            //Get the sort column (if it is a columnbox, the column that is bounded to the datasource has to be sorted)
            DataGridViewColumn column = sortComboBoxCell ? 
                this.Columns[GetBoundedColumnIndex(e.ColumnIndex)] : this.Columns[e.ColumnIndex];                              


            //sort order is ascending, if columns last sort order has not been ascending
            System.Windows.Forms.SortOrder sortOrder = System.Windows.Forms.SortOrder.Ascending;
            ListSortDirection listSortOrder = ListSortDirection.Ascending;

            if (column.HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending)
            {
                listSortOrder = ListSortDirection.Descending;
                sortOrder = System.Windows.Forms.SortOrder.Descending;
            }

            // Remove SortGlyphs (necessary because of comboboxe cells)
            foreach (DataGridViewColumn col in this.Columns)
            {
                col.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.None;
            }

            //Glyphs is needed on the ComboBoxCell, sorting happens on the bounded cell
            if (sortComboBoxCell)
            {
                this.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.Programmatic;
                this.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;                
            }

            column.SortMode = DataGridViewColumnSortMode.Programmatic; 
            column.HeaderCell.SortGlyphDirection = sortOrder;
            this.Sort(column, listSortOrder);

            //Refresh comboboxes (displayed value is lost because comboBox cell is not direectly bounded to the datasource
            foreach (int colIdx in _cmbItemSources.Keys)
            {
                string columnName = this.Columns[GetBoundedColumnIndex(colIdx)].Name;
                RefreshCellBoundComboBox(columnName);
            }

        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsagDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.DataBindingComplete -= IsagDataGridView_DataBindingComplete;

            UpdateCellBoundComboBox();

            this.CellValueChanged += IsagDataGridView_CellValueChanged;
            this.EditingControlShowing += IsagDataGridView_EditingControlShowing;
            this.CurrentCellDirtyStateChanged += IsagDataGridView_CurrentCellDirtyStateChanged;

            //foreach (DataGridViewColumn column in this.Columns)
            //{
            //    column.SortMode = DataGridViewColumnSortMode.Programmatic;
            //    column.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
            //    this.Sort(column, ListSortDirection.Ascending);

            //}
        }

   

        /// <summary>
        /// Whenn a Combox is opened register to the DrawItem Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsagDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (_cmbItemSources.Keys.Contains(this.SelectedCells[0].ColumnIndex))
            {
                ComboBox cmb = (ComboBox)e.Control;

                if (cmb != null)
                {
                    cmb.DrawMode = DrawMode.OwnerDrawFixed;
                    cmb.DrawItem -= cmb_DrawItem;
                    cmb.DrawItem += cmb_DrawItem;

                    cmb.SelectedIndexChanged -= cmb_SelectedIndexChanged;
                    cmb.SelectedIndexChanged += cmb_SelectedIndexChanged;
                }
            }
        }

        void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            //Undo change as user has selected a disabled item
            if (_cmbItemSources[this.CurrentCell.ColumnIndex].IsDisabled(cmb.SelectedItem))
                cmb.SelectedItem = this.CurrentCell.Value;
        }

        /// <summary>
        /// Draws the items if a combobox is opend.
        /// Invalid Items (cell values that is not contained in the ItemSource) are marked red
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index != -1)
            {
                ComboBox cmb = (ComboBox)EditingControl;

                object itemValue = ((ComboBox)sender).Items[e.Index];
                Color color = _cmbItemSources[this.SelectedCells[0].ColumnIndex].GetForeColor(itemValue);

                using (var brush = new SolidBrush(color))
                    e.Graphics.DrawString(itemValue.ToString(), e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Commits the cells value change earlier
        /// This way the cell does not have to loose focus to trigger cell value change event.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsagDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.IsCurrentCellDirty) this.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>
        /// If a cell value has change the ItemList has to be updated.
        /// - invalid items (not included in the ItemSource but in the ItemList) has to be removed from the ItemList
        /// - if the cell value is not contained in the ItemList it has to be added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsagDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.CellValueChanged -= IsagDataGridView_CellValueChanged;

            if (_cmbItemSources.Keys.Contains(e.ColumnIndex))
            {

                //Get the new value of the cell (if value has been choosen by a an EdintingControl like a combobox, the cell still contains the old value)
                object value = (EditingControl != null ? EditingControl.Text : this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);


                //if (_cmbItemSources[e.ColumnIndex].IsDisabled(value))
                //{
                //    //Undo change as user has selected a disabled item

                //    // this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.Rows[e.RowIndex].Cells[GetBoundedColumnIndex(e.ColumnIndex)].Value;
                //    this.CancelEdit();

                //    //this.CommitEdit(DataGridViewDataErrorContexts.Commit); 
                //    //this.RefreshEdit();
                //}

                //Update Cell that is databounded to the GridViews DataSource
                this.Rows[e.RowIndex].Cells[GetBoundedColumnIndex(e.ColumnIndex)].Value = value;

                if (_cmbItemSources[e.ColumnIndex].ConfigType == ComboboxConfigType.MARK_INVALID)
                {
                    //Remove invalid items from ItemList (except Cell Values)
                    RemoveUnusedInvalidItems(e.ColumnIndex);

                    UpdateColumnBoxStyle(e.ColumnIndex, this.Rows[e.RowIndex]);
                }
            }

            //if (this.IsCurrentCellDirty) this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            this.CellValueChanged += IsagDataGridView_CellValueChanged;
        }

        /// <summary>
        /// Removes invalid Items from the ItemList if the item does not equal the cell value
        /// </summary>
        /// <param name="columnIndex">the column Index of the combobox column</param>
        private void RemoveUnusedInvalidItems(int columnIndex)
        {
            DataGridViewComboBoxColumn cmbColumn = (DataGridViewComboBoxColumn)this.Columns[columnIndex];

            // invalid items that are still necessary in the itemlist
            List<object> invalidItems = new List<object>();

            foreach (DataGridViewRow row in this.Rows)
            {
                object rowValue = row.Cells[columnIndex].Value;
                if (!_cmbItemSources[columnIndex].GetItemList().Contains(rowValue))
                {
                    invalidItems.Add(rowValue);
                }
            }
            for (int i = cmbColumn.Items.Count - 1; i >= 0; i--)
            {

                object item = cmbColumn.Items[i];
                if (!invalidItems.Contains(item) && !_cmbItemSources[columnIndex].GetItemList().Contains(item)) cmbColumn.Items.RemoveAt(i);
            }
        }

        /// <summary>
        /// Updates the style of a combobox for each row
        /// </summary>
        /// <param name="columnIndex">the column Index of the combobox column</param>
        private void UpdateColumnBoxStyle(int columnIndex)
        {
            foreach (DataGridViewRow row in Rows)
            {
                UpdateColumnBoxStyle(columnIndex, row);
            }
        }
        /// <summary>
        /// Updates the style of a combobox cell
        /// (change colors depending on item state (valid or invalid) 
        /// </summary>
        /// <param name="columnIndex">the column Index of the combobox column</param>
        /// <param name="row">the row that contains the combobox cell that has to be updated</param>
        private void UpdateColumnBoxStyle(int columnIndex, DataGridViewRow row)
        {
            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[columnIndex];

            if (cell.Value != null)
            {
                ComboBoxConfiguration config = _cmbItemSources[columnIndex];
                if (config.ConfigType == ComboboxConfigType.MARK_INVALID)
                {
                    Color color = config.GetForeColor(cell.Value);
                    Color backColor = config.GetBackColor(cell.Value);

                    cell.Style.ForeColor = color;
                    cell.Style.SelectionBackColor = backColor;
                    cell.Style.SelectionForeColor = color;
                }
            }
        }

        /// <summary>
        /// Adds a combobox column that is bounded to another Column
        /// </summary>
        /// <param name="srcColumnName">the name of the column that the combobox column is bounded to</param>
        /// <param name="dataSource">the DataSource for the ItemList of the combobox</param>
        public void AddCellBoundedComboBox(string srcColumnName, BindingList<object> dataSource, ComboboxConfigType configurationType)
        {
            dataSource.ListChanged += DataSource_ListChanged;
            ComboBoxConfiguration comboConfig = new ComboBoxConfiguration(dataSource, configurationType);

            DataGridViewComboBoxColumn cmbColumn = new DataGridViewComboBoxColumn();
            DataGridViewColumn srcColumn = this.Columns[srcColumnName];
            srcColumn.Visible = false;

            cmbColumn.Name = CMB_COLUMN_PREFIX + srcColumnName;
            cmbColumn.HeaderText = srcColumn.HeaderText;
            cmbColumn.Sorted = true;
            cmbColumn.ValueType = srcColumn.ValueType;


            cmbColumn.Items.AddRange(comboConfig.GetItemList());
            cmbColumn.FlatStyle = FlatStyle.Flat;

            int index = _cmbItemSources.Count;
            _cmbItemSources.Add(index, comboConfig);
            this.Columns.Insert(index, cmbColumn);
            this.Columns[index].DisplayIndex = srcColumn.Index;

            //Copy Values from cell (bounded to grid datasource) to cell (combobox cell)
            foreach (DataGridViewRow row in this.Rows)
            {
                row.Cells[cmbColumn.Index].Value = row.Cells[srcColumn.Index].Value;
            }
        }

        public void AddCellBoundedComboBox(string srcColumnName, Type srcEnum)
        {
            BindingList<object> dataSource = new BindingList<object>();
            Array enums = Enum.GetValues(srcEnum);

            for (int i = 0; i < enums.Length; i++)
            {
                dataSource.Add(enums.GetValue(i));
            }

            AddCellBoundedComboBox(srcColumnName, dataSource, ComboboxConfigType.MARK_INVALID);
        }

        /// <summary>
        /// Updates the "bounded" combobox column
        /// (remove invalid items, add invalid items that euqals a cell value of the combobox column
        /// </summary>
        /// <param name="columnIndex">the column index of the combobox column</param>
        private void UpdateCellBoundComboBox(int columnIndex)
        {           
            this.CellValueChanged -= IsagDataGridView_CellValueChanged;
            _cmbItemSources[columnIndex]._itemSource.ListChanged -= DataSource_ListChanged;

            DataGridViewComboBoxColumn cmbColumn = (DataGridViewComboBoxColumn)this.Columns[columnIndex];
            DataGridViewColumn srcColumn = this.Columns[columnIndex + 1];
            int boundedColumnIndex = GetBoundedColumnIndex(columnIndex);


            if (_cmbItemSources[columnIndex].ConfigType == ComboboxConfigType.MARK_INVALID)
            {
                cmbColumn.Items.Clear();
                cmbColumn.Items.AddRange(_cmbItemSources[columnIndex].GetItemList());
            }

            foreach (DataGridViewRow row in this.Rows)
            {
                DataGridViewCell boundedCell = row.Cells[boundedColumnIndex];
                if (boundedCell.Value == null) boundedCell.Value = "";

                if (_cmbItemSources[columnIndex].ConfigType == ComboboxConfigType.MARK_INVALID)
                {
                    if (!_cmbItemSources[columnIndex].GetItemList().Contains(boundedCell.Value))
                    {
                        cmbColumn.Items.Add(boundedCell.Value);
                    }
                }
                try
                {
                    row.Cells[columnIndex].Value = boundedCell.Value;
                }
                catch (Exception)
                {

                }
            }

            _cmbItemSources[columnIndex]._itemSource.ListChanged += DataSource_ListChanged;
            this.CellValueChanged += IsagDataGridView_CellValueChanged;

        }

        public void RefreshCellBoundComboBox(string columnName)
        {
            this.CellValueChanged -= IsagDataGridView_CellValueChanged;
            int columnIndex = this.Columns[CMB_COLUMN_PREFIX + columnName].Index;
            int boundedColumnIndex = GetBoundedColumnIndex(columnIndex);


            foreach (DataGridViewRow row in this.Rows)
            {
                DataGridViewCell boundedCell = row.Cells[boundedColumnIndex];
                try
                {
                  //  this.CurrentCell = row.Cells[columnIndex]; 
                    row.Cells[columnIndex].Value = boundedCell.Value;
                    //this.UpdateCellValue(columnIndex, row.Index);
                }
                catch (Exception)
                {

                }
            }

            this.CellValueChanged += IsagDataGridView_CellValueChanged;

            //this.Update();
            //this.Refresh();
            

        }

        /// <summary>
        /// Updates all "bounded" combobox columns
        /// </summary>
        public void UpdateCellBoundComboBox()
        {
            foreach (int key in _cmbItemSources.Keys)
            {
                UpdateCellBoundComboBox(key);
            }
        }

        /// <summary>
        /// React if the DataSource for the ItemList has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            BindingList<object> datasource = (BindingList<object>)sender;

            foreach (int key in _cmbItemSources.Keys)
            {
                if (_cmbItemSources[key].HasItemSource(datasource) && _cmbItemSources[key].ConfigType == ComboboxConfigType.MARK_INVALID)
                {
                    UpdateCellBoundComboBox(key);
                    UpdateColumnBoxStyle(key);
                }
            }

            //  this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //this.Update();
            //this.Refresh();

        }

        /// <summary>
        /// Gets the column index of the column that the combobox column is bounded to
        /// (by removing the prefix CMB_COLUMN_PREFIX of the combobox column name
        /// </summary>
        /// <param name="cmbColumnIndex"></param>
        /// <returns></returns>
        private int GetBoundedColumnIndex(int cmbColumnIndex)
        {
            string boundedColumnName = this.Columns[cmbColumnIndex].Name.Substring(CMB_COLUMN_PREFIX.Length);
            return this.Columns[boundedColumnName].Index;
        }

        public void RemoveSelectedRows()
        {
            while (SelectedRows.Count > 0)
            {
                this.Rows.Remove(SelectedRows[0]);
            }
        }


        public void SelectCheckBoxes(bool select)
        {
            this.CellValueChanged -= IsagDataGridView_CellValueChanged;
            
            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (cell is DataGridViewCheckBoxCell && !cell.ReadOnly)
                {                    
                    DirtyEditCell(cell, select);                
                }
            }

            foreach (DataGridViewRow row in this.SelectedRows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell is DataGridViewCheckBoxCell && !cell.ReadOnly)
                    {
                        DirtyEditCell(cell, select); 
                    }
                }
            }

            foreach (DataGridViewColumn col in this.SelectedColumns)
            {
                if (col is DataGridViewCheckBoxColumn && !col.ReadOnly)
                {
                    foreach (DataGridViewRow row in this.Rows)
                    {
                        if (!row.Cells[col.Index].ReadOnly)
                        {
                            DirtyEditCell(row.Cells[col.Index], select); 
                        }
                    }
                }
            }

            this.CellValueChanged += IsagDataGridView_CellValueChanged;
        }

        public void DirtyEditCell(DataGridViewCell cell, object value)
        {
            DataGridViewCell currentCell = this.CurrentCell;
            this.CurrentCell = cell;
            cell.Value = value;
            this.CurrentCell = currentCell;
        }

        public void SetCompleteItemSource(List<object> itemSource, string columnName)
        {
            DataGridViewColumn col = this.Columns[CMB_COLUMN_PREFIX + columnName];

            if (col != null)
            {
                _cmbItemSources[col.Index].SetCompleteItemSource(itemSource);
                UpdateCellBoundComboBox(col.Index);
            }
        }
    }



    class ComboBoxConfiguration
    {
       
        public BindingList<object> _itemSource;
        private List<object> _completeItemSource { get; set; }
        public ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType ConfigType { get; set; }

        public ComboBoxConfiguration(BindingList<object> itemSource, ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType configType)
        {
            _itemSource = itemSource;
            _completeItemSource = itemSource.ToList<object>();
            ConfigType = configType;
        }

        public void SetCompleteItemSource(List<object> itemSource)
        {
            _completeItemSource = itemSource;
        }

        public Color GetForeColor(object item)
        {
            bool isValid = IsValid(item);

            if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.MARK_INVALID)
                return isValid ? Color.Black : Color.Red;
            else if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE)
                return isValid ? Color.Black : Color.LightGray;

            return Color.Black;
        }

        public Color GetBackColor(object item)
        {
            bool isValid = IsValid(item);
            if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.MARK_INVALID)
                return isValid ? Color.Empty : Color.White;
            else if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE)
                return isValid ? Color.Empty : Color.White;

            return Color.Empty;
        }

        public object[] GetItemList()
        {
            switch (ConfigType)
            {
                case ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.MARK_INVALID:
                    return _itemSource.ToArray<object>();
                case ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE:
                    return _completeItemSource.ToArray<object>();
                default:
                    break;
            }

            return null;
        }

        private bool IsValid(object item)
        {
            return _itemSource.Contains(item);
        }

        public bool IsDisabled(object item)
        {
            return ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE && !IsValid(item);
        }

        public bool HasItemSource(BindingList<object> itemSource)
        {
            return _itemSource == itemSource;
        }
    }
}
