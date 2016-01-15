using alipay01.Models;
using alipay01.MyUtils;
using Com.Alipay;
using mvcDemo01.MyUtils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace alipay01.Controllers
{
    [RoutePrefix("Alipay")]
    public class AliPayController : Controller
    {
        // GET: AliPay
        [Route("Payment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Payment()
        {
            int state;
            DateTime dt_now = DateTime.Now;
            Recharge rechargeInfo = new Recharge();
            TryUpdateModel(rechargeInfo);

            if (!ModelState.IsValid)
            {
                return "输入错误!";
            }

            rechargeInfo = (Recharge)Session["rechargeInfo"];


            if (null == rechargeInfo)
            {
                return "输入错误!";
            }

            SqlConnection sqlCnt = DBHelper.getConnection();
            SqlCommand cmd = null;
            SqlTransaction sqlTransaction = null;


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
            string req_id = dt_now.ToString("yyyyMMddHHmmss");
            //必填，须保证每次请求都是唯一

            //req_data详细信息

            //服务器异步通知页面路径
            string notify_url = "http://122.226.141.18:28810/Alipay/Notify";
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string call_back_url = "http://122.226.141.18:28810/Alipay/Success";
            //需http://格式的完整路径，不允许加?id=123这类自定义参数

            //操作中断返回地址
            string merchant_url = "http://122.226.141.18:28810/Alipay/Error";
            //用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数

            //商户订单号
            string out_trade_no = rechargeInfo.flowid;
            //Request.Form["flowid"];//"testID00008";//   WIDout_trade_no.Text.Trim();
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = "预交款:流水号:["+rechargeInfo.desc+"],mrn:{"+rechargeInfo.mrn+"}";
            //Request.Form["desc"]; //"testItem0000008";// //WIDsubject.Text.Trim();
            //必填

            //付款金额
            string total_fee = rechargeInfo.fee.ToString("f2");
            //Request.Form["fee"]; //"0.01";//WIDtotal_fee.Text.Trim();
            //必填

            //订单超时时间
            string pay_expir = "1";
            // 1 min

            //请求业务参数详细
            string req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" + call_back_url + "</call_back_url><seller_account_name>" + Config.Seller_email + "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url><pay_expire>" + pay_expir + "</pay_expire></direct_trade_create_req>";
            //必填

            Guid guid = new Guid();
            try
            {
                guid = new Guid(out_trade_no);
            }
            catch (Exception e)
            {
                //取不到支付宝预交款订单信息
                return "输入错误的订单号。"+e.Message;
            }






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


            //=======================================================================================



            //本地记录xml
            
            try
            {
                sqlCnt.Open();

                cmd = sqlCnt.CreateCommand();              //创建SqlCommand对象
                cmd.CommandType = CommandType.Text;
                //cmd.CommandText = "select COUNT(*) from BankMsgSwitch where GUID =@GUID";   //sql语句

                cmd.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@GUID"].Value = guid;

                cmd.Parameters.Add("@HospSerial", SqlDbType.NChar);
                cmd.Parameters["@HospSerial"].Value = rechargeInfo.desc;

                cmd.Parameters.Add("@TMsg_xml", SqlDbType.Xml);
                cmd.Parameters["@TMsg_xml"].Value = req_dataToken;

                cmd.Parameters.Add("@TMsg_txt", SqlDbType.Text);
                cmd.Parameters["@TMsg_txt"].Value = req_dataToken;

                cmd.Parameters.Add("@Tdatetime", SqlDbType.DateTime);
                cmd.Parameters["@TDatetime"].Value = dt_now;



                sqlTransaction = sqlCnt.BeginTransaction();
                cmd.Transaction   =   sqlTransaction;
                cmd.CommandText = "insert into BankMsgSwitch (GUID,TransType,HospSerialNo,BankID,TMsg,TMsgText,TDateTime) values (@GUID,'I-paPref',@HospSerial,'100040',@TMsg_xml,@TMsg_txt,GETDATE())";   //sql语句
                try
                {
                    cmd.ExecuteNonQuery();
                    sqlTransaction.Commit();
                    state = 1;
                }
                catch (Exception e)
                {
                    sqlTransaction.Rollback();
                    state = -1;
                    log4netHelper.error("转支付宝失败", "--2--前端发起支付宝交易失败：[GUID]" + guid + "__[ERROR]" + e.Message.ToString());
                }
                   
                
            }
            catch (Exception e)
            {
                state = -3;
                log4netHelper.error("转支付宝失败", "--1--前端获取订单信息失败：[GUID]" + guid + "__[ERROR]" + e.Message.ToString());
            }
            finally
            {
                if (null != sqlTransaction)
                {
                    
                    sqlTransaction.Dispose();
                    sqlTransaction = null;
                }
                
               
                if (null != cmd)
                {
                    
                    cmd.Dispose();
                    cmd = null;
                }
                sqlCnt.Close();

            }

            switch (state) {
                case 1:
                    return sHtmlText;
                    break;
                case -1:
                    return "失败";
                default:
                    return "失败";

            }
            
            
        }

        [Route("Success")]
        public ActionResult Success()
        {
            ViewBag.Message = "success";

            //byte[] result = new byte[1024];
            //string posString, flowString;
            //string a;
            

            //IPAddress ip = IPAddress.Parse(MyUtils.SystemParms.socketIP );
            //Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //try
            //{
            //    clientSocket.Connect(new IPEndPoint(ip, MyUtils.SystemParms.socketPort)); //配置服务器IP与端口  
            //    //Console.WriteLine("连接服务器成功");
            //}
            //catch (Exception e)
            //{
            //    //Console.WriteLine("连接服务器失败，请按回车键退出！");
            //    return "Error" + e.Message;
            //}
            ////通过clientSocket接收数据  
            ////int receiveLength = clientSocket.Receive(result);
            ////log4netHelper.fatal("Socket",Encoding.ASCII.GetString(result, 0, receiveLength));
            //////Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));
            //////通过 clientSocket 发送数据  

            //string sendMessage = "";
            //try
            //{
            //    //Thread.Sleep(100);    //等待1秒钟  
            //    sendMessage = "<Root><TransCode>801</TransCode><Date>20140619</Date><BankTrace></BankTrace><BankNumber></BankNumber><idno>331002198810160636</idno><AMT>0.01</AMT><TradeCode>123</TradeCode><PosId>A001</PosId></Root>";
            //    clientSocket.Send(Encoding.GetEncoding("gb18030").GetBytes(sendMessage));
            //    //Console.WriteLine("向服务器发送消息：{0}" + sendMessage);

            //    string recStr = "";
            //    byte[] recBytes = new byte[4096];
            //    int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
            //    //recStr += Encoding.GetEncoding("gb18030").GetString(recBytes, 0, bytes);
            //    recStr += Encoding.Default.GetString(recBytes, 0, bytes);
            //    sendMessage = recStr;
            //    recStr = recStr.Substring(10, recStr.Length - 10);
            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.LoadXml(recStr);
            //    //商户订单号
            //    recStr = xmlDoc.SelectSingleNode("/Response/TransReturn").InnerText;



                
            //    xmlDoc = null;

            //}
            //catch (Exception e)
            //{

            //    sendMessage = "Error" + e.Message;
            //}
            //finally
            //{
            //    clientSocket.Shutdown(SocketShutdown.Both);
            //    clientSocket.Close();
            //}



            return View("Success");
        }

        [ValidateInput(false)]
        [Route("Notify")]
        public string Contact()
        {

            SqlConnection sqlCnt = DBHelper.getConnection();
            SqlCommand cmd = null;
            SqlTransaction sqlTransaction = null;

            SqlConnection sqlCnt2 = DBHelper.createConnection();
            SqlCommand cmd2 = null;


            SqlDataReader reader = null;

            log4netHelper.info("Notify:" , "Begin--收到异步通知");
            string rtnXML ="",buyerAcount="";
            string out_trade_no = "";

            string content = "";
            bool isSuccess = false;
            string mrnString = "";
            string flowString = "";
            string posString = "";
            double feeRtn = 0;
            DateTime payDate = new DateTime();
            Dictionary<string, string> sPara = GetRequestPost(Request.Form);
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyNotify(sPara, sPara["sign"]);

                if (verifyResult)//验证成功
                {
                    //////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码
                    int status = 0;

                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //解密（如果是RSA签名需要解密，如果是MD5签名则下面一行清注释掉）
                    //sPara = aliNotify.Decrypt(sPara);

                    //XML解析notify_data数据
                    try
                    {

                        rtnXML = sPara["notify_data"];
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(sPara["notify_data"]);
                        //商户订单号
                        out_trade_no = xmlDoc.SelectSingleNode("/notify/out_trade_no").InnerText;
                        //支付宝帐号
                        buyerAcount = xmlDoc.SelectSingleNode("/notify/buyer_email").InnerText;
                        //金额
                        feeRtn = double.Parse(xmlDoc.SelectSingleNode("/notify/total_fee").InnerText);
                        //支付宝交易号
                        string trade_no = xmlDoc.SelectSingleNode("/notify/trade_no").InnerText;
                        //交易状态
                        string trade_status = xmlDoc.SelectSingleNode("/notify/trade_status").InnerText;
                        //
                        payDate = DateTime.Parse(xmlDoc.SelectSingleNode("/notify/gmt_payment").InnerText);

                        posString = xmlDoc.SelectSingleNode("/notify/subject").InnerText;

                        xmlDoc = null;


                        mrnString = "";
                        flowString = "";
                        int posStart = 0;
                        int posEnd = 0;
                        posStart = posString.IndexOf(":{") + 2;
                        posEnd = posString.IndexOf("}");

                        if ((posEnd - posStart) > 3)
                        {
                            mrnString = posString.Substring(posStart, posEnd - posStart);
                            posStart = posString.IndexOf(":[") + 2;
                            posEnd = posString.IndexOf("]");
                            if ((posEnd - posStart) > 3)
                            {
                                flowString = posString.Substring(posStart, posEnd - posStart);
                                posString = flowString.Substring(flowString.Length - 4, 4);
                                flowString = flowString.Substring(0, flowString.Length - 4);
                            }
                            else
                            {
                                mrnString = "";
                                flowString = "";
                                posString = "";
                            }
                        }
                        else
                        {
                            mrnString = "";
                            flowString = "";
                            posString = "";
                        }

                        

                        log4netHelper.info("Notify:" + trade_status, "RTN:" + rtnXML);
                        log4netHelper.info("Notify:", "流水号:" + out_trade_no);
                        log4netHelper.info("Notify:", "淘宝订单号:" + trade_no);
                        log4netHelper.info("Notify:", "支付宝帐号:" + buyerAcount);
                        log4netHelper.info("Notify:", "金额:" + feeRtn.ToString());
                        log4netHelper.info("Notify", "mrn：" + mrnString);
                        log4netHelper.info("Notify", "flowid：" + flowString);
                        log4netHelper.info("Notify", "pos：" + posString);
                        if (trade_status == "TRADE_FINISHED")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序

                            //注意：
                            //该种交易状态只在两种情况下出现
                            //1、开通了普通即时到账，买家付款成功后。
                            //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。

                            status = 2;
                            //return "异步通知：交易完成";
                            //Response.Write("success");  //请不要修改或删除
                        }
                        else if (trade_status == "TRADE_SUCCESS")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序

                            //注意：
                            //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。

                            status = 1;
                            //return "异步通知：交易成功";
                            
                            //Response.Write("success");  //请不要修改或删除
                        }
                        else
                        {
                            status = -1;
                            //Response.Write(trade_status);
                            //return "异步通知：交易类型为其他";
                        }

                    }
                    catch (Exception exc)
                    {
                        status = -9;
                        content = "解析报表有误:" + exc.Message;
                        log4netHelper.info("Notify", "解析报表有误！");
                        //return "异步通知：解析报表出错";
                    }
                    finally
                    {
                        log4netHelper.info("Notify", "解析走完！");
                    }


                    if (status > 0)
                    {
                        if (out_trade_no.Length > 10 && mrnString.Length>4 && posString.Length==4 && flowString.Length >10)
                        {
                            Socket clientSocket = null;
                            Guid guid = new Guid();
                            try
                            {
                                guid = new Guid(out_trade_no);

                                string pcid ="";
                                int count = 0;

                                sqlCnt.Open();
                                sqlCnt2.Open();

                                cmd = sqlCnt.CreateCommand();              //创建SqlCommand对象
                                cmd2 = sqlCnt2.CreateCommand();


                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "select COUNT(*) from BankMsgSwitch where GUID =@GUID";   //sql语句

                                cmd.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier);
                                cmd.Parameters["@GUID"].Value = guid;

                                cmd2.CommandType = CommandType.Text;
                                cmd2.CommandText = "select COUNT(*) from BankMsgSwitch where GUID =@GUID";   //sql语句

                                cmd2.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier);
                                cmd2.Parameters["@GUID"].Value = guid;
                                
                                
                                cmd.Parameters.Add("@RMsg_xml", SqlDbType.Xml);
                                cmd.Parameters["@RMsg_xml"].Value = rtnXML;

                                cmd.Parameters.Add("@RMsg_txt", SqlDbType.Text);
                                cmd.Parameters["@RMsg_txt"].Value = rtnXML;

                                cmd.Parameters.Add("@BuyerAcount", SqlDbType.Text);
                                cmd.Parameters["@BuyerAcount"].Value = buyerAcount;

                                cmd.Parameters.Add("@Rdatetime", SqlDbType.DateTime);
                                cmd.Parameters["@Rdatetime"].Value = DateTime.Now;

                                cmd.Parameters.Add("@PayDate", SqlDbType.DateTime);
                                cmd.Parameters["@PayDate"].Value = payDate;

                                cmd.Parameters.Add("@FeeRtn", SqlDbType.Decimal);
                                cmd.Parameters["@FeeRtn"].Value = feeRtn;

                                cmd.Parameters.Add("@Mrn", SqlDbType.Int);
                                cmd.Parameters["@Mrn"].Value = int.Parse(mrnString);


                                count = (int)cmd.ExecuteScalar();

                                //cmd.CommandText = "select COUNT(*) from BankTradeTitle where GInterID =@GUID";
                                //count *= (int)cmd.ExecuteScalar();

                               
                                if (count == 1 )
                                {
                                   
                                        cmd.CommandText = "select b.pcid as 'pcid' from pmain b where mrn =@Mrn";

                                        reader = cmd.ExecuteReader();

                                        if (reader.HasRows)
                                        {
                                            reader.Read();

                                            pcid = reader["pcid"].ToString();
                                            cmd.Parameters.Add("@Pcid", SqlDbType.Text);
                                            cmd.Parameters["@Pcid"].Value = reader["pcid"].ToString();

                                            reader.Close();
                                            reader.Dispose();
                                            try
                                            {

                                                cmd.CommandText = "select RMsgText from BankMsgSwitch where GUID =@GUID";
                                                string rMsgText = cmd.ExecuteScalar().ToString().Trim();
                                                if (rMsgText.Length<3)
                                                 {
                                                     sqlTransaction = sqlCnt.BeginTransaction();
                                                     cmd.Transaction = sqlTransaction;
                                                     cmd.CommandText = "update BankMsgSwitch set RMsg = @RMsg_xml ,RMsgText = @RMsg_txt,RDateTime=@Rdatetime,socketflag='0' where GUID = @GUID";   //sql语句
                                                     cmd.ExecuteNonQuery();

                                                     //cmd.CommandText = "insert into BankMnyFlow (FlowID,CrdNO,Mrn,DoType,DoMoney,Operater,Updatetime,BankID) values (@GUID,@Pcid,@Mrn,'1',@FeeRtn,@Operater,@Rdatetime,'100040')";   //sql语句
                                                     //cmd.ExecuteNonQuery();


                                                     //转账

                                                     sqlTransaction.Commit();
                                                     content = "更新日志表成功,待发送Socket";
                                                     log4netHelper.info("Notify", "SWITCH表Commit成功！");
                                                     log4netHelper.info("Notify", "身份证：" + pcid);
                                                     log4netHelper.info("Notify", "GUID：" + guid.ToString());

                                                     log4netHelper.info("Notify", "更新日志表成功" );
                                                 }
                                                if (null != sqlTransaction)
                                                    sqlTransaction.Dispose();

                                                sqlTransaction = sqlCnt2.BeginTransaction();
                                                cmd2.Transaction = sqlTransaction;
                                                cmd2.CommandText = "select socketflag from BankMsgSwitch where GUID =@GUID";
                                                rMsgText = cmd2.ExecuteScalar().ToString().Trim();
                                                if ("0".Equals(rMsgText))
                                                {
                                                    IPAddress ip = IPAddress.Parse(MyUtils.SystemParms.socketIP);
                                                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                    clientSocket.Connect(new IPEndPoint(ip, MyUtils.SystemParms.socketPort)); //配置服务器IP与端口  

                                                    //通过clientSocket接收数据  
                                                    //int receiveLength = clientSocket.Receive(result);
                                                    //log4netHelper.fatal("Socket",Encoding.ASCII.GetString(result, 0, receiveLength));
                                                    ////Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));
                                                    ////通过 clientSocket 发送数据  

                                                    string sendMessage = "<Root><TransCode>801</TransCode><Date>" + payDate.ToString("yyyyMMdd") + "</Date><BankTrace></BankTrace><BankNumber></BankNumber>"
                                                        + "<idno>" + pcid + "</idno><AMT>" + feeRtn.ToString("f2") + "</AMT><TradeCode>" + flowString + "</TradeCode><PosId>" + posString + "</PosId><Guid>" + out_trade_no + "</Guid></Root>";

                                                    clientSocket.Send(Encoding.GetEncoding("gb18030").GetBytes(sendMessage));
                                                    log4netHelper.info("Notify", "Socket：" + "发送");

                                                    Thread.Sleep(300);
                                                    string recStr = "";
                                                    byte[] recBytes = new byte[4096];
                                                    int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
                                                    recStr = Encoding.GetEncoding("gb18030").GetString(recBytes, 0, bytes);
                                                    //recStr += Encoding.Default.GetString(recBytes, 0, bytes);
                                                    recStr = recStr.Substring(10, recStr.Length - 10);
                                                    XmlDocument xmlDoc = new XmlDocument();
                                                    xmlDoc.LoadXml(recStr);
                                                    //商户订单号
                                                    recStr = xmlDoc.SelectSingleNode("/Response/TransReturn").InnerText;


                                                    xmlDoc = null;
                                                    log4netHelper.info("Notify", "Socket：" + "接受，TransReturn：" + recStr);
                                                }

                                                content = "成功发送Socket";
                                     
                                                isSuccess = true;
                                            }
                                            catch (Exception e)
                                            {
                                                sqlTransaction.Rollback();
                                                content = "更新日志表出错" + e.Message;
                                                isSuccess = false;

                                                //log4netHelper.info("Notify", );
                                            }
                                            finally
                                            {
                                                if (null != sqlTransaction)
                                                {
                                                    sqlTransaction.Dispose();
                                                    sqlTransaction = null;
                                                }
                                                if (null != clientSocket)
                                                {
                                                    clientSocket.Shutdown(SocketShutdown.Both);
                                                    clientSocket.Close();
                                                }
                                            }

                                    }
                                    else
                                    {
                                        content = "无该订单信息";
                                        isSuccess = false;
                                        //log4netHelper.info("Notify", "End:无该订单信息！");
                                        //return "无该订单信息";
                                    }

                                }
                                else
                                {
                                    content = "BankMsgSwitch表记录有误！";
                                    isSuccess = false;
                                    //log4netHelper.info("Notify", "BankMnyFlow、BankTradeTitle、BankMsgSwitch三个表记录有误！");
                                    //return "BankMnyFlow、BankTradeTitle、BankMsgSwitch三个表记录有误！";
                                }

                            }
                            catch (Exception e)
                            {
                                content = "操作数据库有误：" + e.Message;
                                isSuccess = false;
                                log4netHelper.info("Notify", "操作数据库有误：" + e.Message);
                            }
                            finally
                            {
                                if (null != reader)
                                {
                                    reader.Dispose();
                                }
                                if (null != cmd)
                                {
                                    cmd.Dispose();
                                }
                                if (null != cmd2)
                                {
                                    cmd2.Dispose();
                                }
                                sqlCnt.Close();
                                sqlCnt2.Close();
                                sqlCnt2 = null;
                                sqlCnt = null;
                                
                            }
                            log4netHelper.info("Notify", "End:----");
                        }
                        else
                        {
                            content = "GUID有误";
                            isSuccess = false;
                            //log4netHelper.info("Notify", "End:GUID有误！");
                            //return "GUID有误";
                        }

                    }
                    else
                    {
                        content = "交易状态为其他";
                        isSuccess = false;
                        //log4netHelper.info("Notify", "End:交易状态为其他");
                        //return "交易状态为其他";
                    }


                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    content = "验证失败";
                    isSuccess = false;
                    //log4netHelper.info("Notify:", "End:验证失败！");
                    //return "验证失败";
                }
            }
            else
            {
                //log4netHelper.info("Notify:", "End:无通知参数");
                //return "无通知参数";
                content = "无通知参数:" + sPara.Count;
                isSuccess = false;
            }

            if (isSuccess)
            {
                log4netHelper.info("Notify", "Success:" + content);
                log4netHelper.info("Notify:", "    ");
                return "success";
            }
            else
            {
                log4netHelper.info("Notify:", "Fail:" + content);
                log4netHelper.info("Notify:", "    ");
                return "fail";
            }
            


        }


        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        [ValidateInput(false)]
        public Dictionary<string, string> GetRequestPost(NameValueCollection rtnForm)
        {
            
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = rtnForm;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;
            try
            {
                for (i = 0; i < requestItem.Length; i++)
                {
                    sArray.Add(requestItem[i], coll[requestItem[i]]);
                }
            }
            catch (Exception e)
            {
                log4netHelper.error("GetRequestPost","--3--异步通知：绑定出错！");
            }
            

            return sArray;
        }




        [Route("PaymentPage/{guidString}/{mrn}/{fee}/{flowNo}")]
        public ActionResult Index(string guidString, long mrn, string flowNo, decimal fee)
        {
            int state = -1;
            Guid guid = new Guid();
            try
            {
                guid = new Guid(guidString);
            }
            catch (Exception e)
            {
                //取不到支付宝预交款订单信息
                ViewBag.errorMessage = "输入错误的订单号！";
                return View("Error");
            }


            Recharge rechargeInfo = new Recharge();

            rechargeInfo.mrn = mrn;
            rechargeInfo.flowid = guidString;
            rechargeInfo.fee = fee;
            rechargeInfo.desc = flowNo;

            Session.Add("rechargeInfo", rechargeInfo);

            state = 1;


            switch (state)
            {
                case 1:
                    //正常
                    return View("View",rechargeInfo);
                    break;
                case -2:
                    //取不到支付宝预交款订单信息
                    ViewBag.errorMessage = "获取不到支付宝预交款订单信息！";
                    return View("Error");
                    break;
                case -1:
                    //获取相应支付宝预交款日志信息
                    ViewBag.errorMessage = "获取相应支付宝预交款日志信息！";
                    return View("Error");
                    break;
                case -3:
                    //操作数据库有误
                    ViewBag.errorMessage = "操作数据库有误！";
                    return View("Error");
                    break;

                default:
                    //其他
                    ViewBag.errorMessage = "其他操作有误！";
                    return View("Error");
                    break;
            }
        }

        [Route("Error")]
        public ActionResult Error()
        {
            ViewBag.errorMessage = "抱歉，预交款充值失败！请重新尝试或则前往人工窗口充值预交款。";
            ViewBag.errorTitle = "预交款充值失败";
            return View("Error");
        }

    }
}