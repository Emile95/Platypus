using System.Reflection;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public class BinaryFileAttribute : FileAttribute
    {
        public new string Extension { get; set; }

        protected override bool PropertyTypeIsValid(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsEquivalentTo(typeof(byte[]));
        }

        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            byte[] value = File.ReadAllBytes(filePath);
            propertyInfo.SetValue(obj, value);
        }

        public override void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            byte[] value = propertyInfo.GetValue(obj) as byte[];
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            File.WriteAllBytes(filePath, value);
        }
    }
}
