using PlatypusAPI.User;

namespace PlatypusAPI.Network.ServerResponse
{
    public class UserConnectionServerResponse : ServerResponseBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
