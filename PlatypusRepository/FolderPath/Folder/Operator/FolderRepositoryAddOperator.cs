using PlatypusRepository.FolderPath.Abstract;
using PlatypusRepository.FolderPath.Folder.Abstract;
using PlatypusRepository.FolderPath.Folder.Configuration;

namespace PlatypusRepository.FolderPath.Folder.Operator
{
    public class FolderRepositoryAddOperator<EntityType> : FolderPathRepositoryAddOperator<EntityType>, IRepositoryAddOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryAddOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryAddOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryAddOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Add(EntityType entity)
        {
            string id = _entityHandler.GetID(entity);
            if (string.IsNullOrEmpty(id))
                id = GenerateGuid();

            _entityHandler.SetID(entity, id);

            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, id);
            Directory.CreateDirectory(entityDirectoryPath);

            _entityHandler.IterateAttributesOfProperties<FolderEntityPropertyAttribute>((attribute, propertyInfo) =>
            {
                IFolderEntityPropertySaver saver = attribute as IFolderEntityPropertySaver;
                saver?.Save(entity, propertyInfo, entityDirectoryPath);
            });

            return entity;
        }
    }
}
