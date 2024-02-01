using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryRemoveOperator<EntityType> : FolderRepositoryOperator<EntityType>, IRepositoryRemoveOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryRemoveOperator(string repositoryDirectoryPath)
            : base(repositoryDirectoryPath, new FolderRepositoryEntityHandler<EntityType>()) { }

        public FolderRepositoryRemoveOperator(string repositoryDirectoryPath, FolderRepositoryEntityHandler<EntityType> folderEntityHandler)
            : base(repositoryDirectoryPath, folderEntityHandler) { }

        public void Remove(EntityType entity)
        {
            string entityDirectoryPath = _folderEntityHandler.GetFolderEntityPath(entity, _repositoryDirectoryPath);
            Directory.Delete(entityDirectoryPath, true);
        }
    }
}
