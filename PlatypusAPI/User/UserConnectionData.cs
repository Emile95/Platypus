namespace PlatypusAPI.User
{
    public class UserConnectionData
    {
        public string ConnectionMethodGuid { get; set; }
        public Dictionary<string, object> Credential { get; set; }
    }
}
