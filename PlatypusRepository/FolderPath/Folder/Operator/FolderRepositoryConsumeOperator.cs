using PlatypusRepository.FolderPath.Abstract;
using PlatypusRepository.FolderPath.Folder.Abstract;
using PlatypusRepository.FolderPath.Folder.Configuration;

namespace PlatypusRepository.FolderPath.Folder.Operator
{
    public class FolderRepositoryConsumeOperator<EntityType> : FolderPathRepositoryOperator<EntityType>, IRepositoryConsumeOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryConsumeOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryConsumeOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryConsumeOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            string[] entityDirectoryPaths = Directory.GetDirectories(_repositoryDirectoryPath);

            foreach (string entityDirectoryPath in entityDirectoryPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(entityDirectoryPath);

                EntityType entity = Activator.CreateInstance<EntityType>();

                _entityHandler.IterateAttributesOfProperties<FolderEntityPropertyAttribute>((attribute, propertyInfo) =>
                {
                    IFolderEntityPropertyFetcher fetcher = attribute as IFolderEntityPropertyFetcher;
                    fetcher?.Fetch(entity, propertyInfo, entityDirectoryPath);
                });

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
    }
}
