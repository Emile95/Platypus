namespace PlatypusRepository
{
    public interface IRepositoryAddOperator<EntityType>
        where EntityType : class
    {
        EntityType Add(EntityType entity);
    }
}
