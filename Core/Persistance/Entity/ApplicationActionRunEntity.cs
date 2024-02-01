using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration.Class;

namespace Core.Persistance.Entity
{
    [File(Name = "action",Extension = "log")]
    internal class ApplicationActionRunEntity
    {
        [RepositoryEntityID]
        public int RunNumber { get; set; }
    }
}
