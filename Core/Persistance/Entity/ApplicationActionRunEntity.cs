using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration.Property;

namespace Core.Persistance.Entity
{
    public class ApplicationActionRunEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }

        [TextFile(
            FileName = "action",
            Extension = "log")]
        public string Log { get; set; }
    }
}
