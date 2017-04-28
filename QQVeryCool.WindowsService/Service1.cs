using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using QQVeryCool.Handlers;

namespace QQVeryCool.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            var tr2 = new TextWriterTraceListener(AppDomain.CurrentDomain.BaseDirectory + "\\dlog.txt");
            Trace.Listeners.Add(tr2);
        }

        protected override void OnStart(string[] args)
        {
            QqPraiseHandler.Instance.Start();
        }

        protected override void OnStop()
        {
            QqPraiseHandler.Instance.Stop();
        }
    }
}
