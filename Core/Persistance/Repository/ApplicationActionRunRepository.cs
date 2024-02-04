using Core.Persistance.Entity;
using PlatypusRepository;
using PlatypusRepository.FolderPath.Abstract;
using PlatypusRepository.FolderPath.Folder.Operator;

namespace Core.Persistance.Repository
{
    public class ApplicationActionRunRepository :
        FolderPathRepositoryOperator<ApplicationActionRunEntity>,
        IRepositoryAddOperator<ApplicationActionRunEntity>
    {
        private readonly IRepositoryAddOperator<ApplicationActionRunEntity> _addOperator;

        public ApplicationActionRunRepository(string applicationActionGuid)
            : base(ApplicationPaths.GetActionRunsDirectoryPath(applicationActionGuid))
        {
            _addOperator = new FolderRepositoryAddOperator<ApplicationActionRunEntity>(_entityType, _repositoryDirectoryPath, _entityHandler);
        }

        public ApplicationActionRunEntity Add(ApplicationActionRunEntity entity)
        {
            return _addOperator.Add(entity);
        }
    }
}
