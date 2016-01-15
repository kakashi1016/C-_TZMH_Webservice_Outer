using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace alipay01.MyUtils
{
    public class DBHelper
    {
        public static string constr = "Data Source=192.168.100.18;Initial Catalog=ehis;User ID=syd;Password=syd0572;Min Pool Size=10";
        //public static string constr = "Data Source=192.168.100.50;Initial Catalog=Depehis;User ID=mt;Password=mt;Min Pool Size=10";
        public static SqlConnection con = new SqlConnection(constr);
        public static SqlConnection getConnection()
        {
            return con;
        }

        public static SqlConnection createConnection()
        {
            return new SqlConnection(constr);
        }
    }
}