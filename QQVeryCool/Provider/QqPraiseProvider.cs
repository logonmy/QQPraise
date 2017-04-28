using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using God.Biz.Persistence;
using God.Log;
using God.Model;
using God.Proxy;
using God.Qq.Core;
using HtmlAgilityPack;
using QQVeryCool.Util;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace QQVeryCool.Provider
{
    /// <summary>
    /// qq空间点赞
    /// </summary>
    public class QqPraiseProvider
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }

        private const string CheckUrl = "http://check.ptlogin2.qq.com/check";
        private const string LoginPage = "http://ptlogin2.qq.com/login";
        private string IndexPage = "http://user.qzone.qq.com/{0}/myhome";
        private string PraiseUrl = "http://w.cnc.qzone.qq.com/cgi-bin/likes/internal_dolike_app?g_tk={0}";
        private string CodeImage = "http://captcha.qq.com/getimage?aid={0}&uin={1}&cap_cd={2}";
        //验证码
        private string cap_cd = string.Empty;
        private const string AppId = "549000912";
        private readonly ScrapingBrowser _miniBrowser = new ScrapingBrowser { Encoding = Encoding.UTF8 };

        public RunStatus RunStatus = RunStatus.Init;

        public CodeStatus CodeStatus = CodeStatus.Init;

        private QqPraiseProvider() { }

        public QqPraiseProvider(int qqNumber, string pwd)
        {
            UserName = qqNumber.ToString();
            PassWord = pwd;

            IndexPage = string.Format(IndexPage, qqNumber);
        }

        /// <summary>
        /// 全屏点赞
        /// </summary>
        public void Praise()
        {
            QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Init);

            if (RunStatus != RunStatus.Sucess && CodeStatus != CodeStatus.NotNeedCode)
            {
                Check();
            }
            else
            {
                try
                {
                    _miniBrowser.DownloadString(IndexPage, Praise);
                }
                catch (Exception ex)
                {
                    RunStatus = RunStatus.Fail;
                    QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Fail);

                    CodeStatus = CodeStatus.NeedCode;
                    QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);

                    Log4Logger.Error(UserName + " Login失败!\r\n" + ex.Message, ex);
                    MailLogger.LogError(UserName + " Login失败!\r\n" + ex.Message, ex);
                    QqMsgLogger.LogError(UserName + " Login失败!", ex);
                }
            }

        }
        public string Check(bool needSendMail = true)
        {
            try
            {
                var postForm = new NameValueCollection();
                postForm["uin"] = UserName;
                postForm["appid"] = AppId;
                //todo 研究下
                postForm["r"] = "0.10299430438317358";
                _miniBrowser.NavigateTo(CheckUrl, postForm, Check);
                if (CodeStatus == CodeStatus.NeedCode)
                {
                    var wr = _miniBrowser.DownloadWebResource(new Uri(string.Format(CodeImage, AppId, UserName, cap_cd)));
                    var baseStartupPath = AppDomain.CurrentDomain.BaseDirectory;
                    if (!Directory.Exists(baseStartupPath + "\\CodeImages"))
                    {
                        Directory.CreateDirectory(baseStartupPath + "\\CodeImages");
                    }
                    File.WriteAllBytes(baseStartupPath + "\\CodeImages\\" + UserName + ".jpg", wr.Content.ToArray());
                    if (needSendMail)
                    {
                        //var msq = string.Format(
                        //    "{0} 需要验证码，请尽快输入验证码！<a href='http://123.57.83.216:8000/QQVeryCool/{1}/Check/{2}'>进入</a>",
                        //    UserName, TenantId, Id);
                        //MailLogger.LogInfo(msq);
                        var msq = string.Format(
                            "{0} 需要验证码，请尽快输入验证码！http://123.57.83.216:8000/QQVeryCool/{1}/Check/{2}",
                            UserName, TenantId, Id);
                        QqMsgLogger.LogInfo(msq);
                    }
                    return string.Format("/QQVeryCool/{0}/GetCodeImage/{1}", TenantId, Id);
                }
                else
                {
                    return "ok.";
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Error("Check失败!\r\n" + UserName + "\r\n" + ex.Message, ex);
                QqMsgLogger.LogError("Check失败!" + UserName + ":" + ex.Message, ex);
                return "error.";
            }

        }
        /// <summary>
        ///获取验证信息
        //验证信息格式为：ptui_checkVC('0','!MIW','\x00\x00\x00\x00\x9a\x65\x0f\xd7') 
        //其中分为三部分，第一个值0或1判断是否需要图片验证码
        //                第二个值是默认验证码，若不需要图片验证码，就用此验证码来提交
        //                第三部分是所使用的QQ号码的16进制形式
        /// </summary>
        /// <param name="scrapingBrowser"></param>
        /// <param name="htmlStr"></param>
        private void Check(ScrapingBrowser scrapingBrowser, string htmlStr)
        {
            cap_cd = string.Empty;

            //将验证码信息的三部分存入数组
            int checkCodePosition = htmlStr.IndexOf("(", System.StringComparison.Ordinal) + 1;
            var checkCode = htmlStr.Substring(checkCodePosition, htmlStr.LastIndexOf(")", System.StringComparison.Ordinal) - checkCodePosition);
            var checkArray = checkCode.Replace("'", "").Split(',');  //验证码数组
            if (checkArray[0] == "0")
            {
                CodeStatus = CodeStatus.NotNeedCode;
                QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NotNeedCode);
                Log4Logger.Info(UserName + " 获取验证信息成功! " + htmlStr);

                Login(checkArray[1]);
            }
            else if (checkArray[0] == "1")
            {
                cap_cd = checkArray[1];
                var ruokuaiUser = ConfigurationManager.AppSettings["ruokuaiUser"] ?? "";
                var ruokuaiPwd = ConfigurationManager.AppSettings["ruokuaiPwd"] ?? "";
                var ruokuaiTypeid = ConfigurationManager.AppSettings["ruokuaiTypeid"] ?? "";
                var ruokuaiSoftid = ConfigurationManager.AppSettings["ruokuaiSoftid"] ?? "";
                var ruokuaiSoftkey = ConfigurationManager.AppSettings["ruokuaiSoftkey"] ?? "";
                var param = new Dictionary<object, object>
                    {
                        {"username", ruokuaiUser},
                        {"password", ruokuaiPwd},
                        {"typeid", ruokuaiTypeid},
                        {"timeout", "90"},
                        {"softid", ruokuaiSoftid},
                        {"softkey", ruokuaiSoftkey}
                    };
                var url = string.Format(CodeImage, AppId, UserName, cap_cd);
                var wr = scrapingBrowser.DownloadWebResource(new Uri(url));
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

                        CodeStatus = CodeStatus.NotNeedCode;
                        QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NotNeedCode);
                        Log4Logger.Info(UserName + " 获取验证信息成功! " + htmlStr);
                        QqMsgLogger.LogInfo(UserName + " 若快识别成功! ");
                        //停顿下，否则qq会认为是在攻击它
                        Thread.Sleep(2000);
                        Login(result);
                    }
                    else
                    {
                        CodeStatus = CodeStatus.NeedCode;
                        QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);
                        Log4Logger.Info(UserName + " 需要验证码，请尽快输入验证码！");
                    }
                }
                catch (Exception ex)
                {
                    Log4Logger.Error(ex.Message, ex);

                    CodeStatus = CodeStatus.NeedCode;
                    QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);

                    Log4Logger.Info(UserName + " 需要验证码，请尽快输入验证码！");
                    QqMsgLogger.LogInfo(UserName + " 若快识别失败! " + ex.Message, ex);
                }
            }
            else
            {
                CodeStatus = CodeStatus.NeedCode;
                QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);
                Log4Logger.Error("Check失败!\r\n" + UserName + "\r\n" + htmlStr);
                QqMsgLogger.LogError("Check失败!" + UserName + ":" + htmlStr);
            }

        }
        public void Login(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                var postForm = new NameValueCollection();
                postForm["u"] = UserName;
                postForm["verifycode"] = code;
                postForm["p"] = PasswordHelper.GetPassword(UserName, PassWord, code);
                postForm["aid"] = AppId;
                postForm["u1"] = IndexPage;
                postForm["h"] = "1";
                postForm["t"] = "1";
                postForm["g"] = "1";
                postForm["from_ui"] = "1";
                postForm["ptlang"] = "2052";
                postForm["action"] = "4-17-1422078797748";
                //登录
                _miniBrowser.NavigateTo(LoginPage, postForm, Login);
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="scrapingBrowser"></param>
        /// <param name="htmlStr"></param>
        private void Login(ScrapingBrowser scrapingBrowser, string htmlStr)
        {
            if (htmlStr.Contains("登录成功"))
            {
                Log4Logger.Info(UserName + "登录成功!\r\n" + htmlStr);
                RunStatus = RunStatus.Sucess;
                QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Sucess);

                CodeStatus = CodeStatus.NotNeedCode;
                QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NotNeedCode);
                scrapingBrowser.DownloadString(IndexPage, Praise);
            }
            else
            {
                RunStatus = RunStatus.Fail;
                QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Fail);

                CodeStatus = CodeStatus.NeedCode;
                QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);

                Log4Logger.Error(UserName + " Login失败!\r\n" + htmlStr);
                //MailLogger.LogError(UserName + " Login失败!\r\n" + htmlStr);
                QqMsgLogger.LogError(UserName + " Login失败!" + htmlStr);
            }
        }
        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="scrapingBrowser"></param>
        /// <param name="htmlStr"></param>
        private void Praise(ScrapingBrowser scrapingBrowser, string htmlStr)
        {
            var cookie = scrapingBrowser.GetCookie(new Uri(IndexPage), "skey");
            if (cookie == null)
            {
                RunStatus = RunStatus.Fail;
                QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Fail);

                CodeStatus = CodeStatus.NeedCode;
                QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);

                Log4Logger.Error(UserName + ":skey为空了");
                QqMsgLogger.LogError(UserName + ":skey为空了");
                return;
            }
            var gtk = Gtk.GetGtk(cookie.Value);
            PraiseUrl = string.Format(PraiseUrl, gtk);

            var document = new HtmlDocument();
            document.LoadHtml(htmlStr);

            var list = document.DocumentNode.CssSelect("li.f-single");
            if (!list.Any())
            {
                QqMsgLogger.LogInfo(UserName + " :li.f-single 找不到点赞的模块");
                Log4Logger.Info(UserName + " :li.f-single 找不到点赞的模块.\r\n" + htmlStr);
                if (document.DocumentNode.CssSelect("i.ico_login").Any())
                {
                    RunStatus = RunStatus.Fail;
                    QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Fail);

                    CodeStatus = CodeStatus.NeedCode;
                    QqProvider.Instance.UpdateCodeStatus(Id, CodeStatus.NeedCode);
                }
                return;
            }
            else
            {
                var isContinue = htmlStr.Contains("g_ic_fpfeedsType='friend',");
                if (!isContinue)
                {
                    //其它角色对用户进行点赞评论后会跳转到与我相关页面，这时候再请求一次
                    //QqMsgLogger.LogInfo(UserName + " 有人关注，需要重新定位到主页.");
                    //_miniBrowser.DownloadString(IndexPage, Praise);
                    return;
                }
            }
            foreach (var htmlNode in list)
            {
                if (htmlNode == null || string.IsNullOrWhiteSpace(htmlNode.InnerHtml)) continue;
                try
                {
                    var nameNode = htmlNode.CssSelect("a.f-name");
                    var praiseNode = htmlNode.CssSelect("a.qz_like_btn_v3").FirstOrDefault();
                    if (praiseNode != null && praiseNode.Attributes["data-clicklog"] != null
                        && praiseNode.Attributes["data-clicklog"].Value == "like")
                    {
                        Thread.Sleep(1000);
                        Log4Logger.Info(UserName + "为《" + nameNode.First().InnerHtml + "》点赞!");
                        var unikey = praiseNode.Attributes["data-unikey"].Value;
                        var curkey = praiseNode.Attributes["data-curkey"].Value;
                        var postForm = new NameValueCollection();
                        postForm["qzreferrer"] = IndexPage;
                        postForm["opuin"] = UserName;
                        postForm["unikey"] = unikey;
                        postForm["curkey"] = curkey;
                        postForm["from"] = "1";
                        postForm["appid"] = "311";
                        postForm["typeid"] = "0";
                        postForm["abstime"] = "1423372434";
                        postForm["fid"] = "3611d32392f0d654e5a20900";
                        postForm["active"] = "0";
                        postForm["fupdate"] = "1";
                        scrapingBrowser.NavigateTo(PraiseUrl, postForm, Success, HttpVerb.Post);
                    }
                    else if (praiseNode == null)
                    {
                        QqMsgLogger.LogInfo(UserName + " :a.qz_like_btn_v3 没有找到点赞的按钮");
                        //Log4Logger.Info(UserName + " :a.qz_like_btn_v3 没有找到点赞的按钮.\r\n" + htmlStr);
                    }
                    else if (praiseNode.Attributes["data-clicklog"] == null)
                    {
                        QqMsgLogger.LogInfo(UserName + " :data-clicklog 没有找到点赞的按钮");
                        Log4Logger.Info(UserName + " :data-clicklog 没有找到点赞的按钮.\r\n" + htmlStr);
                    }
                }
                catch (Exception ex)
                {
                    Log4Logger.Error(UserName + " 点赞失败!\r\n" + htmlStr, ex);
                    QqMsgLogger.LogError(UserName + " 点赞失败!" + ex.Message, ex);
                }

            }
            RunStatus = RunStatus.Sucess;
            QqProvider.Instance.UpdateRunStatus(Id, RunStatus.Sucess);
        }

        private void Success(ScrapingBrowser scrapingBrowser, string htmlStr)
        {
            if (htmlStr.Contains("succ"))
            {
                Log4Logger.Info(UserName + " 点赞成功");
            }
            else
            {
                Log4Logger.Error(UserName + " 点赞失败!\r\n" + htmlStr);
                QqMsgLogger.LogError(UserName + " 点赞失败!" + htmlStr);
            }
        }
    }
}
