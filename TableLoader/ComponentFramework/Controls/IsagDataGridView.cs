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
    public partial class IsagDataGridView: DataGridView
    {
        public enum ComboboxConfigType { MARK_INVALID = 0, DISABLE = 1 }
        private bool IsCellValueChangeEventDisabled = false;

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
            this.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
        }

        /// <summary>
        /// Sort Datagrid
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
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

            //Refresh comboboxes (displayed value is lost because comboBox cell is not directly bounded to the datasource
            foreach (int colIdx in _cmbItemSources.Keys)
            {
                string columnName = this.Columns[GetBoundedColumnIndex(colIdx)].Name;
                RefreshCellBoundComboBox(columnName);
            }

            base.OnColumnHeaderMouseClick(e);
        }

        /// <summary>
        /// Each combobox column has to be updated whenever the bounded column has changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            foreach (int columnIndex in _cmbItemSources.Keys)
            {
                RefreshCellBoundComboBox(this.Columns[GetBoundedColumnIndex(columnIndex)].Name);
            }

            base.OnDataBindingComplete(e);
        }

        /// <summary>
        /// Whenn a Combox is opened register to the DrawItem Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            if (_cmbItemSources.Keys.Contains(this.SelectedCells[0].ColumnIndex))
            {
                ComboBox cmb = (ComboBox) e.Control;

                if (cmb != null)
                {
                    cmb.DrawMode = DrawMode.OwnerDrawFixed;
                    cmb.DrawItem -= cmb_DrawItem;
                    cmb.DrawItem += cmb_DrawItem;
                }

            }
            base.OnEditingControlShowing(e);
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
                ComboBox cmb = (ComboBox) EditingControl;

                object itemValue = ((ComboBox) sender).Items[e.Index];
                Color color = _cmbItemSources[this.SelectedCells[0].ColumnIndex].GetForeColor(itemValue);

                using (var brush = new SolidBrush(color))
                    e.Graphics.DrawString(itemValue.ToString(), e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Commits the cells value change earlier (does not apply to textbox cells)
        /// This way the cell does not have to loose focus to trigger cell value change event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCurrentCellDirtyStateChanged(EventArgs e)
        {
            if (this.IsCurrentCellDirty && !(this.CurrentCell is DataGridViewTextBoxCell))
                this.CommitEdit(DataGridViewDataErrorContexts.Commit);

            base.OnCurrentCellDirtyStateChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            if (!IsCellValueChangeEventDisabled)
            {
                DoCellValueChanged(e);
                base.OnCellValueChanged(e);
            }
        }


        /// <summary>
        /// Set cells back color. Color depends on ReadOnly Property of the cell.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                bool readOnly = this.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly;
                e.CellStyle.BackColor = readOnly ? Color.LightGray : Color.White;
            }

            base.OnCellFormatting(e);
        }

        /// <summary>
        /// When the ReadOnly Property of a cell has changed, the cell has to be redrawn in order to change the back color.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellStateChanged(DataGridViewCellStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.ReadOnly)
                Refresh();
            base.OnCellStateChanged(e);
        }
        /// <summary>
        ///    If a cell value has change the ItemList has to be updated.
        /// - invalid items (not included in the ItemSource but in the ItemList) has to be removed from the ItemList
        /// - if the cell value is not contained in the ItemList it has to be added
        /// </summary>
        /// <param name="e"></param>
        private void DoCellValueChanged(DataGridViewCellEventArgs e)
        {
            IsCellValueChangeEventDisabled = true;

            if (_cmbItemSources.Keys.Contains(e.ColumnIndex))
            {
                DataGridViewRow row = this.Rows[e.RowIndex];
                ComboBoxConfiguration config = _cmbItemSources[e.ColumnIndex];

                //Get the new value of the cell (if value has been choosen by a an EdintingControl like a combobox, the cell still contains the old value)
                object value = (EditingControl != null ? EditingControl.Text : this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                if (value == null)
                    value = "";

                //Update combobox Itemlist
                List<object> itemList = _cmbItemSources[e.ColumnIndex].GetItemList(row.DataBoundItem);

                if (!itemList.Contains(value))
                    itemList.Insert(0, value);
                DataGridViewComboBoxCell cell = ((DataGridViewComboBoxCell) row.Cells[e.ColumnIndex]);
                SetItemList(cell, itemList);

                //Update Cell that is databounded to the GridViews DataSource
                this.Rows[e.RowIndex].Cells[GetBoundedColumnIndex(e.ColumnIndex)].Value = value;
            }


            IsCellValueChangeEventDisabled = false;
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            if (RowCount > 0)
            {
                foreach (ComboBoxConfiguration config in _cmbItemSources.Values)
                {
                    config.AddItemSourceElement(this.Rows[e.RowIndex].DataBoundItem);
                }
            }
            base.OnRowsAdded(e);
        }

        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            if (RowCount > 0)
            {
                foreach (ComboBoxConfiguration config in _cmbItemSources.Values)
                {
                    config.RemoveItemSourceElement(this.Rows[e.RowIndex].DataBoundItem);
                }
            }
            base.OnRowsRemoved(e);
        }


        private ComboBoxConfiguration GetComboBoxConfigFromSingleDataSource(BindingList<object> dataSource, ComboboxConfigType configurationType)
        {
            Dictionary<object, BindingList<object>> dataSourceDictionary = new Dictionary<object, BindingList<object>>();

            foreach (DataGridViewRow row in this.Rows)
            {
                dataSourceDictionary.Add(row.DataBoundItem, dataSource);
            }

            //only one registration is necessary because all datasources are equal
            dataSource.ListChanged += DataSource_ListChanged;

            return new ComboBoxConfiguration(dataSourceDictionary, configurationType);
        }

        /// <summary>
        /// Adds a combobox column that is bounded to another Column
        /// </summary>
        /// <param name="srcColumnName">the name of the column that the combobox column is bounded to</param>
        /// <param name="dataSource">the DataSource for the ItemList of the combobox</param>
        public void AddCellBoundedComboBox(string srcColumnName, BindingList<object> dataSource, ComboboxConfigType configurationType)
        {
            IsCellValueChangeEventDisabled = true;

            ComboBoxConfiguration comboConfig = GetComboBoxConfigFromSingleDataSource(dataSource, configurationType);

            DataGridViewComboBoxColumn cmbColumn = new DataGridViewComboBoxColumn();
            DataGridViewColumn srcColumn = this.Columns[srcColumnName];
            srcColumn.Visible = false;

            cmbColumn.Name = CMB_COLUMN_PREFIX + srcColumnName;
            cmbColumn.HeaderText = srcColumn.HeaderText;
            cmbColumn.ValueType = srcColumn.ValueType;
            cmbColumn.FlatStyle = FlatStyle.Flat;

            int index = _cmbItemSources.Count;
            _cmbItemSources.Add(index, comboConfig);
            this.Columns.Insert(index, cmbColumn);
            this.Columns[index].DisplayIndex = srcColumn.Index;

            //Copy Values from cell (bounded to grid datasource) to cell (combobox cell)
            foreach (DataGridViewRow row in this.Rows)
            {
                List<object> itemList = comboConfig.GetItemList(row.DataBoundItem);
                object value = row.Cells[srcColumn.Index].Value;
                if (!itemList.Contains(value))
                    itemList.Insert(0, value);
                DataGridViewComboBoxCell cell = ((DataGridViewComboBoxCell) row.Cells[cmbColumn.Index]);
                SetItemList(cell, itemList);

                row.Cells[cmbColumn.Index].Value = row.Cells[srcColumn.Index].Value;
            }

            IsCellValueChangeEventDisabled = false;
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
        /// React if the DataSource for the ItemList has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            IsCellValueChangeEventDisabled = true;
            BindingList<object> itemListDataSource = (BindingList<object>) sender;

            foreach (int columnIndex in _cmbItemSources.Keys)
            {
                ComboBoxConfiguration comboConfig = _cmbItemSources[columnIndex];

                if (comboConfig.HasItemListDataSource(itemListDataSource))
                {
                    foreach (DataGridViewRow row in this.Rows)
                    {
                        if (comboConfig.HasItemListDataSource(itemListDataSource, row.DataBoundItem))
                        {
                            List<object> itemList = comboConfig.GetItemList(row.DataBoundItem);

                            object value = row.Cells[columnIndex].Value;
                            // if (value == null) value = ""; //TODO: Warum ist der Wert der ComboBoxCell null und nicht ""? Diese Methode dürfte in diesem Fall ("" vom User gewählt)garnicht aufgerufen werden!"
                            if (!itemList.Contains(value))
                                itemList.Insert(0, value);
                            DataGridViewComboBoxCell cell = ((DataGridViewComboBoxCell) row.Cells[columnIndex]);

                            SetItemList(cell, itemList);

                        }
                    }
                }
            }



            IsCellValueChangeEventDisabled = false;
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
            IsCellValueChangeEventDisabled = true;

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

            IsCellValueChangeEventDisabled = false;
        }

        public void DirtyEditCell(DataGridViewCell cell, object value)
        {
            DataGridViewCell currentCell = this.CurrentCell;
            this.CurrentCell = cell;
            cell.Value = value;
            this.CurrentCell = currentCell;
        }

        public void RefreshCellBoundComboBox(string columnName)
        {
            IsCellValueChangeEventDisabled = true;

            int columnIndex = this.Columns[CMB_COLUMN_PREFIX + columnName].Index;
            int boundedColumnIndex = GetBoundedColumnIndex(columnIndex);

            ComboBoxConfiguration comboConfig = _cmbItemSources[columnIndex];

            foreach (DataGridViewRow row in this.Rows)
            {
                DataGridViewCell boundedCell = row.Cells[boundedColumnIndex];
                try
                {
                    List<object> itemList = comboConfig.GetItemList(row.DataBoundItem);

                    object value = boundedCell.Value;
                    if (value == null)
                    {
                        boundedCell.Value = "";
                        value = "";
                    }
                    if (!itemList.Contains(value))
                        itemList.Insert(0, value);
                    DataGridViewComboBoxCell cell = ((DataGridViewComboBoxCell) row.Cells[columnIndex]);
                    SetItemList(cell, itemList);
                    cell.Value = value;
                }
                catch (Exception)
                {

                }
            }

            IsCellValueChangeEventDisabled = false;
        }

        private void SetItemList(DataGridViewComboBoxCell cell, List<object> itemList)
        {
            int count = cell.Items.Count;
            cell.Items.AddRange(itemList.ToArray<object>());
            for (int i = 0; i < count; i++)
            {
                cell.Items.RemoveAt(0);
            }
        }



    }



    class ComboBoxConfiguration
    {

        private Dictionary<object, BindingList<object>> _itemSource;
        private List<object> _completeItemSource { get; set; }
        public ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType ConfigType { get; set; }

        public ComboBoxConfiguration(Dictionary<object, BindingList<object>> itemSource, ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType configType)
        {
            _itemSource = itemSource;
            ConfigType = configType;
        }

        public void AddItemSourceElement(object dataBoundItem)
        {
            if (_itemSource.Count > 0 && !_itemSource.ContainsKey(dataBoundItem))
                _itemSource.Add(dataBoundItem, _itemSource.Values.First());
        }

        public void RemoveItemSourceElement(object dataBoundItem)
        {
            if (_itemSource.Count > 0 && !_itemSource.ContainsKey(dataBoundItem))
                _itemSource.Remove(dataBoundItem);
        }

        public void SetCompleteItemSource(List<object> itemSource)
        {
            _completeItemSource = itemSource;
        }

        public Color GetForeColor(object item)
        {
            //bool isValid = IsValid(item);

            //if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.MARK_INVALID)
            //    return isValid ? Color.Black : Color.Red;
            //else if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE)
            //    return isValid ? Color.Black : Color.LightGray;

            return Color.Black;
        }

        public Color GetBackColor(object item)
        {
            //bool isValid = IsValid(item);
            //if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.MARK_INVALID)
            //    return isValid ? Color.Empty : Color.White;
            //else if (ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE)
            //    return isValid ? Color.Empty : Color.White;

            return Color.Empty;
        }

        public List<object> GetItemList(object databoundedItem)
        {
            List<object> itemList = _itemSource[databoundedItem].ToList();
            itemList.Sort();
            return itemList;
        }



        private bool IsValid(object databoundedItem, object item)
        {
            return _itemSource[databoundedItem].Contains(item);
        }

        public bool IsDisabled(object item)
        {
            return false; //            ConfigType == ComponentFramework.Controls.IsagDataGridView.ComboboxConfigType.DISABLE && !IsValid(item);
        }

        public bool HasItemListDataSource(BindingList<object> itemSource)
        {
            return _itemSource.ContainsValue(itemSource);
        }

        public bool HasItemListDataSource(BindingList<object> itemSource, object databoundedItem)
        {
            return _itemSource.ContainsKey(databoundedItem) && _itemSource[databoundedItem] == itemSource;
        }

        public object GetDataBoundedItem(BindingList<object> dataSource)
        {
            return _itemSource.FirstOrDefault(x => x.Value.Contains(dataSource)).Key;
        }
    }
}
