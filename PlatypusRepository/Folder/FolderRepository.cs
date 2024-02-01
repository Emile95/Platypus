using PlatypusRepository.Folder.Operator;

namespace PlatypusRepository.Folder
{
    public class FolderRepository<EntityType> : IRepository<EntityType>
        where EntityType : class
    {
        private readonly IRepositoryAddOperator<EntityType> _addOperator;
        private readonly IRepositoryUpdateOperator<EntityType> _updateOperator;
        private readonly IRepositoryRemoveOperator<EntityType> _removeOperator;
        private readonly IRepositoryConsumeOperator<EntityType> _consumeOperator;

        public FolderRepository(string directoryPath)
        {
            FolderRepositoryEntityHandler<EntityType> _folderEntityHandler = new FolderRepositoryEntityHandler<EntityType>();

            _addOperator = new FolderRepositoryAddOperator<EntityType>(directoryPath, _folderEntityHandler);
            _updateOperator = new FolderRepositoryUpdateOperator<EntityType>(directoryPath, _folderEntityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<EntityType>(directoryPath, _folderEntityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<EntityType>(directoryPath, _folderEntityHandler);
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
