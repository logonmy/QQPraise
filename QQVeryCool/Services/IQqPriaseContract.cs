using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace QQVeryCool.Services
{
    [ServiceContract]
    public interface IQqPriaseContract
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{tenantId}/List", Method = "GET")]
        Stream List(string tenantId);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{tenantId}/Check/{id}", Method = "GET")]
        Stream Check(string tenantId, string id);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{tenantId}/Praise/{id}/{code}", Method = "GET")]
        bool Praise(string tenantId, string id, string code);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "{tenantId}/GetCodeImage/{id}", Method = "GET")]
        Stream GetCodeImage(string tenantId, string id);
    }
}