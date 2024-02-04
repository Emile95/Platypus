using Newtonsoft.Json;
using PlatypusRepository.FolderPath.Abstract;

namespace PlatypusRepository.FolderPath.Json.Operator
{
    public class JsonRepositoryConsumeOperator<EntityType> : FolderPathRepositoryOperator<EntityType>, IRepositoryConsumeOperator<EntityType>
        where EntityType : class
    {
        public JsonRepositoryConsumeOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryConsumeOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryConsumeOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            string[] entityJsonFilePaths = Directory.GetFiles(_repositoryDirectoryPath, "*.json");

            foreach (string entityJsonFilePath in entityJsonFilePaths)
            {
                string json = File.ReadAllText(entityJsonFilePath);
                EntityType entity = JsonConvert.DeserializeObject<EntityType>(json);

                if (entity == null) continue;
                if (condition != null)
                {
                    if (condition(entity)) consumer(entity);
                    continue;
                }
                consumer(entity);
            }
        }
    }
}
