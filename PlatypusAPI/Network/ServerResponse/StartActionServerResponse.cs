using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Network.ServerResponse
{
    public class StartActionServerResponse : PlatypusServerResponse
    {
        public ApplicationActionRunResult Result { get; set; }
    }
}
