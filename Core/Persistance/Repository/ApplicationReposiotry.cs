﻿using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    internal class ApplicationRepository : 
        FolderRepositoryOperator<ApplicationEntity, string>, 
        IRepositoryAddOperator<ApplicationEntity>, 
        IRepositoryConsumeOperator<ApplicationEntity>, 
        IRepositoryRemoveOperator<ApplicationEntity>
    {
        private readonly IRepositoryAddOperator<ApplicationEntity> _addOperator;
        private readonly IRepositoryConsumeOperator<ApplicationEntity> _consumeOperator;
        private readonly IRepositoryRemoveOperator<ApplicationEntity> _removeOperator;

        internal ApplicationRepository()
            : base(ApplicationPaths.APPLICATIONSDIRECTORYPATHS)
        {
            _addOperator = new FolderRepositoryAddOperator<ApplicationEntity, string>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<ApplicationEntity, string>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<ApplicationEntity, string>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public ApplicationEntity Add(ApplicationEntity entity)
        {
            return _addOperator.Add(entity);
        }

        public void Consume(Action<ApplicationEntity> consumer, Predicate<ApplicationEntity> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }

        public void Remove(ApplicationEntity entity)
        {
            _removeOperator.Remove(entity);
        }
    }
}