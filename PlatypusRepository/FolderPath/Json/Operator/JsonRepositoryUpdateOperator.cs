using Newtonsoft.Json;
using PlatypusRepository.FolderPath.Abstract;

namespace PlatypusRepository.FolderPath.Json.Operator
{
    public class JsonRepositoryUpdateOperator<EntityType> : FolderPathRepositoryOperator<EntityType>, IRepositoryUpdateOperator<EntityType>
        where EntityType : class
    {
        public JsonRepositoryUpdateOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryUpdateOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryUpdateOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Update(EntityType entity)
        {
            string id = _entityHandler.GetID(entity);
            string jsonFilePath = Path.Combine(_repositoryDirectoryPath, id + ".json");

            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(jsonFilePath, json);

            return entity;
        }
    }
}
