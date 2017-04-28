using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using God.Qq.Core.Caches;
using God.Qq.Core.Handlers;
using God.Qq.Core.Models;
using God.Qq.Core.Services;
using ScrapySharp.Network;

namespace God.Qq.Core
{
    /// <summary>
    /// qq消息
    /// </summary>
    public class QqMsgHandler
    {
        private QqMsgHandler()
        {

        }

        static QqMsgHandler()
        {
            Instance = new QqMsgHandler();
        }

        public static readonly QqMsgHandler Instance;

        readonly WebServiceHost _host = new WebServiceHost(typeof(QqMsgContract), new Uri("http://localhost:8001"));

        public void Start(Action action)
        {
            try
            {
                var binding = new WebHttpBinding();
                _host.AddServiceEndpoint(typeof(IQqMsgContract), binding, "QQMsgLogger");
                _host.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true, FaultExceptionEnabled = true });
                _host.Open();
                Log.Log4Logger.Info("QqMsgHandler启动成功端口8001");
            }
            catch (Exception ex)
            {
                Log.Log4Logger.Error(ex.StackTrace, ex);
            }
            var miniBrowser = new ScrapingBrowser { Encoding = Encoding.UTF8 };
            var qqNumber = ConfigurationManager.AppSettings["qqfromto"];
            var pwd = ConfigurationManager.AppSettings["qqfromtopwd"];
            var nick = ConfigurationManager.AppSettings["qqsendto"];
            var face = ConfigurationManager.AppSettings["face"];

            var ruokuaiUser = ConfigurationManager.AppSettings["ruokuaiUser"];
            var ruokuaiPwd = ConfigurationManager.AppSettings["ruokuaiPwd"];
            var ruokuaiTypeid = ConfigurationManager.AppSettings["ruokuaiTypeid"];
            var ruokuaiSoftid = ConfigurationManager.AppSettings["ruokuaiSoftid"];
            var ruokuaiSoftkey = ConfigurationManager.AppSettings["ruokuaiSoftkey"];

            var context = new QqContext();
            context["needSendMail"] = true;
            context["miniBrowser"] = miniBrowser;
            context["userName"] = qqNumber;
            context["pwd"] = pwd;
            context["nick"] = nick;
            context["face"] = face;

            context["ruokuaiUser"] = ruokuaiUser;
            context["ruokuaiPwd"] = ruokuaiPwd;
            context["ruokuaiTypeid"] = ruokuaiTypeid;
            context["ruokuaiSoftid"] = ruokuaiSoftid;
            context["ruokuaiSoftkey"] = ruokuaiSoftkey;
            QqContextCache.Instance.Add("qqmsglog", context);
            CheckQqHandler.Instance.Run(context);
            if (context.QqMsgCodeStatus == QqMsgCodeStatus.NotNeedCode)
            {
                LoginQqHandler.Instance.Run(context);
                GetQqFriendsHandler.Instance.Run(context);
                PullQqMsgHandler.Instance.Run(context);
            }
            while (true)
            {
                Thread.Sleep(500);
                if (QqContextCache.Instance.QqMsgLogConext.IsReady)
                {
                    action.Invoke();
                    break;
                }
            }
        }

        public void Stop()
        {
            _host.Close();
            Log.Log4Logger.Info("QqMsgHandler停止成功端口8001");
        }
    }
}
