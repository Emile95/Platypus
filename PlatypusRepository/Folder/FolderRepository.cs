using PlatypusRepository.Configuration;
using System.Reflection;

namespace PlatypusRepository.Folder
{
    public class FolderRepository<EntityType> : Repository<EntityType>
        where EntityType : class
    {
        
        private readonly DirectoryInfo _directoryInfo;
        private readonly FolderEntityHandler<EntityType> _folderEntityHandler;
        private readonly Type _entityType;
        private readonly PropertyInfo _propertyInfoEntityId;

        public FolderRepository(string directoryPath)
        {
            _directoryInfo = new DirectoryInfo(directoryPath);
            _folderEntityHandler = new FolderEntityHandler<EntityType>();

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

            Initialize();
        }

        public override EntityType Add(EntityType entity)
        {
            string folderName = GetFolderName(entity);
            string entityDirectoryPath = Path.Combine(_directoryInfo.FullName, folderName);
            Directory.CreateDirectory(entityDirectoryPath);
            _folderEntityHandler.Resolve(_entityType, entity, entityDirectoryPath);
            return entity;
        }

        public override EntityType Update(EntityType entity)
        {
            string folderName = GetFolderName(entity);
            string entityDirectoryPath = Path.Combine(_directoryInfo.FullName, folderName);
            _folderEntityHandler.Resolve(_entityType, entity, entityDirectoryPath);
            return entity;
        }

        public override void Remove(EntityType entity)
        {
            string folderName = GetFolderName(entity);
            string entityDirectoryPath = Path.Combine(_directoryInfo.FullName, folderName);
            Directory.Delete(entityDirectoryPath, true);
        }

        public override void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            string[] entityDirectoryPaths = Directory.GetDirectories(_directoryInfo.FullName);

            foreach(string entityDirectoryPath in entityDirectoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(entityDirectoryPath);
                EntityType entity = _folderEntityHandler.Fetch(typeof(EntityType), entityDirectoryPath) as EntityType;
                _propertyInfoEntityId.SetValue(entity, directoryInfo.Name);

                if (entity == null) continue;
                if (condition != null)
                {
                    if (condition(entity)) consumer(entity);
                    continue;
                }
                consumer(entity);
            }
        }

        public string GetFolderName(object entity)
        {
            return _propertyInfoEntityId.GetValue(entity).ToString();
        }

        private void Initialize()
        {

        }
    }
}
