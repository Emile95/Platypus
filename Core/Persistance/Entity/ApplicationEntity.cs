using PlatypusRepository.Configuration;

namespace Core.Persistance.Entity
{
    public class ApplicationEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }
        public byte[] AssemblyRaw { get; set; }
        public string DirectoryPath { get; set; }
    }
}
