using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryUpdateOperator<EntityType> : FolderRepositoryEntityResolver<EntityType>, IRepositoryUpdateOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryUpdateOperator(string repositoryDirectoryPath)
            : base(repositoryDirectoryPath, new FolderRepositoryEntityHandler<EntityType>()) { }

        internal FolderRepositoryUpdateOperator(string repositoryDirectoryPath, FolderRepositoryEntityHandler<EntityType> folderEntityHandler)
            : base(repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Update(EntityType entity)
        {
            string entityDirectoryPath = _folderEntityHandler.GetFolderEntityPath(entity, _repositoryDirectoryPath);
            Resolve(_entityType, entity, entityDirectoryPath);
            return entity;
        }
    }
}
