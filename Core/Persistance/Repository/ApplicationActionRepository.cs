using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    public class ApplicationActionRepository :
        FolderRepositoryOperator<ApplicationActionEntity>,
        IRepositoryAddOperator<ApplicationActionEntity>,
        IRepositoryRemoveOperator<ApplicationActionEntity, string>,
        IRepositoryConsumeOperator<ApplicationActionEntity>
    {
        private readonly IRepositoryAddOperator<ApplicationActionEntity> _addOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity, string> _removeOperator;
        private readonly IRepositoryConsumeOperator<ApplicationActionEntity> _consumeOperator;

        public ApplicationActionRepository()
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

        public void Remove(string id)
        {
            _removeOperator.Remove(id);
        }
    }
}
