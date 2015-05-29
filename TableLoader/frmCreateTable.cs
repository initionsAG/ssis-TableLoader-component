using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TableLoader
{
    public partial class frmCreateTable : Form
    {
        private SqlConnection _con;

        public frmCreateTable(IsagCustomProperties properties, SqlConnection con)
        {
            InitializeComponent();

            this.Text = "Create Table";           
            teSql.Text = SqlCreator.GetCreateDestinationTable(properties);
            _con = con;
        }

        public frmCreateTable(IsagCustomProperties properties, SqlColumnList sqlColumns, SqlConnection con)
        {
            InitializeComponent();

            this.Text = "Alter Table";
            teSql.Text = SqlCreator.GetAlterDestinationTable(properties, sqlColumns);
            _con = con;
        }

         public frmCreateTable(IsagCustomProperties properties, BindingList<ColumnConfig> columnConfigList, SqlConnection con)
        {
            InitializeComponent();
            btnOk.Enabled = false;

            this.Text = "Create SCD Table";
            SCDList scdList = new SCDList(columnConfigList, properties.DestinationTable);          
            teSql.Text = scdList.GetCreateScdTables();
            _con = con;
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
      
            try
            {
                if (teSql.Text.Length > 0)
                {
                    if (_con.State == ConnectionState.Closed) _con.Open();
                    SqlCommand comm = _con.CreateCommand();
                    comm.CommandText = teSql.Text;                    
                    comm.ExecuteNonQuery();
                    _con.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen der Tabelle" + Environment.NewLine + ex.ToString());     
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string getTableName() {
            int posBracketOpen = teSql.Text.IndexOf("(");
            int posStart = teSql.Text.IndexOf("CREATE TABLE [") + "CREATE TABLE [".Length;

            string result = teSql.Text.Substring(posStart, posBracketOpen - posStart).Trim();
            result = result.Replace("[", "");
            result = result.Replace("]", "");

            if (!result.Contains(".")) result = "dbo." + result;


            return result;
        }
 
    }
}
