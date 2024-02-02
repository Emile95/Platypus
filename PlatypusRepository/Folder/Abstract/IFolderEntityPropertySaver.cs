using System.Reflection;

namespace PlatypusRepository.Folder.Abstract
{
    internal interface IFolderEntityPropertySaver
    {
        void Save(object obj, PropertyInfo propertyInfo, string directoryPath);
    }
}
