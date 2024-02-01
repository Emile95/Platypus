using System.Reflection;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public class TextFileAttribute : FileAttribute
    {
        public new string Extension { get; set; }

        protected override bool PropertyTypeIsValid(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsEquivalentTo(typeof(string));
        }

        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            string value = File.ReadAllText(filePath);
            propertyInfo.SetValue(obj, value);
        }

        public override void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            string value = propertyInfo.GetValue(obj).ToString();
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            File.WriteAllText(filePath, value);
        }
    }
}
