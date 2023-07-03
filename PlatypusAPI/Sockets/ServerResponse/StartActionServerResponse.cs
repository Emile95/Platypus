using Common.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Sockets.ServerResponse
{
    public class StartActionServerResponse : ServerResponseBase
    {
        public string StartActionKey { get; set; }
        public ApplicationActionRunResult Result { get; set; }
    }
}
