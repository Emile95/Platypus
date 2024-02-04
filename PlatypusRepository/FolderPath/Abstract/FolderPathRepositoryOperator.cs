namespace PlatypusRepository.FolderPath.Abstract
{
    public abstract class FolderPathRepositoryOperator<EntityType>
        where EntityType : class
    {
        protected readonly Type _entityType;
        protected readonly string _repositoryDirectoryPath;
        protected readonly RepositoryEntityHandler<EntityType, string> _entityHandler;

        public FolderPathRepositoryOperator(string repositoryDirectoryPath)
            : this(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderPathRepositoryOperator(Type entityType, string repositoryDirectoryPath)
            : this(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderPathRepositoryOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> entityHandler)
        {
            _entityType = entityType;
            _repositoryDirectoryPath = repositoryDirectoryPath;
            _entityHandler = entityHandler;
        }
    }
}
