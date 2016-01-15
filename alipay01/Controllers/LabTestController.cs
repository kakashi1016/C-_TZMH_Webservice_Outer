using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using alipay01.DAO;
using alipay01.Models;

namespace alipay01.Controllers
{
    //院内地址
    //http://192.168.100.79:28811/


    [RoutePrefix("LAB")]
    public class LabTestController : Controller
    {
        [Route("Ajax/GetLabList/{mrn}/{dateStart}/{dateEnd}/")]
        public ActionResult GetLabList(int mrn, int dateStart, int dateEnd)
        {
            List<LabTest> list = LabTestDAO.GetLabTestNote(mrn, dateStart, dateEnd);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [Route("Ajax/GetLabItems/{sID}/{doDate}/")]
        public ActionResult GetLabItems(int sID, int doDate)
        {
            List<LabTestItem> list = LabTestDAO.GetLabTestItem(sID, doDate);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
	}
}