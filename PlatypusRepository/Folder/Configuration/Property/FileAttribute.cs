using System.Reflection;
using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public abstract class FileAttribute : FolderEntityPropertyAttribute, IFolderEntityPropertyFetcher, IFolderEntityPropertyResolver, IFolderEntityPropertyValidator
    {
        public string FileName { get; set; }
        public virtual string Extension { get; protected set; }

        public abstract void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath, Func<Type, string, object> recursion = null);
        public abstract void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null);
        public abstract bool Validate(PropertyInfo propertyInfo);
    }
}
