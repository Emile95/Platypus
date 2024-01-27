using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ServerResponse
{
    public class GetRunningApplicationActionsServerResponse : PlatypusServerResponse
    {
        public List<ApplicationActionRunInfo> ApplicationActionRunInfos { get; set; }
    }
}
