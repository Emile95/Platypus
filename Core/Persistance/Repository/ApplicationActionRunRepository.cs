﻿using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    internal class ApplicationActionRunRepository :
        FolderRepositoryOperator<ApplicationActionRunEntity, int>,
        IRepositoryAddOperator<ApplicationActionRunEntity>
    {
        private readonly IRepositoryAddOperator<ApplicationActionRunEntity> _addOperator;

        internal ApplicationActionRunRepository(string applicationActionGuid)
            : base(ApplicationPaths.GetActionRunsDirectoryPath(applicationActionGuid))
        {
            _addOperator = new FolderRepositoryAddOperator<ApplicationActionRunEntity, int>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public ApplicationActionRunEntity Add(ApplicationActionRunEntity entity)
        {
            return _addOperator.Add(entity);
        }
    }
}