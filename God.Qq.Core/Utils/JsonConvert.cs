using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace God.Qq.Core.Utils
{
    public class JsonConvert<T>
    {
        public static string ObjectToJson(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, obj);

            byte[] dataBytes = new byte[stream.Length];

            stream.Position = 0;

            stream.Read(dataBytes, 0, (int)stream.Length);

            string json = Encoding.UTF8.GetString(dataBytes);

            return json;
        }

        public static T JsonToObject(string jsonString)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T t1 = (T)serializer.ReadObject(stream);
            return t1;
        }
    }
}