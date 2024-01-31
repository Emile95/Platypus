using Newtonsoft.Json;
using System.Reflection;

namespace PlatypusRepository.Folder.Configuration
{
    public class JsonFileAttribute : FileAttribute
    {
        public JsonFileAttribute()
        {
            Extension = "json";
        }

        protected override bool PropertyTypeIsValid(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsClass == true ||
                   propertyInfo.PropertyType.IsEquivalentTo(typeof(string)) == false;
        }

        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            string jsonObject = File.ReadAllText(filePath);
            object value = JsonConvert.DeserializeObject(jsonObject);
            propertyInfo.SetValue(obj, value);
        }

        public override void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            string value = JsonConvert.SerializeObject(propertyInfo.GetValue(obj));
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            File.WriteAllText(filePath, value);
        }
    }
}
