using PlatypusAPI.User;
using PlatypusNetwork.Request;

namespace PlatypusAPI.Network.ClientRequest
{
    public class PlatypusClientRequest : ClientRequestBase
    {
        public UserAccount UserAccount { get; set; }
        public string RequestKey { get; set; }
    }
}
