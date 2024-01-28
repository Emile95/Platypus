using PlatypusAPI.User;
using PlatypusNetwork.Request.Data;

namespace PlatypusAPI.Network.ClientRequest
{
    public class PlatypusClientRequest : ClientRequestBase
    {
        public UserAccount UserAccount { get; set; }
    }
}
