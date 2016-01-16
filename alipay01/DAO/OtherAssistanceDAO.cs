using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using YQ.UTILS.DBHelper;

namespace alipay01.DAO
{
    public abstract class OtherAssistanceDAO
    {
        public static String GetRemoteIP_18(int sessionID)
        {
            string rtn = "";
            string sqlString = " select client_net_address as 'IP' from sys.dm_exec_connections where Session_id= @SESSIONID ";

            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@SESSIONID",SqlDbType.Int)
            };
            cmdParms[0].Value = sessionID;

            DataSet ds = DbHelperSQL.Query(sqlString, cmdParms);

            DataTable dt = ds.Tables["ds"];

            if (dt.Rows.Count > 0)
            {
                rtn = dt.Rows[0]["IP"].ToString().Trim();

            }
            return rtn;
        }
    }
}