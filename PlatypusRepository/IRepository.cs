namespace PlatypusRepository
{
    public interface IRepository<EntityType> : IRepositoryAddOperator<EntityType>, IRepositoryUpdateOperator<EntityType>, IRepositoryRemoveOperator<EntityType>, IRepositoryConsumeOperator<EntityType>
        where EntityType : class {}
}
