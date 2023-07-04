using Common.Sockets.ClientRequest;

namespace PlatypusAPI.Sockets.ClientRequest
{
    public class CancelRunningApplicationRunClientRequest : ClientRequestBase
    {
        public string ApplicationRunGuid { get; set; }
    }
}
