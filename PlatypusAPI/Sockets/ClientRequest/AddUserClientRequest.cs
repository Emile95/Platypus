using Common.Sockets.ClientRequest;

namespace PlatypusAPI.Sockets.ClientRequest
{
    public class AddUserClientRequest : ClientRequestBase
    {
        public string CredentialMethodGUID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
