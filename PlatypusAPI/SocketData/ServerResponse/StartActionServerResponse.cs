using Common.SocketData.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.SocketData.ServerResponse
{
    public class StartActionServerResponse : ServerResponseBase
    {
        public string StartActionKey { get; set; }
        public ApplicationActionRunResult Result { get; set; }
    }
}
