using alipay01.DAO;
using alipay01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace alipay01.Controllers
{
    //PBI  patient basic imformation
    [RoutePrefix("PBI")]
    public class PatientInformationController : Controller
    {
        //
        // GET: /PatientInformation/
        [Route("Ajax/GetPBI01/{mrn}/")]
        public ActionResult GetPatientInformation01(int mrn)
        {
            PatientInfo01 tmp = PatientInfoDAO.GetPatientInfo01(mrn);
            return Json(tmp, JsonRequestBehavior.AllowGet);
        }
	}
}