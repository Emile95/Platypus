using PlatypusRepository.Folder.Operator;
using PlatypusRepository.Json.Operator;

namespace PlatypusRepository.Json
{
    public class JsonRepository<EntityType> : IRepository<EntityType>
        where EntityType : class
    {
        private readonly IRepositoryAddOperator<EntityType> _addOperator;
        private readonly IRepositoryUpdateOperator<EntityType> _updateOperator;
        private readonly IRepositoryRemoveOperator<EntityType> _removeOperator;
        private readonly IRepositoryConsumeOperator<EntityType> _consumeOperator;

        public JsonRepository(string directoryPath)
        {
            RepositoryEntityHandler<EntityType, string> _folderEntityHandler = new RepositoryEntityHandler<EntityType, string>();
            Type entityType = typeof(EntityType);

            _addOperator = new JsonRepositoryAddOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
            _updateOperator = new JsonRepositoryUpdateOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
            _removeOperator = new JsonRepositoryRemoveOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
            _consumeOperator = new JsonRepositoryConsumeOperator<EntityType>(entityType, directoryPath, _folderEntityHandler);
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
