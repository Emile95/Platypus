using PlatypusAPI.User;

namespace PlatypusAPI.Network.ServerResponse
{
    public class AddUserServerResponse : ServerResponseBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
