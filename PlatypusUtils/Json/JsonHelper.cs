using System.Text.Json;

namespace PlatypusUtils.Json
{
    public static class JsonHelper
    {
        public static Dictionary<string, object> GetDictObjectFromJsonElementsDict(Dictionary<string, object> source)
        {
            Dictionary<string, object> newDict = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> set in source)
            {
                JsonElement element = (JsonElement)set.Value;
                newDict.Add(set.Key, element.GetObjectByValueKind());
            }

            return newDict;
        }
    }
}
