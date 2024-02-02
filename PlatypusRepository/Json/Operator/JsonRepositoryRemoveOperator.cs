using PlatypusRepository.Json.Abstract;

namespace PlatypusRepository.Folder.Operator
{
    public class JsonRepositoryRemoveOperator<EntityType> : JsonRepositoryOperator<EntityType>, IRepositoryRemoveOperator<EntityType>
        where EntityType : class
    {
        public JsonRepositoryRemoveOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryRemoveOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Remove(EntityType entity)
        {
            string id = _entityHandler.GetID(entity);
            string entityFilePath = Path.Combine(_repositoryDirectoryPath, id + ".json");
            File.Delete(entityFilePath);
        }
    }
}
