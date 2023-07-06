using PaltypusAPI.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Sockets.ServerResponse
{
    public class StartActionServerResponse : ServerResponseBase
    {
        public ApplicationActionRunResult Result { get; set; }
    }
}
