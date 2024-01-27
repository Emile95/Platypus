namespace PlatypusAPI.Network.ClientRequest
{
    public class UserConnectionClientRequest : PlatypusClientRequest
    {
        public string ConnectionMethodGuid { get; set; }
        public Dictionary<string, object> Credential { get; set; }
    }
}
