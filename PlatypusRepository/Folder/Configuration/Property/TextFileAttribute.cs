using System.Reflection;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public class TextFileAttribute : FileAttribute
    {
        public new string Extension { get; set; }

        public override bool Validate(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsEquivalentTo(typeof(string));
        }

        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath)
        {
            if (Validate(propertyInfo) == false) return;
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            string value = File.ReadAllText(filePath);
            propertyInfo.SetValue(obj, value);
        }

        public override void Save(object obj, PropertyInfo propertyInfo, string directoryPath)
        {
            if (Validate(propertyInfo) == false) return;
            object value = propertyInfo.GetValue(obj);
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            File.WriteAllText(filePath, value == null ? "" : value.ToString());
        }
    }
}
