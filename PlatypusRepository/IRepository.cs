namespace PlatypusRepository
{
    public abstract class Repository<EntityType, IDType>
        where EntityType : IEntity<IDType>
    {
        public abstract EntityType Add(EntityType entity);
        public abstract EntityType Update(EntityType entity);
        public abstract void Remove(IDType id);
        public abstract void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null);

        public void Remove(EntityType entity)
        {
            Remove(entity.GetID());
        }

        public void Remove(Predicate<EntityType> condition)
        {
            Consume((entity) => {
                Remove(entity);
            }, condition);
        }

        public IEnumerable<EntityType> Update(IEnumerable<EntityType> entities)
        {
            foreach(EntityType entity in entities)
                yield return Update(entity);
        }

        public IEnumerable<EntityType> Add(IEnumerable<EntityType> entities)
        {
            foreach (EntityType entity in entities)
                yield return Add(entity);
        }
    }
}
