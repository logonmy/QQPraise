using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using God.Qq.Core.Caches;
using God.Qq.Core.Models;
using ScrapySharp.Network;

namespace God.Qq.Core.Handlers
{
    public class SendQqMsgHandler
    {
        private SendQqMsgHandler()
        {

        }

        static SendQqMsgHandler()
        {
            Instance = new SendQqMsgHandler();
        }

        public static readonly SendQqMsgHandler Instance;

        private static readonly object locker = new object();//添加一个对象作为锁

        public void Run(QqContext context)
        {
            var miniBrowser = (ScrapingBrowser)context["miniBrowser"];
            var uin = context["uin"].ToString();
            var msg = context["msg"].ToString();
            var face = context["face"].ToString();
            var psessionid = context["psessionid"].ToString();

            string postData = "{\"to\":" + uin + ",\"content\":\"[\\\"" + msg.Replace(Environment.NewLine, "\\\\n")
                                + "\\\",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":10,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\""
                                + ",\"face\":"+face
                                + ",\"clientid\":" + Constants.ClientId
                                + ",\"msg_id\":" + Constants.MsgId
                                + ",\"psessionid\":\"" + psessionid
                                + "\"}";
            postData = "r=" + HttpUtility.UrlEncode(postData);
            try
            {

                var result = miniBrowser.NavigateTo(new Uri(Constants.QqMsgSendToOne), HttpVerb.Post, postData);
                //Console.WriteLine(result);
                if (result.Contains(".ok"))
                {
                    //todo
                }
            }
            catch (Exception ex)
            {
                Log.Log4Logger.Error(ex.Message, ex);
            }
        }

        public void SendMsg(string msg)
        {
            lock (locker)
            {
                Thread.Sleep(2500);
                var content = QqContextCache.Instance.QqMsgLogConext;
                content["msg"] = msg;
                if (content.QqMsgStatus == QqMsgStatus.Login)
                {
                    Run(content);
                }
                else
                {
                    Log.Log4Logger.Warn("qqmsg下线了，无法发送消息。");
                }
            }

        }
    }
}
