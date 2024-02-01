using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration.Class;

namespace Core.Persistance.Entity
{
    [CreateEmptyFile(
        Name = "action",
        Extension = "log")]
    internal class ApplicationActionRunEntity
    {
        [RepositoryEntityID]
        public int RunNumber { get; set; }
    }
}
