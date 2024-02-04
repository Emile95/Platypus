using System.Reflection;

namespace PlatypusRepository.FolderPath.Folder.Abstract
{
    internal interface IFolderEntityPropertyValidator
    {
        bool Validate(PropertyInfo propertyInfo);
    }
}
