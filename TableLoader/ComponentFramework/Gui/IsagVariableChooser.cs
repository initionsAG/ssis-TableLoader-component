using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using Microsoft.SqlServer.Dts.Runtime;
using System.Windows.Forms;
using Infragistics.Win;
using ComponentFramework.Controls;
using System.Drawing;

namespace ComponentFramework.Gui
{
    public partial class IsagVariableChooser : IsagUltraComboEditor
    {
        private Variables _variables;
        private frmSelectVariable _frmSelectVariable = null;
        private Icon _icon;

        public void Initialize(Variables variables, Icon icon)
        {
            _icon = icon;
            _variables = variables;
            PopulateValueList();
            this.ButtonsRight.Add(new EditorButton() { Text = "...", ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton });
            this.EditorButtonClick += new EditorButtonEventHandler(IsagVariableChooser_EditorButtonClick);
        }

        private void IsagVariableChooser_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            if (_frmSelectVariable == null) _frmSelectVariable = new frmSelectVariable(_variables, _icon);

            if (_frmSelectVariable.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                this.SelectedItem = this.Items.ValueList.FindByDataValue(_frmSelectVariable.SelectedVariable);
            }
        }


        public string SelectedVariable
        {
            get
            {
                return this.SelectedItem == null ? "" : this.SelectedItem.ToString();
            }
        }

        public IsagVariableChooser()
        {
            InitializeComponent();

            this.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            this.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;

        }

        public IsagVariableChooser(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            this.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            this.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
        }

        protected override void OnAfterCloseUp(EventArgs args)
        {
            base.OnAfterCloseUp(args);

            if (this.Editor.IsInEditMode) this.Editor.ExitEditMode(false, true);
        }

        private void PopulateValueList()
        {
            this.ValueList.ValueListItems.Clear();
            this.SortStyle = ValueListSortStyle.None;

            List<string> varNames = new List<string>();

            foreach (Variable variable in _variables)
            {
                if (!varNames.Contains(variable.QualifiedName)) varNames.Add(variable.QualifiedName);
            }

            varNames.Sort();

            foreach (string varName in varNames)
            {
                this.ValueList.ValueListItems.Add(varName);
            }

        }

    }
}
