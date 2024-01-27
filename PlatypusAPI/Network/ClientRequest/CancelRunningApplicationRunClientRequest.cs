namespace PlatypusAPI.Network.ClientRequest
{
    public class CancelRunningApplicationRunClientRequest : PlatypusClientRequest
    {
        public string ApplicationRunGuid { get; set; }
    }
}
