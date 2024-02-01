using System.Reflection;

namespace PlatypusRepository.Folder.Abstract
{
    internal interface IFolderEntityPropertyResolver
    {
        void Resolve(object obj, PropertyInfo propertyInfo, string directoryPath, Action<Type, object, string> recursion = null);
    }
}
