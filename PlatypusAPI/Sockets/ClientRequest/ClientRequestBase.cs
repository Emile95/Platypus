using PlatypusAPI.User;

namespace PaltypusAPI.Sockets.ClientRequest
{
    public class ClientRequestBase
    {
        public UserAccount UserAccount { get; set; }
        public string RequestKey { get; set;}
    }
}
