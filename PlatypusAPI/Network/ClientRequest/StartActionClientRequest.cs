using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ClientRequest
{
    public class StartActionClientRequest : ClientRequestBase
    {
        public ApplicationActionRunParameter Parameters { get; set; }
    }
}
