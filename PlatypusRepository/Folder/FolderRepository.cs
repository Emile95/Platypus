using PlatypusRepository.Folder.Operator;

namespace PlatypusRepository.Folder
{
    public class FolderRepository<EntityType> : IRepository<EntityType, string>
        where EntityType : class
    {
        private readonly IRepositoryAddOperator<EntityType> _addOperator;
        private readonly IRepositoryUpdateOperator<EntityType> _updateOperator;
        private readonly IRepositoryRemoveOperator<EntityType, string> _removeOperator;
        private readonly IRepositoryConsumeOperator<EntityType> _consumeOperator;

        public FolderRepository(string directoryPath)
        {
            RepositoryEntityHandler<EntityType, string> _folderEntityHandler = new RepositoryEntityHandler<EntityType, string>();

            Type entityType = typeof(EntityType);

            _addOperator = new FolderRepositoryAddOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
            _updateOperator = new FolderRepositoryUpdateOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
        }

        public EntityType Add(EntityType entity)
        {
            return _addOperator.Add(entity);
        }

        public EntityType Update(EntityType entity)
        {
            return _updateOperator.Update(entity);
        }

        public void Remove(string key)
        {
            _removeOperator.Remove(key);
        }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }
    }
}
