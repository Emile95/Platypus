using Common.SocketData.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI.SocketData.ServerResponse
{
    public class UserConnectionServerResponse : ServerResponseBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
