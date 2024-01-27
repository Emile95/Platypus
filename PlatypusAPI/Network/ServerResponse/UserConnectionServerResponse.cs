using PlatypusAPI.User;

namespace PlatypusAPI.Network.ServerResponse
{
    public class UserConnectionServerResponse : PlatypusServerResponse
    {
        public UserAccount UserAccount { get; set; }
    }
}
