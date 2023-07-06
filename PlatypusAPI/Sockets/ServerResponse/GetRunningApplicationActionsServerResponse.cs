using PaltypusAPI.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Sockets.ServerResponse
{
    public class GetRunningApplicationActionsServerResponse : ServerResponseBase
    {
        public List<ApplicationActionRunInfo> ApplicationActionRunInfos { get; set; }
    }
}
