using PlatypusRepository.Configuration;
using PlatypusRepository.FolderPath.Folder.Configuration.Property;

namespace Core.Persistance.Entity
{
    public class ApplicationEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }

        [BinaryFile(
            FileName = "application",
            Extension = "dll")]
        public byte[] AssemblyRaw { get; set; }
    }
}
