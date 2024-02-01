using PlatypusAPI.ApplicationAction.Run;
using PlatypusRepository.Configuration;
using PlatypusRepository.Folder.Configuration;

namespace Core.Persistance.Entity
{
    internal class ApplicationActionRunEntity
    {
        [RepositoryEntityID]
        public int RunNumber { get; set; }

        [JsonFile(FileName = "runInfo")]
        public ApplicationActionRunInfo RunInfo { get; set; }

        [TextFile(
            FileName = "action",
            Extension = "log"
        )]
        public string ActionLog { get; set; }
    }
}
