using System.Reflection;
using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Configuration.Property
{
    public abstract class FileAttribute : FolderEntityPropertyAttribute, IFolderEntityPropertyFetcher, IFolderEntityPropertySaver, IFolderEntityPropertyValidator
    {
        public string FileName { get; set; }
        public virtual string Extension { get; protected set; }

        public abstract void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath);
        public abstract void Save(object obj, PropertyInfo propertyInfo, string directoryPath);
        public abstract bool Validate(PropertyInfo propertyInfo);
    }
}
