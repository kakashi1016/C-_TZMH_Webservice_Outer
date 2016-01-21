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
    abstract class PatientInfoDAO
    {
        public static PatientInfo01 GetPatientInfo01(int mrn)
        {
            PatientInfo01 tmp = null;
            string sqlString = " select a.mrn as 'mrn', a.pname as  'pName', a.psex as 'pSex' , a.age as 'pAge', a.ageunit as 'ageUnit'  "
                + " from pmain a where a.mrn = @MRN ";

            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@MRN",SqlDbType.Int)
            };
            cmdParms[0].Value=mrn;

            DataSet ds =  DbHelperSQL_18.Query(sqlString, cmdParms);

            DataTable dt=ds.Tables["ds"];
            
            if (dt.Rows.Count == 0)
            {
                tmp = null;
            }else {
                DataRow dr = dt.Rows[0];
                string sTmp = "";
                int iTmp = 0;

                tmp = new PatientInfo01(mrn,dr["pName"].ToString().Trim(),"",0,"");
                if (int.TryParse(dr["pAge"].ToString().Trim(), out iTmp))
                {
                    tmp.pAge = iTmp;
                }
                switch (dr["ageUnit"].ToString().Trim().ToUpper())
                {
                    case  "Y":
                        tmp.ageUnit = "岁";
                        break;
                    case "M":
                        tmp.ageUnit = "月";
                        break;
                    case "D":
                        tmp.ageUnit = "日";
                        break;

                }
                switch (dr["pSex"].ToString().Trim().ToUpper())
                {
                    case "F":
                        tmp.pSex = "女";
                        break;
                    case "M":
                        tmp.pSex = "男";
                        break;

                }
                    

            }

            return tmp;
        }


    }
}
