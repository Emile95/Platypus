using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Configuration;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryAddOperator<EntityType, IDType> : FolderRepositoryOperator<EntityType, IDType>, IRepositoryAddOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryAddOperator(string repositoryDirectoryPath)
            : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryAddOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryAddOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, IDType> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Add(EntityType entity)
        {
            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, _entityHandler.GetID(entity).ToString());
            Directory.CreateDirectory(entityDirectoryPath);

            Resolve(_entityType, entity, entityDirectoryPath);

            _entityHandler.IterateAttributesOfClass<FolderEntityClassAttribute>((attribute) => {
                IFolderEntityClassResolver resolver = attribute as IFolderEntityClassResolver;
                resolver?.Resolve(entityDirectoryPath);
            });

            return entity;
        }

        private void Resolve(Type type, object obj, string entityDirectoryPath)
        {
            _entityHandler.IterateAttributesOfProperties<FolderEntityPropertyAttribute>((attribute, propertyInfo) =>
            {
                IFolderEntityPropertyResolver resolver = attribute as IFolderEntityPropertyResolver;
                resolver?.Resolve(obj, propertyInfo, entityDirectoryPath, Resolve);
            });
        }
    }
}
