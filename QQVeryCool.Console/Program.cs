using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using God.Biz.Persistence;
using God.Log;
using God.Qq.Core;
using God.Qq.Core.Handlers;
using QQVeryCool.Handlers;
using QQVeryCool.Provider;
using ScrapySharp.Network;

namespace QQVeryCool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            QqPraiseHandler.Instance.Start();
            System.Console.Read();
        }
    }
}
