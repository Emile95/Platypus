using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Configuration;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryUpdateOperator<EntityType, IDType> : FolderRepositoryOperator<EntityType, IDType>, IRepositoryUpdateOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryUpdateOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryUpdateOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, IDType>()) { }

        public FolderRepositoryUpdateOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, IDType> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Update(EntityType entity)
        {
            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, _entityHandler.GetID(entity).ToString());
            Resolve(_entityType, entity, entityDirectoryPath);
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
