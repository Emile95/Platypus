using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryRemoveOperator<EntityType> : FolderRepositoryOperator<EntityType>, IRepositoryRemoveOperator<string>
        where EntityType : class
    {
        public FolderRepositoryRemoveOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Remove(string id)
        {
            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, id);
            Directory.Delete(entityDirectoryPath, true);
        }
    }
}
