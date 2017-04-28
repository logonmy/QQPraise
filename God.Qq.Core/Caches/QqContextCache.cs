using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using God.Qq.Core.Models;

namespace God.Qq.Core.Caches
{
    public class QqContextCache
    {
        private QqContextCache()
        {

        }

        static QqContextCache()
        {
            Instance = new QqContextCache();
            Cache = new Dictionary<string, QqContext>();
            ObjLock = new object();
        }

        public static readonly QqContextCache Instance;

        private static readonly Dictionary<string, QqContext> Cache;

        private static readonly object ObjLock;

        public void Add(string key, QqContext context)
        {
            lock (ObjLock)
            {
                Cache[key] = context;
            }
        }

        public QqContext Get(string key)
        {
            return Cache[key];
        }

        public bool IsEsists(string key)
        {
            return Cache.Keys.Contains(key);
        }

        public QqContext QqMsgLogConext
        {
            get { return Cache["qqmsglog"]; }
        }
    }
}
