using alipay01.DAO;
using alipay01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace alipay01.Controllers
{
    [RoutePrefix("Register")]
    public class RegisterController : Controller
    {
        // GET: Register
        [Route("GetPlan/{weekday}/")]
        public ActionResult GetDoctorsPlan(int weekday)
        {
            Dictionary<string, DeptPlan> dDict = null;
            string msg;
            bool isSucccess = DeptPlanDAO.GetDeptPlanList(weekday, out dDict, out msg);
            DeptPlanAjaxBean ab = new DeptPlanAjaxBean();
            if (isSucccess)
            {
                
                ab.dict = dDict;
                ab.isSuccess = true;
            }
            else
            {
                ab.dict = null;
                ab.isSuccess = false;
                ab.msg = msg;
            }
            return Json(ab, JsonRequestBehavior.AllowGet);
 
        }

        //public ActionResult GetDoctorsPlan(int weekday)
        //{
        //    Dictionary<string, DeptPlan> dDict = null;
        //    string msg;
        //    bool isSucccess = DeptPlanDAO.GetDeptPlanList(weekday, out dDict, out msg);
        //    DeptPlanAjaxBean ab = new DeptPlanAjaxBean();
        //    if (isSucccess)
        //    {

        //        ab.dict = dDict;
        //        ab.isSuccess = true;
        //    }
        //    else
        //    {
        //        ab.dict = null;
        //        ab.isSuccess = false;
        //        ab.msg = msg;
        //    }
        //    return Json(ab, JsonRequestBehavior.AllowGet);

        //}
    }
}