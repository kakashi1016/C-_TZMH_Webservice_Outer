using alipay01.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace alipay01.Controllers
{
    [RoutePrefix("OTHER")]
    public class OtherAssistanceController : Controller
    {
        [Route("Ajax/GetRemoteIP/{sessionID}/")]
        // GET: /OtherAssistance/
        public String GetRemoteIP_18_Ajax(int sessionID)
        {
            string html = "<html><body><h1>" + OtherAssistanceDAO.GetRemoteIP_18(sessionID) 
                +"</h1></body></html>";
            return html;
        }
        [Route("IP/{sessionID}/")]
        // GET: /OtherAssistance/
        public ActionResult GetRemoteIP_18(int sessionID)
        {
            string html = OtherAssistanceDAO.GetRemoteIP_18(sessionID);
            ViewBag.IP = html;
            return View("IPView");
        }
	}
}