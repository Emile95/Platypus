using Newtonsoft.Json;
using System.Text;
using System.Resources;

namespace PlatypusUtils
{
    public static class Utils
    {
        public static string GetString(ResourceManager ressourceManager, string key, params object[] args)
        {
            return String.Format(ressourceManager.GetString(key), args);
        }

        public static byte[] GetBytesFromObject(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static ObjectType GetObjectFromBytes<ObjectType>(byte[] bytes)
            where ObjectType : class
        {
            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<ObjectType>(json);
        }

        public static string GenerateGuidFromEnumerable(IEnumerable<string> existingGuids, List<string> withoutGuids = null)
        {
            Func<string, bool> predicate = withoutGuids == null ? (guid) => existingGuids.Contains(guid) : (guid) => existingGuids.Contains(guid) || withoutGuids.Contains(guid);
            string guid = null;
            do
            {
                guid = Guid.NewGuid().ToString();
            } while (predicate(guid));
            return guid;
        }
    }
}
