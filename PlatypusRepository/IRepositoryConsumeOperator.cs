namespace PlatypusRepository
{
    public interface IRepositoryConsumeOperator<EntityType>
        where EntityType : class
    {
        void Consume(Action<EntityType> consumer, Predicate<EntityType> condition = null);
    }
}
