using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryRemoveOperator<EntityType, IDType> : FolderRepositoryOperator<EntityType, IDType>, IRepositoryRemoveOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryRemoveOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, IDType> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Remove(EntityType entity)
        {
            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, _entityHandler.GetID(entity).ToString());
            Directory.Delete(entityDirectoryPath, true);
        }
    }
}
