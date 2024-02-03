namespace PlatypusRepository
{
    public interface IRepository<EntityType, IDType> : 
        IRepositoryAddOperator<EntityType>, 
        IRepositoryUpdateOperator<EntityType>, 
        IRepositoryRemoveOperator<EntityType, IDType>, 
        IRepositoryConsumeOperator<EntityType>
        where EntityType : class {}
}
