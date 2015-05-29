using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace TableLoader
{
    public class Common
    {
        public static int ExecSql(string sql, SqlConnection con, int timeOut)
        {
            return ExecSql(sql, con, timeOut, null);
        }

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

        public static void TruncateTable(string tempTableName, SqlConnection con, int timeOut, SqlTransaction transaction)
        {          
                ExecSql("truncate table " + tempTableName,  con, timeOut, transaction);
        }

        public static void DropTable(string tempTableName, SqlConnection con, int timeOut, SqlTransaction transaction)
        {
            ExecSql("drop table " + tempTableName, con, timeOut, transaction);
        }
    }
}
