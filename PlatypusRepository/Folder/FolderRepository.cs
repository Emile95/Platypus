using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration;
using System.Reflection;

namespace PlatypusRepository.Folder
{
    public class FolderRepository<EntityType> : Repository<EntityType>
        where EntityType : class
    {
        private readonly Type _entityType;
        private readonly PropertyInfo[] _propertyInfos;
        private readonly DirectoryInfo _directoryInfo;
        private readonly int _entityIdIndex;

        public FolderRepository(string directoryPath)
        {
            _entityType = typeof(EntityType);
            _propertyInfos = _entityType.GetProperties();

            for(int i = 0; i < _propertyInfos.Length; i++)
            {
                if (_propertyInfos[i].GetCustomAttribute<RepositoryEntityIDAttribute>() != null)
                {
                    _entityIdIndex = i;
                    break;
                }
            }

            _directoryInfo = new DirectoryInfo(directoryPath);
        }

        public override EntityType Add(EntityType entity)
        {
            string folderName = GetFolderName(entity);
            string entityDirectoryPath = Path.Combine(_directoryInfo.FullName, folderName);
            Directory.CreateDirectory(entityDirectoryPath);
            Resolve(entity, entityDirectoryPath);
            return entity;
        }

        public override EntityType Update(EntityType entity)
        {
            string folderName = GetFolderName(entity);
            string entityDirectoryPath = Path.Combine(_directoryInfo.FullName, folderName);
            Resolve(entity, entityDirectoryPath);
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
            string[] directoryPaths = Directory.GetDirectories(_directoryInfo.FullName);

            foreach(string directoryPath in directoryPaths)
            {
                EntityType entity = FetchEntity(directoryPath);
                if (condition != null)
                {
                    if (condition(entity))
                        consumer(entity);
                    continue;
                }
                consumer(entity);
            }
        }

        private EntityType FetchEntity(string entityDirectoryPath)
        {
            return null;
        }

        private void Resolve(EntityType entity, string entityDirectoryPath)
        {
            Type entityType = typeof(EntityType);

            PropertyInfo[] propertyInfos = entityType.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                BinaryFileAttribute attribute = propertyInfo.GetCustomAttribute<BinaryFileAttribute>();
            }
        }

        private string GetFolderName(EntityType entity)
        {
            return _propertyInfos[_entityIdIndex].GetValue(entity).ToString();
        }
    }
}
