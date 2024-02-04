using System.Reflection;

namespace PlatypusRepository.FolderPath.Folder.Abstract
{
    internal interface IFolderEntityPropertyFetcher
    {
        void Fetch(object obj, PropertyInfo propertyInfo, string directoryPath);
    }
}
