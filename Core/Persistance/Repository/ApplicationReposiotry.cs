using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.Folder.Abstract;
using PlatypusRepository.Folder.Operator;

namespace Core.Persistance.Repository
{
    public class ApplicationRepository : 
        FolderRepositoryOperator<ApplicationEntity>, 
        IRepositoryAddOperator<ApplicationEntity>, 
        IRepositoryConsumeOperator<ApplicationEntity>, 
        IRepositoryRemoveOperator<ApplicationEntity, string>
    {
        private readonly IRepositoryAddOperator<ApplicationEntity> _addOperator;
        private readonly IRepositoryConsumeOperator<ApplicationEntity> _consumeOperator;
        private readonly IRepositoryRemoveOperator<ApplicationEntity, string> _removeOperator;

        public ApplicationRepository()
            : base(ApplicationPaths.APPLICATIONSDIRECTORYPATHS)
        {
            _addOperator = new FolderRepositoryAddOperator<ApplicationEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<ApplicationEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<ApplicationEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public ApplicationEntity Add(ApplicationEntity entity)
        {
            return _addOperator.Add(entity);
        }

        public void Consume(Action<ApplicationEntity> consumer, Predicate<ApplicationEntity> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }

        public void Remove(string key)
        {
            _removeOperator.Remove(key);
        }
    }
}
