using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace TableLoader
{
    /// <summary>
    /// Holds sql methods that execute database commands
    /// </summary>
    public class SqlExecutor
    {
  
        /// <summary>
        /// Executes an sql statement
        /// </summary>
        /// <param name="sql">sql statement</param>
        /// <param name="con">sql connection</param>
        /// <param name="timeOut">tiemout</param>
        /// <param name="transaction">sql transaction</param>
        /// <returns>number of rows affected</returns>
        public static int ExecSql(string sql, SqlConnection con, int timeOut, SqlTransaction transaction)
        {            
            if (con.State != ConnectionState.Open)
                con.Open();
            SqlCommand comm = con.CreateCommand();
            comm.CommandText = sql;
            if (transaction != null) comm.Transaction = transaction;

            comm.CommandTimeout = timeOut;

            return comm.ExecuteNonQuery();
        }

        /// <summary>
        /// Truncates an sql table
        /// </summary>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="con">sql connection</param>
        /// <param name="timeOut">timeout</param>
        /// <param name="transaction">transaction</param>
        public static void TruncateTable(string tempTableName, SqlConnection con, int timeOut, SqlTransaction transaction)
        {          
                ExecSql("truncate table " + tempTableName,  con, timeOut, transaction);
        }

        /// <summary>
        /// Drops an sql table
        /// </summary>
        /// <param name="tempTableName">temporary table name</param>
        /// <param name="con">sql connection</param>
        /// <param name="timeOut">timeout</param>
        /// <param name="transaction">transaction</param>
        public static void DropTable(string tempTableName, SqlConnection con, int timeOut, SqlTransaction transaction)
        {
            ExecSql("drop table " + tempTableName, con, timeOut, transaction);
        }

        
    }
}
