using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ServerResponse
{
    public class GetRunningApplicationActionsServerResponse : ServerResponseBase
    {
        public List<ApplicationActionRunInfo> ApplicationActionRunInfos { get; set; }
    }
}
