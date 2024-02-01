using System.Reflection;
using PlatypusRepository.Folder.Configuration;


namespace PlatypusRepository.Folder
{
    public class FolderEntityHandler<EntityType>
    {
        public void Resolve(Type type, object obj, string entityDirectoryPath)
        {
            IterateProperties(type, (attributeHandler, propertyInfo) => attributeHandler.Resolve(obj, propertyInfo, entityDirectoryPath, Resolve));
        }

        public object Fetch(Type type, string entityDirectoryPath)
        {
            object obj = Activator.CreateInstance(type);
            IterateProperties(type, (attributeHandler, propertyInfo) => attributeHandler.Fetch(obj, propertyInfo, entityDirectoryPath, Fetch));
            return obj;
        }

        public void Create(Type type, string entityDirectoryPath)
        {
            IEnumerable<FolderEntityCreatorAttribute> folderEntityCreators = type.GetCustomAttributes<FolderEntityCreatorAttribute>();
            if (folderEntityCreators != null)
                foreach (FolderEntityCreatorAttribute folderEntityCreator in folderEntityCreators)
                    folderEntityCreator.OnCreate(entityDirectoryPath);
        }

        private void IterateProperties(Type type, Action<FolderEntityPropertyAttribute, PropertyInfo> consumer)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                FolderEntityPropertyAttribute attribute = propertyInfo.GetCustomAttribute<FolderEntityPropertyAttribute>();
                if (attribute != null) consumer(attribute, propertyInfo);
            }
        }
    }
}
