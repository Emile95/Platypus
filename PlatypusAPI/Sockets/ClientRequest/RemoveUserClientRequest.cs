using PaltypusAPI.Sockets.ClientRequest;

namespace PlatypusAPI.Sockets.ClientRequest
{
    public class RemoveUserClientRequest : ClientRequestBase
    {
        public string ConnectionMethodGuid { get; set; }
        public int ID { get; set; }
    }
}
