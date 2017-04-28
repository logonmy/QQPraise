using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using God.Qq.Core;
using QQVeryCool.Provider;
using QQVeryCool.Services;

namespace QQVeryCool.Handlers
{
    /// <summary>
    /// 提供restful基础服务
    /// </summary>
    public class HttpServerHandler
    {
        private HttpServerHandler()
        {

        }

        static HttpServerHandler()
        {
            Instance = new HttpServerHandler();
        }

        public static readonly HttpServerHandler Instance;

        readonly WebServiceHost _host = new WebServiceHost(typeof(QqPriaseContract), new Uri("http://localhost:8000"));

        public void Start()
        {
            try
            {
                var binding = new WebHttpBinding();
                _host.AddServiceEndpoint(typeof(IQqPriaseContract), binding, "QQVeryCool");
                _host.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true, FaultExceptionEnabled = true });
                _host.Open();
            }
            catch (Exception ex)
            {
                God.Log.Log4Logger.Error(ex.StackTrace, ex);
                QqMsgLogger.LogError(ex.Message);
            }
            God.Log.Log4Logger.Info("HttpServerHandler启动成功端口8000");
            QqMsgLogger.LogInfo("HttpServerHandler启动成功端口8000");
        }

        public void Stop()
        {
            _host.Close();
            God.Log.Log4Logger.Info("HttpServerHandler停止成功端口8000");
            QqMsgLogger.LogInfo("HttpServerHandler停止成功端口8000");
        }
    }
}
