namespace PlatypusRepository
{
    public abstract class RepositoryBase<EntityType, IDType> : IRepository<EntityType, IDType>
        where EntityType : class
    {
        protected Type _entityType;
        protected RepositoryEntityHandler<EntityType, IDType> _folderEntityHandler;
        
        protected IRepositoryAddOperator<EntityType> _addOperator;
        protected IRepositoryUpdateOperator<EntityType> _updateOperator;
        protected IRepositoryRemoveOperator<EntityType, IDType> _removeOperator;
        protected IRepositoryConsumeOperator<EntityType> _consumeOperator;

        public RepositoryBase()
        {
            _entityType = typeof(EntityType);
            _folderEntityHandler = new RepositoryEntityHandler<EntityType, IDType>();
        }

        public EntityType Add(EntityType entity)
        {
            return _addOperator.Add(entity);
        }

        public void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null)
        {
            _consumeOperator.Consume(consumer, condition);
        }

        public void Remove(IDType id)
        {
            _removeOperator.Remove(id);
        }

        public EntityType Update(EntityType entity)
        {
            return _updateOperator.Update(entity);
        }
    }
}
