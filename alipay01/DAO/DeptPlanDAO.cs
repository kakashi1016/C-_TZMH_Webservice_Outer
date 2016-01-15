using alipay01.Models;
using alipay01.MyUtils;
using mvcDemo01.MyUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace alipay01.DAO
{
    public class DeptPlanDAO
    {
        public static bool GetDeptPlanList(int dt, out Dictionary<string, DeptPlan> dList, out string msg)
        {

            dList = new Dictionary<string, DeptPlan>();
            DeptPlan tempDept = null;
            bool isValid = true;
            string week = "";
            msg = "";
            if (dt > 0 && dt < 8)
            {
                week = "0" + dt;
            }
            else
            {
                isValid = false;
                msg = "输入的参数错误！";
            }
            if (!isValid)
            {
                dList = null;
                return false;
            }


            isValid = false;


            using (SqlConnection sqlCnt = new SqlConnection(SystemParms.sqlConnectString418))
            {
                SqlDataReader reader = null;
                SqlCommand cmd = null;
                sqlCnt.Open();
                try
                {
                    cmd = sqlCnt.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "   select a.deptid,b.deptname as 'deptname',ampm,  "
                        + " doctors=  stuff(( select '#'+rtrim(emp.name) from empplan t join emp on t.empid = emp.empid where t.week =@WEEKDAY "
                        + " and t.doflag ='1' and len(t.deptid) >1 and t.[deptid]=a.[deptid] and a.ampm = ampm for xml path('')), 1, 1, '')   "
                        + "  from empplan a right join dept b on a.deptid = b.deptid "
                        + " where week =@WEEKDAY and doflag ='1' and len(b.deptid) >1 "
                        + " group by a.deptid,ampm,b.deptname order by a.deptid,ampm ";

                    cmd.Parameters.Add("@WEEKDAY", SqlDbType.Char);
                    cmd.Parameters["@WEEKDAY"].Value = week;

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string dID = reader["deptid"].ToString().Trim();
                        if (dList.ContainsKey(dID))
                        {
                            tempDept = dList[dID];
                        }
                        else
                        {
                            tempDept = new DeptPlan(dID, reader["deptname"].ToString().Trim(), "", "");
                        }


                        if ("AM".Equals(reader["ampm"].ToString().Trim().ToUpper()))
                        {
                            tempDept.amDoctorList = reader["doctors"].ToString().Trim();
                        }
                        else
                        {
                            tempDept.pmDoctorList = reader["doctors"].ToString().Trim();
                        }

                        if (!dList.ContainsKey(dID))
                        {
                            dList.Add(dID, tempDept);
                        }


                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    log4netHelper.error("DeptPlanDAO", "GetDeptPlanList:[ " + dt + " ]--  " + e.Message.ToString());
                    msg = "DB查询失败！";
                }
                finally
                {
                    if (null != reader)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                    if (null != cmd)
                    {
                        cmd.Dispose();
                    }
                }

            }



            if (dList.Count == 0)
            {
                dList = null;
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }

    }

    public class RegisterDAO
    {
        public static bool DoRegister(string CardNo, string ReserveTime, string Pname, string Deptin, string Psex, int Page, string AgeUnit, string Paddress, string PTel, string ID, string Doctor, string AmPm, int type, DateTime CDate, int Regsrl,out int result, out string msg)
        {

            bool isSuccess = false;
            msg = "调用存储过程失败！";
            result = -1;



            using (SqlConnection sqlCnt = new SqlConnection(SystemParms.sqlConnectString479))
            {
                SqlDataReader reader = null;
                SqlCommand cmd = null;
                sqlCnt.Open();
                try
                {
                    cmd = sqlCnt.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[UserReg]";

                    cmd.Parameters.Add("@CardNo", SqlDbType.VarChar).Value = CardNo;
                    cmd.Parameters.Add("@ReserveTime", SqlDbType.VarChar).Value = ReserveTime;
                    cmd.Parameters.Add("@Pname", SqlDbType.VarChar).Value = Pname;
                    cmd.Parameters.Add("@Deptin", SqlDbType.VarChar).Value = Deptin;
                    cmd.Parameters.Add("@Psex", SqlDbType.VarChar).Value = Psex;
                    cmd.Parameters.Add("@Page", SqlDbType.Int).Value = Page;
                    cmd.Parameters.Add("@AgeUnit", SqlDbType.VarChar).Value = AgeUnit;
                    cmd.Parameters.Add("@Paddress", SqlDbType.VarChar).Value = Paddress;
                    cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = ID;
                    cmd.Parameters.Add("@PTel", SqlDbType.VarChar).Value = PTel;
                    cmd.Parameters.Add("@Service", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Doctor", SqlDbType.VarChar).Value = Doctor;
                    cmd.Parameters.Add("@AmPm", SqlDbType.VarChar).Value = AmPm;
                    cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = "1";
                    cmd.Parameters.Add("@CDate", SqlDbType.DateTime).Value = CDate;
                    cmd.Parameters.Add("@BeDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Operater", SqlDbType.VarChar).Value = "YQ_Net";
                    cmd.Parameters.Add("@Updatetime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    cmd.Parameters.Add("@Regsrl", SqlDbType.Int).Value = Regsrl;
                    cmd.Parameters.Add("@SrlNo", SqlDbType.Int).Direction = ParameterDirection.Output;


                    reader = cmd.ExecuteReader();
                    result = (int)(cmd.Parameters["@SrlNo"].Value);
                    isSuccess = true;
                    
                    reader.Close();
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    log4netHelper.error("RegisterDAO", "DoRegister:[ 调用存储过程失败 ]--  " + e.Message.ToString());
                    msg = "DB调用存储过程失败！";
                }
                finally
                {
                    if (null != reader)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                    if (null != cmd)
                    {
                        cmd.Dispose();
                    }
                }

            }
            return isSuccess;
        }

    }
}