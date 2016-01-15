using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace alipay01.MyUtils
{
    public class SystemParms
    {
        public static int socketPort = int.Parse(ConfigurationManager.AppSettings["socketPort"]);
        public static string socketIP = ConfigurationManager.AppSettings["socketIP"];

        public static string sqlConnectString418 = "Data Source=192.168.100.18;Initial Catalog=ehis;User ID=syd;Password=syd0572;Min Pool Size=10";
        public static string sqlConnectString479 = "Data Source=192.168.100.79;Initial Catalog=ehis;User ID=Bespeak;Password=Bespeak;Min Pool Size=10";
  
    }
}