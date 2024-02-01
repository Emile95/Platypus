using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryAddOperator<EntityType> : FolderRepositoryEntityResolver<EntityType>, IRepositoryAddOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryAddOperator(string repositoryDirectoryPath)
            : base(repositoryDirectoryPath, new FolderRepositoryEntityHandler<EntityType>()) { }

        public FolderRepositoryAddOperator(string repositoryDirectoryPath, FolderRepositoryEntityHandler<EntityType> folderEntityHandler)
            : base(repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Add(EntityType entity)
        {
            string entityDirectoryPath = _folderEntityHandler.GetFolderEntityPath(entity, _repositoryDirectoryPath);
            Directory.CreateDirectory(entityDirectoryPath);
            Resolve(_entityType, entity, entityDirectoryPath);
            _folderEntityHandler.ResolveClassAttributes(_entityType, entityDirectoryPath);
            return entity;
        }
    }
}
