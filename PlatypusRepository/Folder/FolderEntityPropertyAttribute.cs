using System.Reflection;

namespace PlatypusRepository.Folder
{
    public abstract class FolderEntityPropertyAttribute : Attribute
    {
        protected abstract bool PropertyTypeIsValid(PropertyInfo propertyInfo);
        public abstract void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null);
        public abstract void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null);
    }
}
