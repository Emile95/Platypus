namespace PlatypusRepository
{
    public interface IRepository<EntityType, IDType> : 
        IRepositoryAddOperator<EntityType>, 
        IRepositoryUpdateOperator<EntityType>, 
        IRepositoryRemoveOperator<IDType>, 
        IRepositoryConsumeOperator<EntityType>
        where EntityType : class {}
}
