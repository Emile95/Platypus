using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Configuration;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryConsumeOperator<EntityType, IDType> : FolderRepositoryOperator<EntityType, IDType>, IRepositoryConsumeOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryConsumeOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryConsumeOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryConsumeOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, IDType> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            string[] entityDirectoryPaths = Directory.GetDirectories(_repositoryDirectoryPath);

            foreach (string entityDirectoryPath in entityDirectoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(entityDirectoryPath);

                EntityType entity = Fetch(_entityType, entityDirectoryPath) as EntityType;

                _entityHandler.SetID(entity, directoryInfo.Name);

                if (entity == null) continue;
                if (condition != null)
                {
                    if (condition(entity)) consumer(entity);
                    continue;
                }
                consumer(entity);
            }
        }

        internal object Fetch(Type type, string directoryPath)
        {
            object obj = Activator.CreateInstance(type);
            _entityHandler.IterateAttributesOfProperties<FolderEntityPropertyAttribute>((attribute, propertyInfo) => {
                IFolderEntityPropertyFetcher fetcher = attribute as IFolderEntityPropertyFetcher;
                fetcher?.Fetch(obj, propertyInfo, directoryPath, Fetch);
            });
            return obj;
        }
    }
}
