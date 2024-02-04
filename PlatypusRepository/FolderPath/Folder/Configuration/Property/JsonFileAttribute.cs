using Newtonsoft.Json;
using System.Reflection;

namespace PlatypusRepository.FolderPath.Folder.Configuration.Property
{
    public class JsonFileAttribute : FileAttribute
    {
        public JsonFileAttribute()
        {
            Extension = "json";
        }

        public override bool Validate(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsClass == true ||
                   propertyInfo.PropertyType.IsEquivalentTo(typeof(string)) == false;
        }

        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath)
        {
            if (Validate(propertyInfo) == false) return;
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            string jsonObject = File.ReadAllText(filePath);
            object value = JsonConvert.DeserializeObject(jsonObject, propertyInfo.PropertyType);
            propertyInfo.SetValue(obj, value);
        }

        public override void Save(object obj, PropertyInfo propertyInfo, string directoryPath)
        {
            if (Validate(propertyInfo) == false) return;
            string value = JsonConvert.SerializeObject(propertyInfo.GetValue(obj));
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            File.WriteAllText(filePath, value);
        }
    }
}
