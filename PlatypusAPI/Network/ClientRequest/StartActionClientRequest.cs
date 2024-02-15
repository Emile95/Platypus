using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ClientRequest
{
    public class StartActionClientRequest : PlatypusClientRequest
    {
        public StartApplicationActionParameter Parameters { get; set; }
    }
}
