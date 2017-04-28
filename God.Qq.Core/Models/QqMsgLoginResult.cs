using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace God.Qq.Core.Models
{
    [DataContract]
    public class QqMsgLoginResult
    {
        [DataMember(Name = "retcode")]
        public int Retcode { get; set; }

        [DataMember(Name = "result")]
        public ParamResult Result { get; set; }

        [DataContract]
        public class ParamResult
        {
            [DataMember(Name = "psessionid")]
            public string PsessionId { get; set; }

            [DataMember(Name = "vfwebqq")]
            public string Vfwebqq { get; set; }

            [DataMember(Name = "uin")]
            public Int64 Uin { get; set; }
        }
    }
}
