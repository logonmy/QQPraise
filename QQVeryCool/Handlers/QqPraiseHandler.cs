using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using God.Biz.Persistence;
using God.Log;
using God.Qq.Core;
using God.Qq.Core.Caches;
using God.Qq.Core.Handlers;
using QQVeryCool.Provider;
using Timer = System.Timers.Timer;

namespace QQVeryCool.Handlers
{
    public class QqPraiseHandler
    {
        private QqPraiseHandler()
        {

        }

        static QqPraiseHandler()
        {
            Instance = new QqPraiseHandler();
            //Http请求的并发连接数
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
        }

        public static readonly QqPraiseHandler Instance;

        readonly Timer _timer = new Timer(1000 * 5);

        static readonly object Mylock = new object();

        public void Start()
        {
            QqMsgHandler.Instance.Start(() =>
                {
                    Log4Logger.Info("点赞器开始....");
                    QqMsgLogger.LogInfo("点赞器开始....");
                    HttpServerHandler.Instance.Start();
                    _timer.AutoReset = true;
                    _timer.Enabled = false;  //执行一次
                    _timer.Elapsed += OnMessage;
                    _timer.Start();
                });
        }

        public void Stop()
        {
            HttpServerHandler.Instance.Stop();
            _timer.Stop();
            Log4Logger.Info("点赞器停止....");
            QqMsgLogger.LogInfo("点赞器停止....");
            QqMsgHandler.Instance.Stop();
        }

        private void OnMessage(object source, System.Timers.ElapsedEventArgs e)
        {
            lock (Mylock)
            {
                try
                {
                    var qqs = QqProvider.Instance.ListBy5Mi();
                    foreach (var qq in qqs)
                    {
                        var key = qq.TenantId + "_" + qq.Id;
                        if (!QqPraiseCacheProvider.Instance.Exists(key))
                        {
                            var qqPraiseProvider = new QqPraiseProvider(qq.QqNumber, qq.Pwd);
                            qqPraiseProvider.Id = qq.Id;
                            qqPraiseProvider.TenantId = qq.TenantId;
                            QqPraiseCacheProvider.Instance.Add(key, qqPraiseProvider);
                            qqPraiseProvider.Praise();
                        }
                        else
                        {
                            var qqPraiseProvider = QqPraiseCacheProvider.Instance.Get(key);
                            qqPraiseProvider.Praise();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4Logger.Error(ex.StackTrace, ex);
                    QqMsgLogger.LogError(ex.Message, ex);
                }
            }
        }
    }
}
