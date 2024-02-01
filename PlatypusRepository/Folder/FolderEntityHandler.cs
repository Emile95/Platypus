using System.Reflection;


namespace PlatypusRepository.Folder
{
    public class FolderEntityHandler<EntityType>
    {
        public void Resolve(Type type, object obj, string directoryPath)
        {
            IterateProperties(type, (attributeHandler, propertyInfo) => attributeHandler.Resolve(obj, propertyInfo, directoryPath, Resolve));
        }

        public object Fetch(Type type, string directoryPath)
        {
            object obj = Activator.CreateInstance(type);
            IterateProperties(type, (attributeHandler, propertyInfo) => attributeHandler.Fetch(obj, propertyInfo, directoryPath, Fetch));
            return obj;
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
