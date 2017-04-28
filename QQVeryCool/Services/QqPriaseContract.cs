using System;
using System.IO;
using System.ServiceModel.Web;
using System.Text;
using God.Model;
using QQVeryCool.Provider;

namespace QQVeryCool.Services
{
    public class QqPriaseContract : IQqPriaseContract
    {
        public Stream List(string tenantId)
        {
            var html = QqPraiseCacheProvider.Instance.ListOfHtml(tenantId);

            return Content(html);
        }

        public Stream Check(string tenantId, string id)
        {
            string html;
            var qqPraiseProvider = QqPraiseCacheProvider.Instance.Get(tenantId + "_" + id);
            if (qqPraiseProvider.CodeStatus == CodeStatus.NotNeedCode)
            {
                html = "<html><br/><br/><br/><br/><center>恭喜您！验证已经通过了。</center><html>";
                return Content(html);
            }
            var codeImage = qqPraiseProvider.Check(false);

            if (qqPraiseProvider.CodeStatus == CodeStatus.NeedCode)
            {
                html = string.Format("<meta http-equiv=\"Cache\" content=\"no-cache\"><center><img src=\"{0}\" /><input type='text' id='code' />" +
                                     "<input type='button' onclick=\"var code=document.getElementById('code');window.open('/QQVeryCool/{1}/Praise/{2}/'+code.value)\" value='提交' /></center>", codeImage, tenantId, id);
            }
            else if (codeImage == "ok.")
            {
                html = "<html><br/><br/><br/><br/><center>恭喜您！验证已经通过了。</center><html>";
            }
            else
            {
                html = "<html><br/><br/><br/><br/><center>很遗憾！check失败了。</center><html>";
            }

            return Content(html);
        }



        public bool Praise(string tenantId, string id, string code)
        {
            try
            {
                var qqPraiseProvider = QqPraiseCacheProvider.Instance.Get(tenantId + "_" + id);
                qqPraiseProvider.Login(code);
                return qqPraiseProvider.RunStatus == RunStatus.Sucess;
            }
            catch (Exception ex)
            {
                God.Log.Log4Logger.Error("点赞失败.", ex);
                return false;
            }
        }

        public Stream GetCodeImage(string tenantId, string id)
        {
            var bytes = new byte[1];
            bytes[0] = 1;
            try
            {
                var qqPraiseProvider = QqPraiseCacheProvider.Instance.Get(tenantId + "_" + id);
                var qqNumber = qqPraiseProvider.UserName;
                bytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\CodeImages\\" + qqNumber + ".jpg");

            }
            catch (Exception ex)
            {
                God.Log.Log4Logger.Error("获取验证码失败", ex);
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