namespace PlatypusRepository.Folder.Abstract
{
    public abstract class FolderRepositoryOperator<EntityType>
        where EntityType : class
    {
        protected readonly Type _entityType;
        protected readonly string _repositoryDirectoryPath;
        protected readonly FolderRepositoryEntityHandler<EntityType> _folderEntityHandler;

        public FolderRepositoryOperator(string repositoryDirectoryPath)
            : this(repositoryDirectoryPath, new FolderRepositoryEntityHandler<EntityType>()) { }

        internal FolderRepositoryOperator(string repositoryDirectoryPath, FolderRepositoryEntityHandler<EntityType> folderEntityHandler)
        {
            _entityType = typeof(EntityType);
            _repositoryDirectoryPath = repositoryDirectoryPath;
            _folderEntityHandler = folderEntityHandler;
        }
    }
}
