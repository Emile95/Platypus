namespace PlatypusRepository.Folder.Abstract
{
    public abstract class FolderRepositoryEntityResolver<EntityType> : FolderRepositoryOperator<EntityType>
        where EntityType : class
    {
        internal FolderRepositoryEntityResolver(string repositoryDirectoryPath)
            : base(repositoryDirectoryPath, new FolderRepositoryEntityHandler<EntityType>()) { }

        internal FolderRepositoryEntityResolver(string repositoryDirectoryPath, FolderRepositoryEntityHandler<EntityType> folderEntityHandler)
            : base(repositoryDirectoryPath, folderEntityHandler) { }

        internal void Resolve(Type type, object obj, string entityDirectoryPath)
        {
            _folderEntityHandler.IterateFolderEntityPropertyAttributes(type, (attribute, propertyInfo) =>
            {
                IFolderEntityPropertyResolver resolver = attribute as IFolderEntityPropertyResolver;
                resolver?.Resolve(obj, propertyInfo, entityDirectoryPath, Resolve);
            });
        }
    }
}
