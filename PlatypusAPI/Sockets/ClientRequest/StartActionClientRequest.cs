using Common.Sockets.ClientRequest;
using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Sockets.ClientRequest
{
    public class StartActionClientRequest : ClientRequestBase
    {
        public ApplicationActionRunParameter Parameters { get; set; }
    }
}
