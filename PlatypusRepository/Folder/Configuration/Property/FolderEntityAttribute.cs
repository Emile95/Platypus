using System.Reflection;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public class FolderEntityAttribute : FolderEntityPropertyAttribute
    {
        public string FolderName { get; set; }

        protected override bool PropertyTypeIsValid(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsClass == true &&
                   propertyInfo.PropertyType.IsEquivalentTo(typeof(string)) == true;
        }
        public override void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            string childObjectfolderPath = Path.Combine(directoryPath, FolderName);
            object value = recursion(propertyInfo.PropertyType, childObjectfolderPath);
            propertyInfo.SetValue(obj, value);

        }

        public override void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null)
        {
            if (PropertyTypeIsValid(propertyInfo) == false) return;
            object childObject = propertyInfo.GetValue(obj);
            string childObjectfolderPath = Path.Combine(directoryPath, FolderName);
            Directory.CreateDirectory(childObjectfolderPath);
            recursion(childObject.GetType(), childObject, childObjectfolderPath);
        }
    }
}
