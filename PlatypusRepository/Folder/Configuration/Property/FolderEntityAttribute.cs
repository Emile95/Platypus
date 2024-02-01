using System.Reflection;
using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public class FolderEntityAttribute : FolderEntityPropertyAttribute, IFolderEntityPropertyFetcher, IFolderEntityPropertyResolver, IFolderEntityPropertyValidator
    {
        public string FolderName { get; set; }

        public bool Validate(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsClass == true &&
                   propertyInfo.PropertyType.IsEquivalentTo(typeof(string)) == true;
        }
        public void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null)
        {
            if (Validate(propertyInfo) == false) return;
            string childObjectfolderPath = Path.Combine(directoryPath, FolderName);
            object value = recursion(propertyInfo.PropertyType, childObjectfolderPath);
            propertyInfo.SetValue(obj, value);

        }

        public void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null)
        {
            if (Validate(propertyInfo) == false) return;
            object childObject = propertyInfo.GetValue(obj);
            string childObjectfolderPath = Path.Combine(directoryPath, FolderName);
            Directory.CreateDirectory(childObjectfolderPath);
            recursion(childObject.GetType(), childObject, childObjectfolderPath);
        }
    }
}
