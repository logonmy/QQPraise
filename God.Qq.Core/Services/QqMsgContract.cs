using System;
using System.Configuration;
using System.IO;
using System.ServiceModel.Web;
using System.Text;
using God.Qq.Core.Caches;
using God.Qq.Core.Handlers;
using God.Qq.Core.Models;

namespace God.Qq.Core.Services
{
    public class QqMsgContract : IQqMsgContract
    {
        public Stream Check()
        {
            string html;
            var qqMsgLogConext = QqContextCache.Instance.QqMsgLogConext;
            qqMsgLogConext["needSendMail"] = false;
            if (qqMsgLogConext.QqMsgStatus == QqMsgStatus.Login)
            {
                html = "<html><br/><br/><br/><br/><center>恭喜您！qq已经登录了。</center><html>";
                return Content(html);
            }
            CheckQqHandler.Instance.Run(qqMsgLogConext);

            if (QqMsgCodeStatus.NeedCode == qqMsgLogConext.QqMsgCodeStatus)
            {
                html = string.Format("<meta http-equiv=\"Cache\" content=\"no-cache\"><center><img src=\"/QQMsgLogger/GetCodeImage\" /><input type='text' id='code' />" +
                                     "<input type='button' onclick=\"var code=document.getElementById('code');window.open('/QQMsgLogger/Login/'+code.value)\" value='提交' /></center>");
            }
            else if (QqMsgCodeStatus.NotNeedCode == qqMsgLogConext.QqMsgCodeStatus)
            {
                html = "<html><br/><br/><br/><br/><center>恭喜您！验证已经通过了。</center><html>";
            }
            else
            {
                html = "<html><br/><br/><br/><br/><center>很遗憾！qqmsg check失败了。</center><html>";
            }

            return Content(html);
        }

        public bool Login(string code)
        {
            try
            {
                var qqMsgLogConext = QqContextCache.Instance.QqMsgLogConext;
                qqMsgLogConext["code"] = code;
                LoginQqHandler.Instance.Run(qqMsgLogConext);
                GetQqFriendsHandler.Instance.Run(qqMsgLogConext);
                PullQqMsgHandler.Instance.Run(qqMsgLogConext);
                return qqMsgLogConext.QqMsgStatus == QqMsgStatus.Login;
            }
            catch (Exception ex)
            {
                Log.Log4Logger.Error("qqmsq登录失败.", ex);
                return false;
            }
        }

        public Stream GetCodeImage()
        {
            var bytes = new byte[1];
            bytes[0] = 1;
            try
            {
                var qqNumber = ConfigurationManager.AppSettings["qqfromto"] ?? "1276959707";
                bytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\CodeImages\\qqmsg_" + qqNumber + ".jpg");

            }
            catch (Exception ex)
            {
                Log.Log4Logger.Error("qqmsg获取验证码失败", ex);
            }
            return Content(bytes);
        }

        private Stream Content(byte[] bytes, string contentType = "image/jpeg")
        {
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = contentType;

            var result = new MemoryStream(bytes);

            return result;
        }

        private Stream Content(string html, string contentType = "text/html; charset=utf-8")
        {
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.ContentType = contentType;

            var result = new MemoryStream(Encoding.UTF8.GetBytes(html));

            return result;
        }
    }
}
