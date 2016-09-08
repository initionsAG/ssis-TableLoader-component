using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using TableLoader.SCD;

namespace TableLoader {
    /// <summary>
    /// Windows Form for creating and altering sql tables
    /// </summary>
    public partial class frmCreateTable: Form {
        /// <summary>
        /// Sql connection
        /// </summary>
        private SqlConnection _con;

        /// <summary>
        /// Constructor (creating a table)
        /// </summary>
        /// <param name="properties">SSIS components properites</param>
        /// <param name="con">Sql connection</param>
        public frmCreateTable(IsagCustomProperties properties, SqlConnection con)
        {
            InitializeComponent();

            this.Text = "Create Table";
            tbSql.Text = SqlCreator.GetCreateDestinationTable(properties);
            _con = con;
        }

        /// <summary>
        /// Constructor (altering a table)
        /// </summary>
        /// <param name="properties">SSIS components properites</param>
        /// <param name="sqlColumns">Sql column list</param>
        /// <param name="con">Sql connection</param>
        public frmCreateTable(IsagCustomProperties properties, SqlColumnList sqlColumns, SqlConnection con)
        {
            InitializeComponent();

            this.Text = "Alter Table";
            tbSql.Text = SqlCreator.GetAlterDestinationTable(properties, sqlColumns);
            _con = con;
        }

        /// <summary>
        /// Constructor (creating an SCD table)
        /// </summary>
        /// <param name="properties">SSIS components properites</param>
        /// <param name="columnConfigList">Column config list</param>
        /// <param name="con">Sql connection</param>
        public frmCreateTable(IsagCustomProperties properties, BindingList<ColumnConfig> columnConfigList, SqlConnection con)
        {
            InitializeComponent();
            btnOk.Enabled = false;

            this.Text = "Create SCD Table";
            SCDList scdList = new SCDList(columnConfigList, properties.DestinationTable);
            tbSql.Text = scdList.GetCreateScdTables();
            _con = con;
        }

        /// <summary>
        /// React on button OK click: Execute sql statement for creating/altering table and close window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arguments</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbSql.Text.Length > 0)
                {
                    if (_con.State == ConnectionState.Closed)
                        _con.Open();
                    SqlCommand comm = _con.CreateCommand();
                    comm.CommandText = tbSql.Text;
                    comm.ExecuteNonQuery();
                    _con.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot create/alter table!" + Environment.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// React on button Cancel click: close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Gets table name from sql statement
        /// </summary>
        /// <returns>table name</returns>
        public string GetTableName()
        {
            int posBracketOpen = tbSql.Text.IndexOf("(");
            int posStart = tbSql.Text.IndexOf("CREATE TABLE [") + "CREATE TABLE [".Length;

            string result = tbSql.Text.Substring(posStart, posBracketOpen - posStart).Trim();
            result = result.Replace("[", "");
            result = result.Replace("]", "");

            if (!result.Contains("."))
                result = "dbo." + result;

            return result;
        }

    }
}
