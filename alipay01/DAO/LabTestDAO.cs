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
    abstract  class LabTestDAO
    {
        public static List<LabTest>  GetLabTestNote(int mrn, int dateStart,int dataEnd)
        {
            LabTest tmp= null;
            List<LabTest> rtnList = new List<LabTest>();
            string sqlString = " select  b.TestNotes as 'note', b.DoDate as 'doDate', a.OrderNo as 'orderNo', b.specimenid as 'sID' "
                + " from [LISSerV].[his].[dbo].LabOrder a join [LISSerV].[his].[dbo].labspecimen b on a.OrderNo = b.OrderNo "
                + " where a.Mrn = @MRN  and b.DoDate >= @DATESTART and b.DoDate <=@DATEEND and b.DoState >= '5' ";

            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@MRN",SqlDbType.Int),
                new SqlParameter("@DATESTART",SqlDbType.Int),
                new SqlParameter("@DATEEND",SqlDbType.Int)
            };
            cmdParms[0].Value=mrn;
            cmdParms[1].Value=dateStart;
            cmdParms[2].Value=dataEnd;

            DataSet ds =  DbHelperSQL.Query(sqlString, cmdParms);

            DataTable dt=ds.Tables["ds"];
            //遍历行
            foreach (DataRow dr in dt.Rows)
            {
                //遍历列
                tmp = new LabTest();
                tmp.testNote = dr["note"].ToString();
                tmp.doDate = (int)dr["doDate"];
                tmp.sID = int.Parse(dr["sID"].ToString().Trim());
                tmp.orderNo = int.Parse(dr["orderNo"].ToString().Trim());
                rtnList.Add(tmp);
            }
            
            return rtnList;
        }


        public static List<LabTestItem> GetLabTestItem(int sID, int doDate)
        {
            char cTmp = '-';
            double refL=0,refH=0;
            int iTmp =0;
            string sTmp = "",sTmp1 = "",unit="";
            LabTestItem tmp = null;
            List<LabTestItem> rtnList = new List<LabTestItem>();
            string sqlString = " SELECT   b.chinname as 'cName',b.Unit as 'unit',a.SValue as 'sValue',a.ResultState as 'rState',"
                + " a.Reftype as 'reftype',a.RefLow as 'refLow',a.RefHigh as 'refHigh' "
                + " FROM [LISSerV].[his].[dbo].LabResult a,[LISSerV].[his].[dbo].Labtest b "
                + " WHERE ( a.DoDate = @DODATE ) AND ( a.SpecimenID = @SID ) "
                + " AND ( a.IsProfile = '0' )  AND ( a.testcode = b.testcode)  "
                + " order by b.reportorder  ";

            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@SID",SqlDbType.Int),
                new SqlParameter("@DODATE",SqlDbType.Int)
            };
            cmdParms[0].Value = sID;
            cmdParms[1].Value = doDate;

            DataSet ds = DbHelperSQL.Query(sqlString, cmdParms);

            DataTable dt = ds.Tables["ds"];
            //遍历行
            foreach (DataRow dr in dt.Rows)
            {
                //遍历列
                tmp = new LabTestItem();
                tmp.cName = dr["cName"].ToString().Trim();
                unit = dr["unit"].ToString().Trim();
                if( double.TryParse( dr["refHigh"].ToString().Trim() ,out  refH)){
                    
                }
                if( double.TryParse( dr["refLow"].ToString().Trim() ,out  refL)){
                    
                }

                if (char.TryParse(dr["reftype"].ToString().Trim(), out  cTmp))
                {
                    switch (cTmp) {
                        case '1':

                        case '2':

                            sTmp = "" + refL.ToString() + "-" + refH.ToString() + "";
                            sTmp1 = unit.ToUpper().Substring(0, 1);

                            if (sTmp1.Equals("X") || sTmp1.Equals("%"))
                                sTmp = "(" + sTmp + ")" + unit;
                            break;
                        case '3':
                            sTmp = "阴性";

                            break;
                        case '4':
                            sTmp = "阳性";

                            break;
                        case '5':
                            sTmp = "<" + refH.ToString() + unit;

                            break;
                        case '6':
                            sTmp = ">" + refL.ToString() + unit;

                            break;
                        case '7':
                            sTmp = "<" +refL.ToString()+":"+ refH.ToString() + " "+ unit;

                            break;
                        default:
                            sTmp = "";
                            break;

                    }
                }
                tmp.refference = sTmp;
                tmp.unit = unit;
                if (dr["rState"].ToString().Trim().ToUpper().Equals("H") ){
                    tmp.state = "↑";
                }
                else if (dr["rState"].ToString().Trim().ToUpper().Equals("L"))
                {
                    tmp.state = "↓";
                }
                else
                {
                    tmp.state = "";
                }
                
                tmp.sValue = dr["sValue"].ToString().Trim();


                rtnList.Add(tmp);
            }

            return rtnList;
        }
    }
}
