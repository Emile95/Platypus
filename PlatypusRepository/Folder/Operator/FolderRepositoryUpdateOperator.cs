﻿using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Configuration;

namespace PlatypusRepository.Folder.Operator
{
    public class FolderRepositoryUpdateOperator<EntityType> : FolderRepositoryOperator<EntityType>, IRepositoryUpdateOperator<EntityType>
        where EntityType : class
    {
        public FolderRepositoryUpdateOperator(string repositoryDirectoryPath)
           : base(typeof(EntityType), repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryUpdateOperator(Type entityType, string repositoryDirectoryPath)
            : base(entityType, repositoryDirectoryPath, new RepositoryEntityHandler<EntityType, string>()) { }

        public FolderRepositoryUpdateOperator(Type entityType, string repositoryDirectoryPath, RepositoryEntityHandler<EntityType, string> folderEntityHandler)
            : base(entityType, repositoryDirectoryPath, folderEntityHandler) { }

        public EntityType Update(EntityType entity)
        {
            string entityDirectoryPath = Path.Combine(_repositoryDirectoryPath, _entityHandler.GetID(entity).ToString());

            _entityHandler.IterateAttributesOfProperties<FolderEntityPropertyAttribute>((attribute, propertyInfo) =>
            {
                IFolderEntityPropertySaver saver = attribute as IFolderEntityPropertySaver;
                saver?.Save(entity, propertyInfo, entityDirectoryPath);
            });

            return entity;
        }
    }
}
