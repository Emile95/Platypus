namespace PlatypusAPI.Network.ClientRequest
{
    public class RemoveUserClientRequest : PlatypusClientRequest
    {
        public string ConnectionMethodGuid { get; set; }
        public int ID { get; set; }
    }
}
