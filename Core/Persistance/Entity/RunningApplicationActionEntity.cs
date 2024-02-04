using PlatypusRepository.Configuration;
using PlatypusRepository.FolderPath.Folder.Configuration.Property;

namespace Core.Persistance.Entity
{
    public class RunningApplicationActionEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }

        [TextFile(FileName = "actionGuid")]
        public string ActionGuid { get; set; }

        [TextFile(FileName = "actionRunGuid")]
        public string ActionRunGuid { get; set; }

        [JsonFile(FileName = "parameters")]
        public Dictionary<string, object> Parameters { get; set; }
    }
}
