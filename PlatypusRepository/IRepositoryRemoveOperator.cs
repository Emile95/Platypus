namespace PlatypusRepository
{
    public interface IRepositoryRemoveOperator<EntityType>
        where EntityType : class
    {
        void Remove(EntityType entity);
    }
}
