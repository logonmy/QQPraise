using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using God.Model;

namespace QQVeryCool.Provider
{
    internal class QqPraiseCacheProvider
    {
        private QqPraiseCacheProvider()
        {

        }

        static QqPraiseCacheProvider()
        {
            Instance = new QqPraiseCacheProvider();
        }

        public static readonly QqPraiseCacheProvider Instance;

        private static readonly Dictionary<string, QqPraiseProvider> QqPraiseProviders =
            new Dictionary<string, QqPraiseProvider>();

        public bool Exists(string key)
        {
            return QqPraiseProviders.Keys.Contains(key);
        }

        public void Add(string key, QqPraiseProvider qqPraiseProvider)
        {
            QqPraiseProviders[key] = qqPraiseProvider;
        }

        public List<QqPraiseProvider> ListOfNeedCode(string tenantId)
        {
            return QqPraiseProviders.Where(x => x.Key.Contains(tenantId + "_") && x.Value.CodeStatus == CodeStatus.NeedCode).
                Select(x => x.Value).ToList();
        }

        public string ListOfHtml(string tenantId)
        {
            var qqPraiseProviders = ListOfNeedCode(tenantId);
            var html = "<html>";
            foreach (var qqPraiseProvider in qqPraiseProviders)
            {
                html += string.Format("<a href='#' onclick=\"window.open('/QQVeryCool/{0}/Check/{1}')\">",tenantId,qqPraiseProvider.Id) + qqPraiseProvider.UserName + "</a><br/>";
            }
            html += "</html>";
            return html;
        }

        public QqPraiseProvider Get(string key)
        {
            return QqPraiseProviders[key];
        }
    }
}
