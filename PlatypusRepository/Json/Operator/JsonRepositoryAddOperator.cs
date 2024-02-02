using Newtonsoft.Json;
using PlatypusRepository.Json.Abstract;

namespace PlatypusRepository.Json.Operator
{
    public class JsonRepositoryAddOperator<EntityType> : JsonRepositoryOperator<EntityType>, IRepositoryAddOperator<EntityType>
        where EntityType : class
    {
        public JsonRepositoryAddOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryAddOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public JsonRepositoryAddOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) 
        {
            if(Directory.Exists(_repositoryDirectoryPath) == false) 
                Directory.CreateDirectory(_repositoryDirectoryPath);
        }

        public EntityType Add(EntityType entity)
        {
            string id = _entityHandler.GetID(entity);
            if(string.IsNullOrEmpty(id))
                id = GenerateGuid();

            _entityHandler.SetID(entity, id);

            string jsonFilePath = Path.Combine(_repositoryDirectoryPath, id + ".json");

            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(jsonFilePath, json);

            return entity;
        }

        private string GenerateGuid()
        {
            string[] directories = Directory.GetDirectories(_repositoryDirectoryPath);

            HashSet<string> existingGuids = new HashSet<string>();
            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                existingGuids.Add(directoryInfo.Name);
            }

            string guid = Guid.NewGuid().ToString();
            while (existingGuids.Contains(guid))
                guid = Guid.NewGuid().ToString();

            return guid;
        }
    }
}
