namespace PlatypusRepository.Repository
{
    public abstract class DirectoryRepository<EntityType> : Repository<EntityType, string>
        where EntityType : IEntity<string>
    {

    }
}
