using PaltypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;

namespace PlatypusAPI.Sockets.ServerResponse
{
    public class UpdateUserServerResponse : ServerResponseBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
