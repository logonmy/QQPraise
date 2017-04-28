using System;
using God.Qq.Core.Handlers;

namespace God.Qq.Core
{
    public sealed class QqMsgLogger
    {
        private static readonly string MailTemplate;

        static QqMsgLogger()
        {
            MailTemplate =
                "级别：{0}\\\\n" +
                "时间：{1}\\\\n" +
                "内容：{2}\\\\n" +
                "跟踪：{3}";
        }

        public static void LogError(string msg, Exception ex)
        {
            msg = msg.Replace("\\", "\\\\\\\\");
            msg = msg.Replace("\"", "\\\"");
            string html = string.Format(MailTemplate, "Error", DateTime.Now, msg,
                                        ex == null ? "" : ex.StackTrace.Replace("\\", "\\\\\\\\")).Replace("\"", "\\\"");
            SendQqMsgHandler.Instance.SendMsg(html);
        }

        public static void LogError(string msg)
        {
            LogError(msg, null);
        }

        public static void LogInfo(string msg, Exception ex)
        {
            msg = msg.Replace("\\", "\\\\\\\\");
            msg = msg.Replace("\"", "\\\"");
            string html = string.Format(MailTemplate, "Info", DateTime.Now, msg,
                                        ex == null ? "" : ex.StackTrace.Replace("\\", "\\\\\\\\")).Replace("\"", "\\\"");
            SendQqMsgHandler.Instance.SendMsg(html);
        }

        public static void LogInfo(string msg)
        {
            LogInfo(msg, null);
        }
    }
}
