using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace God.Qq.Core.Services
{
    [ServiceContract]
    public interface IQqMsgContract
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "Check", Method = "GET")]
        Stream Check();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "Login/{code}", Method = "GET")]
        bool Login(string code);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetCodeImage", Method = "GET")]
        Stream GetCodeImage();
    }
}