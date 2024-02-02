using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration.Property;

namespace Core.Persistance.Entity
{
    internal class ApplicationActionRunEntity
    {
        [RepositoryEntityID]
        public int RunNumber { get; set; }

        [TextFile(
            FileName = "action",
            Extension = "log")]
        public string Log { get; set; }
    }
}
