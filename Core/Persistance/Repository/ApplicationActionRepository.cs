using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    internal class ApplicationActionRepository :
        FolderRepositoryOperator<ApplicationActionEntity>,
        IRepositoryAddOperator<ApplicationActionEntity>,
        IRepositoryRemoveOperator<ApplicationActionEntity>,
        IRepositoryConsumeOperator<ApplicationActionEntity>
    {
        private readonly IRepositoryAddOperator<ApplicationActionEntity> _addOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity> _removeOperator;
        private readonly IRepositoryConsumeOperator<ApplicationActionEntity> _consumeOperator;

        internal ApplicationActionRepository()
            : base(ApplicationPaths.ACTIONSDIRECTORYPATH)
        {
            _addOperator = new FolderRepositoryAddOperator<ApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<ApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<ApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public ApplicationActionEntity Add(ApplicationActionEntity entity)
        {
            return _addOperator.Add(entity);
        }

        public void Consume(Action<ApplicationActionEntity> consumer, Predicate<ApplicationActionEntity> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }

        public void Remove(ApplicationActionEntity entity)
        {
            _removeOperator.Remove(entity);
        }
    }
}
