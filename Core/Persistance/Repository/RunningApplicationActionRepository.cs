using Core.Persistance.Entity;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    internal class RunningApplicationActionRepository :
        FolderRepositoryOperator<RunningApplicationActionEntity>,
        IRepositoryAddOperator<RunningApplicationActionEntity>,
        IRepositoryConsumeOperator<RunningApplicationActionEntity>,
        IRepositoryRemoveOperator<RunningApplicationActionEntity>
    {
        private readonly IRepositoryAddOperator<RunningApplicationActionEntity> _addOperator;
        private readonly IRepositoryConsumeOperator<RunningApplicationActionEntity> _consumeOperator;
        private readonly IRepositoryRemoveOperator<RunningApplicationActionEntity> _removeOperator;

        internal RunningApplicationActionRepository()
            : base(ApplicationPaths.RUNNINGACTIONSDIRECTORYPATH)
        {
            _addOperator = new FolderRepositoryAddOperator<RunningApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<RunningApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<RunningApplicationActionEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public RunningApplicationActionEntity Add(RunningApplicationActionEntity entity)
        {
            return _addOperator.Add(entity);
        }

        public void Consume(Action<RunningApplicationActionEntity> consumer, Predicate<RunningApplicationActionEntity> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }

        public void Remove(RunningApplicationActionEntity entity)
        {
            _removeOperator.Remove(entity);
        }
    }
}
