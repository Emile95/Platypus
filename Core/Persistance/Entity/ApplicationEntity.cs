using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration;

namespace Core.Persistance.Entity
{
    internal class ApplicationEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }

        [BinaryFile(
            FileName = "application",
            Extension = "dll")]
        public byte[] AssemblyRaw { get; set; }

        public string DirectoryPath { get; set; }
    }
}
