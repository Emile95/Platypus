using System.Reflection;

namespace PlatypusRepository.Folder.Abstract
{
    internal interface IFolderEntityPropertyValidator
    {
        bool Validate(PropertyInfo propertyInfo);
    }
}
