namespace PlatypusRepository
{
    public interface IRepositoryRemoveOperator<EntityType, IDType>
        where EntityType : class
    {
        void Remove(IDType id);
    }
}
