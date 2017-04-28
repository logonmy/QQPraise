using System;

namespace God.Qq.Core.Models
{
    public class Constants
    {
        static Constants()
        {
            var rad = new Random();
            MsgId = rad.Next(1000000, 100000000);
        }
       /* public const string AppId = "549000912";

        public const string CheckUrl = "http://check.ptlogin2.qq.com/check";

        public const string CodeImageTemplate = "http://captcha.qq.com/getimage?aid={0}&uin={1}&cap_cd={2}";

        public const string LoginPage = "http://ptlogin2.qq.com/login";

        public const string IndexPageTemplate = "http://user.qzone.qq.com/{0}/infocenter";*/

        //public const string QqMsgLoginPage = "http://user.qzone.qq.com/{0}/infocenter";

        public const int ClientId = 123456782;

        public static int MsgId;

        public const string QqMsgCheckUrl = "https://ssl.ptlogin2.qq.com/check";

        public const string QqAppId = "501004106";

        public const string QqMsgCodeImageTemplate = "https://ssl.captcha.qq.com/getimage?aid={0}&r={1}&uin={2}";

        public const string QqMsgLoginPage = "https://ssl.ptlogin2.qq.com/login";

        public const string QqMsgLoginOnline = "https://d.web2.qq.com/channel/login2";

        public const string QqMsgPull = "http://d.web2.qq.com/channel/poll2";

        public const string QqMsgFriends = "http://s.web2.qq.com/api/get_user_friends2";

        public const string QqMsgSendToOne = "http://d.web2.qq.com/channel/send_buddy_msg2";
    }
}
