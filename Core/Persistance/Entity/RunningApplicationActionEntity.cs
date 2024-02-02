using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration.Property;

namespace Core.Persistance.Entity
{
    internal class RunningApplicationActionEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }

        [TextFile(FileName = "actionGuid")]
        public string ActionGuid { get; set; }

        [JsonFile(FileName = "parameters")]
        public Dictionary<string, object> Parameters { get; set; }
    }
}
