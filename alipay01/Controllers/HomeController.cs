using alipay01.Models;
using Com.Alipay;
using mvcDemo01.MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace alipay01.Controllers
{
    [RoutePrefix("alipayDemo")]
    public class HomeController : Controller
    {
        
        [Route("payment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Index()
        {
            //调用log4net
            //
            //调用1
            //LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType).Error("logtest");
            //调用2
            //logger.Error("111111111111");
            //调用3
            //log4netHelper.error(this.GetType(), "321");

            //

            Recharge rechargeInfo = new Recharge();
            TryUpdateModel(rechargeInfo);

            if (!ModelState.IsValid)
            {
                return "失败";
            }



           
            //---------------------------------手机Wap
            //支付宝网关地址
            string GATEWAY_NEW = "http://wappaygw.alipay.com/service/rest.htm?";

            ////////////////////////////////////////////调用授权接口alipay.wap.trade.create.direct获取授权码token////////////////////////////////////////////

            //返回格式
            string format = "xml";
            //必填，不需要修改

            //返回格式
            string v = "2.0";
            //必填，不需要修改

            //请求号
            string req_id = DateTime.Now.ToString("yyyyMMddHHmmss");
            //必填，须保证每次请求都是唯一

            //req_data详细信息

            //服务器异步通知页面路径
            string notify_url = "http://localhost:3056/alipay/success02";
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string call_back_url = "http://localhost:3056/alipay/success01";
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //操作中断返回地址
            string merchant_url = "http://localhost:3056/WS_WAP_PAYWAP-CSHARP-UTF-8/xxxxx.aspx";
            //用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数

            //商户订单号
            string out_trade_no = rechargeInfo.flowid;
            //Request.Form["flowid"];//"testID00008";//   WIDout_trade_no.Text.Trim();
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = rechargeInfo.desc;
            //Request.Form["desc"]; //"testItem0000008";// //WIDsubject.Text.Trim();
            //必填

            //付款金额
            string total_fee = rechargeInfo.fee.ToString();
            //Request.Form["fee"]; //"0.01";//WIDtotal_fee.Text.Trim();
            //必填

            //请求业务参数详细
            string req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" + call_back_url + "</call_back_url><seller_account_name>" + Config.Seller_email + "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url></direct_trade_create_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTempToken = new Dictionary<string, string>();
            sParaTempToken.Add("partner", Config.Partner);
            sParaTempToken.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTempToken.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTempToken.Add("service", "alipay.wap.trade.create.direct");
            sParaTempToken.Add("format", format);
            sParaTempToken.Add("v", v);
            sParaTempToken.Add("req_id", req_id);
            sParaTempToken.Add("req_data", req_dataToken);

            //建立请求
            string sHtmlTextToken = Submit.BuildRequest(GATEWAY_NEW, sParaTempToken);
            //URLDECODE返回的信息
            Encoding code = Encoding.GetEncoding(Config.Input_charset);
            sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

            //解析远程模拟提交后返回的信息
            Dictionary<string, string> dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken);

            //获取token
            string request_token = dicHtmlTextToken["request_token"];

            ////////////////////////////////////////////根据授权码token调用交易接口alipay.wap.auth.authAndExecute////////////////////////////////////////////


            //业务详细
            string req_data = "<auth_and_execute_req><request_token>" + request_token + "</request_token></auth_and_execute_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTemp = new Dictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTemp.Add("service", "alipay.wap.auth.authAndExecute");
            sParaTemp.Add("format", format);
            sParaTemp.Add("v", v);
            sParaTemp.Add("req_data", req_data);

            //建立请求
            string sHtmlText = Submit.BuildRequest(GATEWAY_NEW, sParaTemp, "get", "确认");
            //写html
            //Response.Write(sHtmlText);
            return sHtmlText;
            

           
        }

        [Route("success01")]
        public ActionResult About()
        {
            ViewBag.Message = "success01";

            return View();
        }

        [Route("success02")]
        public ActionResult Contact()
        {
            ViewBag.Message = "恭喜您，预交款充值成功！";
            log4netHelper.error("111", "异步");
            return View("View");
        }

        [Route("show")]
        public ActionResult ShowItem()
        {
            ViewBag.Message = "Show";
            
            return View();
        }
    }
}