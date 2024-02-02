namespace PlatypusRepository.Json.Abstract
{
    public abstract class JsonRepositoryOperator<EntityType>
        where EntityType : class
    {
        protected readonly Type _entityType;
        protected readonly string _repositoryDirectoryPath;
        protected readonly RepositoryEntityHandler<EntityType, string> _entityHandler;

        public JsonRepositoryOperator(string repositoryDirectoryPath)
            : this(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryOperator(Type entityType, string repositoryDirectoryPath)
            : this(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> entityHandler)
        {
            _entityType = entityType;
            _repositoryDirectoryPath = repositoryDirectoryPath;
            _entityHandler = entityHandler;
        }
    }
}
