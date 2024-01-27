using PlatypusAPI.User;

namespace PlatypusAPI.Network.ServerResponse
{
    public class AddUserServerResponse : PlatypusServerResponse
    {
        public UserAccount UserAccount { get; set; }
    }
}
