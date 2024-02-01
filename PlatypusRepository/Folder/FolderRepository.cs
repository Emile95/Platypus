using PlatypusRepository.Folder.Operator;

namespace PlatypusRepository.Folder
{
    public class FolderRepository<EntityType, IDType> : IRepository<EntityType>
        where EntityType : class
    {
        private readonly IRepositoryAddOperator<EntityType> _addOperator;
        private readonly IRepositoryUpdateOperator<EntityType> _updateOperator;
        private readonly IRepositoryRemoveOperator<EntityType> _removeOperator;
        private readonly IRepositoryConsumeOperator<EntityType> _consumeOperator;

        public FolderRepository(string directoryPath)
        {
            RepositoryEntityHandler<EntityType, IDType> _folderEntityHandler = new RepositoryEntityHandler<EntityType, IDType>();

            Type entityType = typeof(EntityType);

            _addOperator = new FolderRepositoryAddOperator<EntityType, IDType>(entityType, directoryPath, _folderEntityHandler);
            _updateOperator = new FolderRepositoryUpdateOperator<EntityType, IDType>(entityType, directoryPath, _folderEntityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<EntityType, IDType>(entityType, directoryPath, _folderEntityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<EntityType, IDType>(entityType, directoryPath, _folderEntityHandler);
        }

        public EntityType Add(EntityType entity)
        {
            return _addOperator.Add(entity);
        }

        public EntityType Update(EntityType entity)
        {
            return _updateOperator.Update(entity);
        }

        public void Remove(EntityType entity)
        {
            _removeOperator.Remove(entity);
        }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }
    }
}
