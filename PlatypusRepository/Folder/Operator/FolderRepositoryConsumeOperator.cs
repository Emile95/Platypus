using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryConsumeOperator<EntityType> : FolderRepositoryOperator<EntityType>, IRepositoryConsumeOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryConsumeOperator(string repositoryDirectoryPath)
            : base(repositoryDirectoryPath, new FolderRepositoryEntityHandler<EntityType>()) { }

        public FolderRepositoryConsumeOperator(string repositoryDirectoryPath, FolderRepositoryEntityHandler<EntityType> folderEntityHandler)
            : base(repositoryDirectoryPath, folderEntityHandler) { }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            string[] entityDirectoryPaths = Directory.GetDirectories(_repositoryDirectoryPath);

            foreach (string entityDirectoryPath in entityDirectoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(entityDirectoryPath);

                EntityType entity = Fetch(_entityType, entityDirectoryPath) as EntityType;

                _folderEntityHandler.SetID(entity, directoryInfo.Name);

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
            _folderEntityHandler.IterateFolderEntityPropertyAttributes(typeof(EntityType), (attribute, propertyInfo) => {
                IFolderEntityPropertyFetcher fetcher = attribute as IFolderEntityPropertyFetcher;
                fetcher?.Fetch(obj, propertyInfo, directoryPath, Fetch);
            });
            return obj;
        }
    }
}
