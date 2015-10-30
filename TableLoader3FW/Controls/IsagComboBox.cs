using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ComponentFramework;


namespace ComponentFramework.Controls
{
    /// <summary>
    /// Enhanced ComboBox that accepts an enumeration or ItemDataSource a datasource for the itemlist
    /// </summary>
    public partial class IsagComboBox : ComboBox
    {
        /// <summary>
        /// Usually updates happen after focus is lost
        /// </summary>
        bool _updateSelectedItemBindingOnSelectedIndexChanged = false;

        /// <summary>
        /// Usually updates happen after focus is lost
        /// </summary>
        public bool UpdateSelectedItemBindingOnSelectedIndexChanged
        {
            get
            {
                return _updateSelectedItemBindingOnSelectedIndexChanged;
            }
            set
            {
                _updateSelectedItemBindingOnSelectedIndexChanged = value;
                this.SelectedIndexChanged -= IsagComboBox_SelectedIndexChanged;
                if (value) this.SelectedIndexChanged += IsagComboBox_SelectedIndexChanged;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public IsagComboBox()
        {
            InitializeComponent();

            this.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
        }

        /// <summary>
        /// React on selected index changed:
        /// Write value for databindings bound to property SelectedItem
        /// </summary>
        /// <param name="sender">evetn sender</param>
        /// <param name="e">event argument</param>
        private void IsagComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DataBindings["SelectedItem"].WriteValue();
        }

        /// <summary>
        /// Fills itemlist from enumeration
        /// </summary>
        /// <param name="srcEnum">source enumeration</param>
        public void SetItemList(Type srcEnum)
        {
            this.Items.Clear();

            foreach (Enum type in Enum.GetValues(srcEnum))
            {
                this.Items.Add(type);
            }
        }

        /// <summary>
        /// Set datasource to itemDatasource
        /// </summary>
        /// <param name="dataSource">itemDatasource</param>
        public void SetItemDataSource(ItemDataSource dataSource)
        {
            this.DataSource = dataSource;
            this.DisplayMember = "DisplayMember";
            this.ValueMember = "ValueMember";
        }
    }
}
