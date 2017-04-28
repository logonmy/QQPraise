using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using God.Log;
using God.Proxy;
using God.Qq.Core.Models;
using ScrapySharp.Network;

namespace God.Qq.Core.Handlers
{
    public class CheckQqHandler : IHandler
    {
        private CheckQqHandler()
        {

        }

        static CheckQqHandler()
        {
            Instance = new CheckQqHandler();
        }

        public static readonly CheckQqHandler Instance;

        public void Run(QqContext context)
        {
            var miniBrowser = (ScrapingBrowser)context["miniBrowser"];
            var userName = context["userName"].ToString();

            var postForm = new NameValueCollection();
            postForm["uin"] = userName;
            postForm["appid"] = Constants.QqAppId;
            postForm["js_ver"] = "10095";
            postForm["js_type"] = "0";
            postForm["u1"] = "http://w.qq.com/proxy.html&r=0.6158497643191367";
            postForm["r"] = "0.6158497643191367";

            var htmlStr = miniBrowser.NavigateTo(new Uri(Constants.QqMsgCheckUrl), HttpVerb.Get, postForm);

            //将验证码信息的三部分存入数组
            int checkCodePosition = htmlStr.IndexOf("(", StringComparison.Ordinal) + 1;
            var checkCode = htmlStr.Substring(checkCodePosition, htmlStr.LastIndexOf(")", StringComparison.Ordinal) - checkCodePosition);
            var checkArray = checkCode.Replace("'", "").Split(',');  //验证码数组
            context["qq16"] = checkArray[2];
            if (checkArray[0] == "0")
            {
                context["code"] = checkArray[1];
                context.QqMsgCodeStatus = QqMsgCodeStatus.NotNeedCode;
            }
            else if (checkArray[0] == "1")
            {
                //必要的参数
                var url = string.Format(Constants.QqMsgCodeImageTemplate, Constants.QqAppId, "0.8478438374586403", userName);
                var param = new Dictionary<object, object>
                    {
                        {"username", context["ruokuaiUser"]},
                        {"password", context["ruokuaiPwd"]},
                        {"typeid", context["ruokuaiTypeid"]},
                        {"timeout", "90"},
                        {"softid", context["ruokuaiSoftid"]},
                        {"softkey", context["ruokuaiSoftkey"]}
                    };
                var wr = miniBrowser.DownloadWebResource(new Uri(url));
                try
                {
                    string httpResult = RuoKuaiHttp.Post("http://api.ruokuai.com/create.xml", param, wr.Content.ToArray());
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(httpResult);

                    XmlNode idNode = xmlDoc.SelectSingleNode("Root/Id");
                    XmlNode resultNode = xmlDoc.SelectSingleNode("Root/Result");
                    //XmlNode errorNode = xmlDoc.SelectSingleNode("Root/Error");
                    if (resultNode != null && idNode != null)
                    {
                        //var topidid = idNode.InnerText;
                        var result = resultNode.InnerText;
                        context["code"] = result;
                        context.QqMsgCodeStatus = QqMsgCodeStatus.NotNeedCode;
                        //停顿下，否则qq会认为是在攻击它
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    Log4Logger.Error(ex.Message, ex);
                }

                if (string.IsNullOrWhiteSpace((context["code"] ?? "").ToString()))
                {
                    var baseStartupPath = AppDomain.CurrentDomain.BaseDirectory;
                    if (!Directory.Exists(baseStartupPath + "\\CodeImages"))
                    {
                        Directory.CreateDirectory(baseStartupPath + "\\CodeImages");
                    }
                    File.WriteAllBytes(baseStartupPath + "\\CodeImages\\qqmsg_" + userName + ".jpg", wr.Content.ToArray());
                    context.QqMsgCodeStatus = QqMsgCodeStatus.NeedCode;
                    if ((bool)context["needSendMail"])
                    {
                        Log4Logger.Info("QQMsgLogger需要验证码，请尽快输入验证码！");
                        MailLogger.LogInfo("QQMsgLogger需要验证码，请尽快输入验证码！<a href='http://123.57.83.216:8001/QQMsgLogger/Check'>进入</a>");
                    }
                }

            }
            else
            {
                context.QqMsgStatus = QqMsgStatus.Fail;
            }
            context["verifysession"] = miniBrowser.GetCookie(new Uri(Constants.QqMsgCheckUrl), "verifysession").Value;
        }
    }
}