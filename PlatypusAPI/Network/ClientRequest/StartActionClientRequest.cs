using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ClientRequest
{
    public class StartActionClientRequest : PlatypusClientRequest
    {
        public ApplicationActionRunParameter Parameters { get; set; }
    }
}
