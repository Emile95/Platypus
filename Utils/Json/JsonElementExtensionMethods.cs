using System.Text.Json;

namespace Utils.Json
{
    public static class JsonElementExtensionMethods
    {
        

        public static object GetObjectByValueKind(this JsonElement element)
        {
            object newValue = null;
            switch (element.ValueKind)
            {
                case JsonValueKind.String: newValue = element.GetString(); break;
                case JsonValueKind.Object:
                    if (element.TryGetDateTime(out DateTime datetime))
                    {
                        newValue = datetime;
                        break;
                    }
                    newValue = JsonHelper.GetDictObjectFromJsonElementsDict(element.Deserialize<Dictionary<string, object>>()); break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    newValue = element.GetBoolean();
                    break;
                case JsonValueKind.Number:
                    if (element.TryGetInt16(out short s))
                    {
                        newValue = s;
                        break;
                    }
                    if (element.TryGetInt32(out int i))
                    {
                        newValue = i;
                        break;
                    }
                    if (element.TryGetInt64(out long l))
                    {
                        newValue = l;
                        break;
                    }
                    if (element.TryGetDouble(out double d))
                    {
                        newValue = d;
                        break;
                    }
                    if (element.TryGetDecimal(out decimal de))
                    {
                        newValue = de;
                        break;
                    }
                    break;
                case JsonValueKind.Array:
                    List<object> newArray = new List<object>();
                    foreach (JsonElement arrayElement in element.EnumerateArray())
                        newArray.Add(GetObjectByValueKind(arrayElement));
                    break;
            }
            return newValue;
        }
    }
}
