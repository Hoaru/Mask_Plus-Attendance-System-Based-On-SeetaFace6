using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MFAsys
{
    class Dao
    {
        //数据库连接
        public SqlConnection connection()
        {
            string str = "Data Source=LAPTOP-8KN6IIJ8;Initial Catalog=MFAS;Integrated Security=True";
            //string str = "Data Source=LAPTOP-8KN6IIJ8;Initial Catalog=stum_database;Integrated Security=True";
            SqlConnection sqlconnection = new SqlConnection(str);//建立连接
            sqlconnection.Open();//打开数据库链接
            return sqlconnection;
        }
        //指令建立
        public SqlCommand command(string str)
        {
            SqlCommand sqlcommand = new SqlCommand(str, connection());
            return sqlcommand;
        }

        //执行SQL语句,用于delete、update、insert，返回受影响的行数
        public int execute(string sql)
        {
            return command(sql).ExecuteNonQuery();//对链接执行transact-sql语句，并返回受影响的行数
        }

        //执行SQL语句,用于select，返回符合条件的对象
        public SqlDataReader read(string sql)
        {
            return command(sql).ExecuteReader();
        }
    }
}
