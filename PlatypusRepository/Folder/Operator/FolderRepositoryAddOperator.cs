using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Configuration;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryAddOperator<EntityType> : FolderRepositoryOperator<EntityType>, IRepositoryAddOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryAddOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryAddOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryAddOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) 
        {
            if(Directory.Exists(_repositoryDirectoryPath) == false) 
                Directory.CreateDirectory(_repositoryDirectoryPath);
        }

        public EntityType Add(EntityType entity)
        {
            string newGuid = GenerateGuid();
            _entityHandler.SetID(entity, newGuid);

            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, newGuid);
            Directory.CreateDirectory(entityDirectoryPath);

            _entityHandler.IterateAttributesOfProperties<FolderEntityPropertyAttribute>((attribute, propertyInfo) =>
            {
                IFolderEntityPropertySaver saver = attribute as IFolderEntityPropertySaver;
                saver?.Save(entity, propertyInfo, entityDirectoryPath);
            });

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
