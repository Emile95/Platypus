using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ServerResponse
{
    public class StartActionServerResponse : ServerResponseBase
    {
        public ApplicationActionRunResult Result { get; set; }
    }
}
