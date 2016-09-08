using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ComponentFramework
{
    /// <summary>
    /// Element of a ItemSource for a combobox
    /// </summary>
    public class ItemDataSourceElement
    {
        /// <summary>
        /// value of the combobox item
        /// </summary>
        public object ValueMember { get; set; }
        /// <summary>
        /// displayed text of the combobox item
        /// </summary>
        public string DisplayMember { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">value member</param>
        /// <param name="display">display member</param>
        public ItemDataSourceElement (object value, string display)
        {
            ValueMember = value;
            DisplayMember = display;
        }
    }

    /// <summary>
    /// ItemSource for a combobox
    /// DataSource is an enumeration (values) and a string array (display).
    /// </summary>
    public class ItemDataSource : BindingList<ItemDataSourceElement>
    {
        /// <summary>
        /// Disabled (removed) items 
        /// </summary>
        private BindingList<ItemDataSourceElement> disabledItems = new BindingList<ItemDataSourceElement>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="srcEnum">Item values (enumeration)</param>
        /// <param name="stringValue">Item display members (string array)</param>
        public ItemDataSource(Type srcEnum, string[] stringValue)
            : base()
        {
            FillItemDataSource(srcEnum, stringValue);
        }

        /// <summary>
        /// Creates ItemDataSourceElements asnd fills this list
        /// </summary>
        /// <param name="srcEnum"></param>
        /// <param name="stringValue"></param>
        private void FillItemDataSource(Type srcEnum, string[] stringValue)
        {
            this.Items.Clear();
            Array enums = Enum.GetValues(srcEnum);

            for (int i = 0; i < enums.Length; i++)
            {
                this.Add(new ItemDataSourceElement(enums.GetValue(i), stringValue[i]));
            }
        }

        /// <summary>
        /// Gets ItemDataSourceElement by value
        /// </summary>
        /// <param name="value">the enumeration value</param>
        /// <param name="list">this list</param>
        /// <returns>ItemDataSourceElement</returns>
        private ItemDataSourceElement GetItemDataSourceElement(object value, BindingList<ItemDataSourceElement> list)
        {
            foreach (ItemDataSourceElement item in list)
            {
                if (item.ValueMember == value) return item;
            }

            return null;
        }

        /// <summary>
        /// Does an ItemDataSourceElement with the given value exist?
        /// </summary>
        /// <param name="value">enumeratiuon valule</param>
        /// <returns>Does an ItemDataSourceElement with the given value exist?</returns>
        public bool HasItemValue(object value)
        {
            return (GetItemDataSourceElement(value, this) != null);
        }
        /// <summary>
        /// Disables the value
        /// </summary>
        /// <param name="value">enumeration value</param>

        public void RemoveItem(object value)
        {
            ItemDataSourceElement item = GetItemDataSourceElement(value, this);
            if (item != null)
            {
                this.Remove(item);
                disabledItems.Add(item);
            }
        }

        /// <summary>
        /// Enables the value
        /// </summary>
        /// <param name="value">>enumeration value</param>
        public void AddItem(object value)
        {
            ItemDataSourceElement item = GetItemDataSourceElement(value, disabledItems);
            if (item != null)
            {
                disabledItems.Remove(item);
                this.Add(item);
            }
        }

    }

}

