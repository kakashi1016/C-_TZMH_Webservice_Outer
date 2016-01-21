using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using YQ.UTILS.DBHelper;
using alipay01.Models;
namespace alipay01.DAO
{
    abstract class CheckUserDAO
    {
        public static CheckUserInfo  CheckInOut(int userID, int type)
        {
            CheckUserInfo tmp = null;
            string sqlString = " select  b.TestNotes as 'note', b.DoDate as 'doDate', a.OrderNo as 'orderNo', b.specimenid as 'sID' "
                + " from [LISSerV].[his].[dbo].LabOrder a join [LISSerV].[his].[dbo].labspecimen b on a.OrderNo = b.OrderNo "
                + " where a.Mrn = @MRN  and b.DoDate >= @DATESTART and b.DoDate <=@DATEEND and b.DoState >= '5' ";

     //       INSERT INTO CHECKINOUT
     //      (USERID,CHECKTIME,CHECKTYPE,VERIFYCODE,SENSORID,Memoinfo ,WorkCode,sn ,UserExtFmt)
     //VALUES
     //      (319,getdate(),'I',15,103,null,0,'3262155100034',1)


            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@MRN",SqlDbType.Int),
                new SqlParameter("@DATESTART",SqlDbType.Int),
                new SqlParameter("@DATEEND",SqlDbType.Int)
            };
            cmdParms[0].Value=mrn;
            cmdParms[1].Value=dateStart;
            cmdParms[2].Value=dataEnd;

            DataSet ds =  DbHelperSQL_18.Query(sqlString, cmdParms);

            DataTable dt=ds.Tables["ds"];
            //遍历行
            foreach (DataRow dr in dt.Rows)
            {
                //遍历列
                tmp = new LabTest();
                tmp.testNote = dr["note"].ToString().Trim();
                tmp.doDate = (int)dr["doDate"];
                tmp.sID = int.Parse(dr["sID"].ToString().Trim());
                tmp.orderNo = int.Parse(dr["orderNo"].ToString().Trim());
                rtnList.Add(tmp);
            }

            return tmp;
        }


    }
}
