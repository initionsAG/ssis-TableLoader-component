using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;

namespace TableLoader
{
    public partial class IsagCheckBox : UltraCheckEditor
    {
        public IsagCheckBox()
        {
            InitializeComponent();

            this.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
        }

        protected override void OnCheckedValueChanged()
        {
            base.OnCheckedValueChanged();

            if (this.Editor.IsInEditMode) this.Editor.ExitEditMode(false, true);
        }

    }
}
