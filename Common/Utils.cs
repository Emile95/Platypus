using Common.Ressource;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common
{
    public static class Utils
    {
        //private static readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public static string GetString(string key, params object[] args)
        {
            return String.Format(Strings.ResourceManager.GetString(key), args);
        }

        public static byte[] GetBytesFromObject(object obj)
        {
            /*Stream stream = new MemoryStream();
            _binaryFormatter.Serialize(stream, obj);

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            stream.Close();*/
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static ObjectType GetObjectFromBytes<ObjectType>(byte[] bytes)
            where ObjectType : class
        {
            /*Stream stream = new MemoryStream(bytes);
            object obj = _binaryFormatter.Deserialize(stream);
            stream.Close();
            return obj as ObjectType;*/
            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<ObjectType>(json);
        }
    }
}
