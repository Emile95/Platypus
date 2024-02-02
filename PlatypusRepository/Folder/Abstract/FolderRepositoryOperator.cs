namespace PlatypusRepository.Folder.Abstract
{
    public abstract class FolderRepositoryOperator<EntityType>
        where EntityType : class
    {
        protected readonly Type _entityType;
        protected readonly string _repositoryDirectoryPath;
        protected readonly RepositoryEntityHandler<EntityType, string> _entityHandler;

        public FolderRepositoryOperator(string repositoryDirectoryPath)
            : this(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryOperator(Type entityType, string repositoryDirectoryPath)
            : this(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> entityHandler)
        {
            _entityType = entityType;
            _repositoryDirectoryPath = repositoryDirectoryPath;
            _entityHandler = entityHandler;
        }
    }
}
