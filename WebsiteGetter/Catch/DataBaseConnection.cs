using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace WebsiteGetter
{
    class DataBaseConnection
    {
        public string connectionString;
        public DataBaseConnection(string connstr)
        {
            connectionString = connstr;
        }
        /// <summary>
        /// 执行sql语句，返回查询结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet search(string sql)
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand MyCommand = new SqlCommand(sql, con);
            SqlDataAdapter SelectAdapter = new SqlDataAdapter();
            SelectAdapter.SelectCommand = MyCommand;
            DataSet MyDataSet = new DataSet();
            con.Open();
            SelectAdapter.SelectCommand.ExecuteNonQuery();
            con.Close();
            SelectAdapter.Fill(MyDataSet);
            return MyDataSet;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        public void excute(string sql)
        {
            SqlConnection con = new SqlConnection(connectionString);
            //sql = ToGBK(sql);
            SqlCommand MyCommand = new SqlCommand(sql, con);
            SqlDataAdapter SelectAdapter = new SqlDataAdapter();
            SelectAdapter.InsertCommand = MyCommand;
            con.Open();
            SelectAdapter.InsertCommand.ExecuteNonQuery();
            con.Close();
        }

    }
}
