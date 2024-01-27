using PlatypusAPI.User;

namespace PlatypusAPI.Network.ClientRequest
{
    public class ClientRequestBase
    {
        public UserAccount UserAccount { get; set; }
        public string RequestKey { get; set; }
    }
}
