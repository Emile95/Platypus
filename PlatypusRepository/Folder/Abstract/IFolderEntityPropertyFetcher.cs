using System.Reflection;

namespace PlatypusRepository.Folder.Abstract
{
    internal interface IFolderEntityPropertyFetcher
    {
        void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath);
    }
}
