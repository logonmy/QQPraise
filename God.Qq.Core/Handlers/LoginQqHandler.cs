using System;
using System.Collections.Specialized;
using System.IO;
using God.Qq.Core.Models;
using God.Qq.Core.Utils;
using ScrapySharp.Network;

namespace God.Qq.Core.Handlers
{
    public class LoginQqHandler : IHandler
    {
        private LoginQqHandler()
        {

        }

        static LoginQqHandler()
        {
            Instance = new LoginQqHandler();
        }

        public static readonly LoginQqHandler Instance;

        public void Run(QqContext context)
        {
            var miniBrowser = (ScrapingBrowser)context["miniBrowser"];
            var code = context["code"].ToString();
            if (!string.IsNullOrWhiteSpace(code))
            {
                var userName = context["userName"].ToString();
                var pwd = context["pwd"].ToString();

                var postForm = new NameValueCollection();
                postForm["u"] = userName;
                postForm["verifycode"] = code;
                postForm["p"] = NewPasswordHelper.GetPassword(context["qq16"].ToString(), pwd, code);
                postForm["aid"] = Constants.QqAppId;
                postForm["u1"] = "http://w.qq.com/proxy.html?login2qq=1&webqq_type=10";
                postForm["h"] = "1";
                //postForm["ptredirect"] = "0";
                //postForm["ptlang"] = "2052";
                postForm["webqq_type"] = "10";
                postForm["remember_uin"] = "1";
                postForm["login2qq"] = "1";
                postForm["ptredirect"] = "0";
                postForm["ptlang"] = "2052";
                postForm["daid"] = "164";
                postForm["from_ui"] = "1";
                postForm["pttype"] = "1";
                postForm["action"] = "0-66-182865";
                postForm["fp"] = "loginerroralert";
                postForm["mibao_css"] = "m_webqq";
                postForm["js_type"] = "0";
                postForm["js_ver"] = "10114";
                postForm["pt_verifysession_v1"] = context["verifysession"].ToString();
                //登录
                var htmlStr = miniBrowser.NavigateTo(new Uri(Constants.QqMsgLoginPage), HttpVerb.Get, postForm);
                int checkCodePosition = htmlStr.IndexOf("(", StringComparison.Ordinal) + 1;
                var checkCode = htmlStr.Substring(checkCodePosition, htmlStr.LastIndexOf(")", StringComparison.Ordinal) - checkCodePosition);
                var checkArray = checkCode.Replace("'", "").Split(',');  //验证码数组
                //Console.WriteLine(htmlStr);
                if (htmlStr.Contains("登录成功"))
                {
                    context.QqMsgStatus = QqMsgStatus.Login;
                    var ptwebqq = miniBrowser.GetCookie(new Uri(Constants.QqMsgLoginPage), "ptwebqq");
                    var skey = miniBrowser.GetCookie(new Uri(Constants.QqMsgLoginPage), "skey");
                    context["ptwebqq"] = ptwebqq.Value;
                    context["skey"] = skey.Value;
                    //更新cookie
                    miniBrowser.NavigateTo(new Uri(checkArray[2]), HttpVerb.Get, "");

                    var json =
                        string.Format(
                            "r={{\"ptwebqq\":\"{0}\",\"clientid\":{1},\"psessionid\":\"\",\"status\":\"online\"}}",
                            ptwebqq.Value, Constants.ClientId);
                    var result = miniBrowser.NavigateTo(new Uri(Constants.QqMsgLoginOnline), HttpVerb.Post, json);
                    var qqMsgLoginResult = JsonConvert<QqMsgLoginResult>.JsonToObject(result);
                    if (qqMsgLoginResult.Retcode == 0)
                    {
                        context["vfwebqq"] = qqMsgLoginResult.Result.Vfwebqq;
                        context["psessionid"] = qqMsgLoginResult.Result.PsessionId;
                    }
                    else
                    {
                        context.QqMsgStatus = QqMsgStatus.Fail;
                    }
                    //Console.WriteLine(result);
                }
                else
                {
                    Log.Log4Logger.Info(htmlStr);
                    context.QqMsgStatus = QqMsgStatus.Fail;
                }
            }
            else
            {
                context.QqMsgStatus = QqMsgStatus.Fail;
            }
        }
    }
}