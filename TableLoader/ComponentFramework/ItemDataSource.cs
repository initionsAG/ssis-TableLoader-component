using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TableLoader.ComponentFramework
{
    public class ItemDataSourceElement
    {
        public object ValueMember { get; set; }
        public string DisplayMember { get; set; }

        public ItemDataSourceElement (object value, string display)
        {
            ValueMember = value;
            DisplayMember = display;
        }
    }

    public class ItemDataSource : BindingList<ItemDataSourceElement>
    {
        private BindingList<ItemDataSourceElement> disabledItems = new BindingList<ItemDataSourceElement>();

        public ItemDataSource(Type srcEnum, string[] stringValue)
            : base()
        {
            FillItemDataSource(srcEnum, stringValue);
        }

        private void FillItemDataSource(Type srcEnum, string[] stringValue)
        {
            this.Items.Clear();
            Array enums = Enum.GetValues(srcEnum);

            for (int i = 0; i < enums.Length; i++)
            {
                this.Add(new ItemDataSourceElement(enums.GetValue(i), stringValue[i]));
            }
        }

        private ItemDataSourceElement GetItemDataSourceElement(object value, BindingList<ItemDataSourceElement> list)
        {
            foreach (ItemDataSourceElement item in list)
            {
                if (item.ValueMember == value) return item;
            }

            return null;
        }

        public bool HasItemValue(object value)
        {
            return (GetItemDataSourceElement(value, this) != null);
        }

        public void RemoveItem(object value)
        {
            ItemDataSourceElement item = GetItemDataSourceElement(value, this);
            if (item != null)
            {
                this.Remove(item);
                disabledItems.Add(item);
            }
        }

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

