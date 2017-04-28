using System.Collections.Generic;

namespace God.Qq.Core.Models
{
    public class QqContext
    {
        private readonly Dictionary<string, object> _container = new Dictionary<string, object>(); 

        public object this[string key]
        {
            get { return _container[key]; }
            set { _container[key] = value; }
        }

        public QqMsgStatus QqMsgStatus = QqMsgStatus.Init;

        public QqMsgCodeStatus QqMsgCodeStatus = QqMsgCodeStatus.Init;

        public bool IsReady = false;

        public bool Esists(string key)
        {
            return _container.ContainsKey(key);
        }
    }

    public enum QqMsgStatus
    {
        Init=0,
        Login=1,
        Fail=2
    }


    public enum QqMsgCodeStatus
    {
        Init = 0,
        NeedCode = 1,
        NotNeedCode = 2
    }
}