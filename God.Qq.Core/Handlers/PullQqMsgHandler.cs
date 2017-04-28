using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using God.Qq.Core.Models;
using ScrapySharp.Network;

namespace God.Qq.Core.Handlers
{
    public class PullQqMsgHandler
    {
        private PullQqMsgHandler()
        {

        }

        static PullQqMsgHandler()
        {
            Instance = new PullQqMsgHandler();
        }

        public static readonly PullQqMsgHandler Instance;


        public void Run(QqContext context)
        {
            if (context.QqMsgStatus == QqMsgStatus.Fail)
            {
                return;
            }
            else
            {
                var task = new Task(() =>
                {
                    do
                    {
                        var miniBrowser = (ScrapingBrowser)context["miniBrowser"];

                        var psessionid = context["psessionid"].ToString();
                        var ptwebqq = context["ptwebqq"].ToString();
                        var json =
                            string.Format("r={{\"ptwebqq\":\"{0}\",\"clientid\":{1},\"psessionid\":\"{2}\",\"key\":\"\"}}",
                                          ptwebqq, Constants.ClientId, psessionid);
                        var result = miniBrowser.NavigateTo(new Uri(Constants.QqMsgPull), HttpVerb.Post, json);
                        //Console.WriteLine(result);
                        if (result.Contains("\"retcode\":121"))
                        {
                            context.QqMsgStatus = QqMsgStatus.Fail;
                            Log.Log4Logger.Error("qqmsg掉线!" + result);
                            Log.MailLogger.LogError("qqmsg掉线!" + result);
                            break;
                        }
                    } while (true);

                });
                task.Start();
            }
        }
    }
}
