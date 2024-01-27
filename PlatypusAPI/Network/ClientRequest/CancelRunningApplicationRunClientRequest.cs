namespace PlatypusAPI.Network.ClientRequest
{
    public class CancelRunningApplicationRunClientRequest : ClientRequestBase
    {
        public string ApplicationRunGuid { get; set; }
    }
}
