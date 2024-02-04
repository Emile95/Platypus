using PlatypusRepository.FolderPath.Abstract;

namespace PlatypusRepository.FolderPath.Json.Operator
{
    public class JsonRepositoryRemoveOperator<EntityType> : FolderPathRepositoryOperator<EntityType>, IRepositoryRemoveOperator<EntityType, string>
        where EntityType : class
    {
        public JsonRepositoryRemoveOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Remove(string id)
        {
            string entityFilePath = Path.Combine(_repositoryDirectoryPath, id + ".json");
            File.Delete(entityFilePath);
        }
    }
}
