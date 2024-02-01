using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration.Class;

namespace Core.Persistance.Entity
{
    [CreateFolder(Name = "runs")]
    internal class ApplicationActionEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }
    }
}
