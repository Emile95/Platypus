namespace PlatypusAPI.Network.ClientRequest
{
    public class RemoveUserClientRequest : PlatypusClientRequest
    {
        public string ConnectionMethodGuid { get; set; }
        public string Guid { get; set; }
    }
}
