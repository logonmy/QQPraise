using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using God.Biz.Persistence;
using God.Log;
using NUnit.Framework;
using QQVeryCool.Util;

namespace QQVeryCool.Tests
{
    public class Class1
    {
        [Test]
        public void Point()
        {
            var map = new Bitmap("d:\\getimage.jpg");
            var unCheckobj = new UnCodeAiYing(map);
            string strNum = unCheckobj.getPicnum();
            Console.WriteLine(strNum);
        }

        [Test]
        public void GetPwd()
        {
            ////ptui_checkVC('0','!IIU','\x00\x00\x00\x00\x1d\x2c\x12\x9c','d9b2b0c898711c51d8cd6ae11eb8cd6beefc70d04198716afdd55d7c9c9f5d0fbd709cfcaf70fde67d9807bb62098959','0');
            //var a = NewPasswordHelper.GetPassword("vbshuqizhao12", @"\x00\x00\x00\x00\x1d\x2c\x12\x9c", "!IIU", "false");
            ////yL-*4tGhsUp3QRU8TJiGiT6MtlnlSDEQtv8ZPz62l7tUGwvLltLUG1p5IUMcLEfP*NzZfOBwiblfMPYCZteo0cZ9bquvtS7LbINWZSV6aRkL5Q*bPRJN6z3zwYMPcBBCB-1N*VM6GJSSc7MLBjWFORYFG66n6AOlns8lB0jjJSxjT8Oqdnmnr0T0iLuJAFtyl3N0R1jhpbnKnzM*KCJHVQ__
            //Console.WriteLine(a);
        }

        [Test]
        public void SendMail()
        {
            MailLogger.LogInfo("fds");
        }

        [Test]
        public void SendQqMsg()
        {
            var qq = QqProvider.Instance.GetQqById(1);
        }
    }
}
