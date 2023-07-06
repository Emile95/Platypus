using PaltypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI.Sockets.ServerResponse
{
    public class UserConnectionServerResponse : ServerResponseBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
