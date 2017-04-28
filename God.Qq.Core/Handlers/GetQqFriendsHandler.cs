using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using God.Qq.Core.Models;
using God.Qq.Core.Utils;
using ScrapySharp.Network;

namespace God.Qq.Core.Handlers
{
    public class GetQqFriendsHandler
    {
        private GetQqFriendsHandler()
        {

        }

        static GetQqFriendsHandler()
        {
            Instance = new GetQqFriendsHandler();
        }

        public static readonly GetQqFriendsHandler Instance;

        public void Run(QqContext context)
        {
            if (context.QqMsgStatus == QqMsgStatus.Fail) return;
            var miniBrowser = (ScrapingBrowser)context["miniBrowser"];
            var ptwebqq = context["ptwebqq"].ToString();
            var qqNumber = context["userName"].ToString();
            var hash = NewPasswordHelper.GetQqHash(qqNumber, ptwebqq);
            var vfwebqq = context["vfwebqq"].ToString();
            var json = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            var result = miniBrowser.NavigateTo(new Uri(Constants.QqMsgFriends), HttpVerb.Post, json);
            var qqMsgFriendResult = JsonConvert<QqMsgFriendResult>.JsonToObject(result);

            var query = from f in qqMsgFriendResult.Result.Friends
                        join i in qqMsgFriendResult.Result.Info on f.Uin equals i.Uin into table1
                        from t1 in table1.DefaultIfEmpty()
                        join m in qqMsgFriendResult.Result.Marknames on f.Uin equals m.Uin into table2
                        from t2 in table2.DefaultIfEmpty()
                        select new Friend()
                            {
                                Uin = f.Uin,
                                Face = t1 == null ? string.Empty : t1.Face,
                                Category = f.Categories,
                                Nick = t1 == null ? string.Empty : t1.Nick,
                                MarkName = t2 == null ? string.Empty : t2.Markname
                            };
            var nick = context["nick"].ToString();
            var uin = query.First(x => x.Nick == nick).Uin;
            context["uin"] = uin;
            context.IsReady = true;
        }
    }
}
