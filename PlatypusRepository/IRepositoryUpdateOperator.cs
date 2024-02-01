namespace PlatypusRepository
{
    public interface IRepositoryUpdateOperator<EntityType>
        where EntityType : class
    {
        EntityType Update(EntityType entity);
    }
}
