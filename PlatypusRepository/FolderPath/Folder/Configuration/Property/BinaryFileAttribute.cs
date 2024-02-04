using System.Reflection;

namespace PlatypusRepository.FolderPath.Folder.Configuration.Property
{
    public class BinaryFileAttribute : FileAttribute
    {
        public new string Extension { get; set; }

        public override bool Validate(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsEquivalentTo(typeof(byte[]));
        }

        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath)
        {
            if (Validate(propertyInfo) == false) return;
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            byte[] value = File.ReadAllBytes(filePath);
            propertyInfo.SetValue(obj, value);
        }

        public override void Save(object obj, PropertyInfo propertyInfo, string directoryPath)
        {
            if (Validate(propertyInfo) == false) return;
            byte[] value = propertyInfo.GetValue(obj) as byte[];
            string filePath = Path.Combine(directoryPath, FileName + "." + Extension);
            File.WriteAllBytes(filePath, value);
        }
    }
}
