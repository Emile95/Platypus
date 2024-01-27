using PlatypusAPI.User;

namespace PlatypusAPI.Network.ServerResponse
{
    public class UpdateUserServerResponse : PlatypusServerResponse
    {
        public UserAccount UserAccount { get; set; }
    }
}
