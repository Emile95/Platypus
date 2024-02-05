namespace PlatypusRepository.FolderPath.Abstract
{
    public class FolderPathRepositoryAddOperator<EntityType> : FolderPathRepositoryOperator<EntityType>
        where EntityType : class
    {
        public FolderPathRepositoryAddOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderPathRepositoryAddOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderPathRepositoryAddOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        protected string GenerateGuid()
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
