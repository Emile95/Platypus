using System.Reflection;
using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Configuration;


namespace PlatypusRepository.Folder
{
    public class FolderRepositoryEntityHandler<EntityType>
    {
        private readonly Type _entityType;
        private readonly PropertyInfo _propertyInfoEntityId;

        internal FolderRepositoryEntityHandler()
        {
            _entityType = typeof(EntityType);
            PropertyInfo[] propertyInfos = _entityType.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetCustomAttribute<RepositoryEntityIDAttribute>() != null)
                {
                    _propertyInfoEntityId = propertyInfo;
                    break;
                }
            }
        }

        internal string GetFolderName(object entity)
        {
            return _propertyInfoEntityId.GetValue(entity).ToString();
        }

        internal void SetID(object obj, object value)
        {
            _propertyInfoEntityId.SetValue(obj, value);
        }

        internal string GetFolderEntityPath(EntityType entity, string repositoryDirectoryPath)
        {
            string folderName = GetFolderName(entity);
            string entityDirectoryPath = Path.Combine(repositoryDirectoryPath, folderName);
            return entityDirectoryPath; ;
        }

        internal void IterateFolderEntityPropertyAttributes(Type type, Action<FolderEntityPropertyAttribute, PropertyInfo> consumer)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                FolderEntityPropertyAttribute attribute = propertyInfo.GetCustomAttribute<FolderEntityPropertyAttribute>();
                if (attribute != null) consumer(attribute, propertyInfo);
            }
        }

        internal void ResolveClassAttributes(Type type, string entityDirectoryPath)
        {
            IEnumerable<FolderEntityClassAttribute> attributes = type.GetCustomAttributes<FolderEntityClassAttribute>();
            if (attributes == null) return;

            foreach(FolderEntityClassAttribute attribute in attributes)
            {
                IFolderEntityClassResolver resolver = attribute as IFolderEntityClassResolver;
                resolver?.Resolve(entityDirectoryPath);
            }
        }
    }
}
