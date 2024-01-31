using PlatypusRepository;

namespace Core.Persistance.Entity
{
    public class ApplicationEntity : IEntity<string>
    {
        public string Guid { get; set; }
        public byte[] AssemblyRaw { get; set; }
        public string DirectoryPath { get; set; }

        public string GetID()
        {
            return Guid;
        }
    }
}
