using PlatypusRepository.FolderPath.Abstract;

namespace PlatypusRepository.FolderPath.Folder.Operator
{
    public class FolderRepositoryRemoveOperator<EntityType> : FolderPathRepositoryOperator<EntityType>, IRepositoryRemoveOperator<EntityType, string>
        where EntityType : class
    {
        public FolderRepositoryRemoveOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Remove(string key)
        {
            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, key);
            Directory.Delete(entityDirectoryPath, true);
        }
    }
}
