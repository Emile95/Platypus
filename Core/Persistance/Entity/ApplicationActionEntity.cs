using PlatypusRepository.Configuration;

namespace Core.Persistance.Entity
{
    internal class ApplicationActionEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }
    }
}
