namespace PlatypusRepository.FolderPath.Abstract
{
    public abstract class FolderPathRepository<EntityType> : RepositoryBase<EntityType, string>
        where EntityType : class
    {
        public FolderPathRepository(string directoryPath)
        {
        }
    }
}
