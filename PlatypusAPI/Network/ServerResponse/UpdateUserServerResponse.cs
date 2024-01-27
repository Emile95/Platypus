using PlatypusAPI.User;

namespace PlatypusAPI.Network.ServerResponse
{
    public class UpdateUserServerResponse : ServerResponseBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
