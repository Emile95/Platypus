using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.FolderPath.Folder.Operator;
using PlatypusRepository.FolderPath.Abstract;

namespace Core.Persistance.Repository
{
    public class RunningApplicationActionRepository :
        FolderPathRepositoryOperator<RunningApplicationActionEntity>,
        IRepositoryAddOperator<RunningApplicationActionEntity>,
        IRepositoryConsumeOperator<RunningApplicationActionEntity>,
        IRepositoryRemoveOperator<RunningApplicationActionEntity, string>
    {
        private readonly IRepositoryAddOperator<RunningApplicationActionEntity> _addOperator;
        private readonly IRepositoryConsumeOperator<RunningApplicationActionEntity> _consumeOperator;
        private readonly IRepositoryRemoveOperator<RunningApplicationActionEntity, string> _removeOperator;

        public RunningApplicationActionRepository()
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

        public void Remove(string id)
        {
            _removeOperator.Remove(id);
        }
    }
}
