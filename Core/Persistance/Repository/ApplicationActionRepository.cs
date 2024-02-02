using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    internal class ApplicationActionRepository :
        FolderRepositoryOperator<ApplicationActionEntity>,
        IRepositoryAddOperator<ApplicationActionEntity>,
        IRepositoryRemoveOperator<ApplicationActionEntity>
    {
        private readonly IRepositoryAddOperator<ApplicationActionEntity> _addOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity> _removeOperator;

        internal ApplicationActionRepository()
            : base(ApplicationPaths.ACTIONSDIRECTORYPATH)
        {
            _addOperator = new FolderRepositoryAddOperator<ApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<ApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public ApplicationActionEntity Add(ApplicationActionEntity entity)
        {
            return _addOperator.Add(entity);
        }

        public void Remove(ApplicationActionEntity entity)
        {
            _removeOperator.Remove(entity);
        }
    }
}
