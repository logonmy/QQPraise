using System.Collections.Generic;
using System.Runtime.Serialization;

namespace God.Qq.Core.Models
{
    [DataContract]
    public class QqMsgFriendResult
    {
        [DataMember(Name = "retcode")]
        public int Retcode { get; set; }

        [DataMember(Name = "result")]
        public ParamResult Result = new ParamResult();

        [DataContract]
        public class ParamResult
        {
            /// 
            /// 分组信息
            /// 
            [DataMember(Name = "categories")]
            public List<ParamCategories> Categories { get; set; }
            /// 
            /// 好友汇总
            /// 
            [DataMember(Name = "friends")]
            public List<ParamFriends> Friends { get; set; }
            /// 
            /// 好友信息
            ///
            [DataMember(Name = "info")]
            public List<ParamInfo> Info { get; set; }
            /// 
            /// 备注
            /// 
            [DataMember(Name = "marknames")]
            public List<ParamMarkNames> Marknames { get; set; }

            /// 
            /// 分组
            /// 
            [DataContract]
            public class ParamCategories
            {
                [DataMember(Name = "index")]
                public string Index { get; set; }

                [DataMember(Name = "sort")]
                public int Sort { get; set; }

                [DataMember(Name = "name")]
                public string Name { get; set; }
            }
            /// 
            /// 好友汇总
            /// 
            [DataContract]
            public class ParamFriends
            {
                [DataMember(Name = "flag")]
                public string Flag { get; set; }

                [DataMember(Name = "uin")]
                public string Uin { get; set; }

                [DataMember(Name = "categories")]
                public string Categories { get; set; }
            }
            /// 
            /// 好友信息
            /// 
            [DataContract]
            public class ParamInfo
            {
                [DataMember(Name = "face")]
                public string Face { get; set; }

                [DataMember(Name = "nick")]
                public string Nick { get; set; }

                [DataMember(Name = "uin")]
                public string Uin { get; set; }
            }
            /// 
            /// 备注
            /// 
            [DataContract]
            public class ParamMarkNames
            {
                [DataMember(Name = "uin")]
                public string Uin { get; set; }

                [DataMember(Name = "markname")]
                public string Markname { get; set; }
            }
        }
    }
}