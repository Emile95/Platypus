using PlatypusRepository.Configuration;

namespace Core.Persistance.Entity
{
    public class ApplicationActionEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }
    }
}
