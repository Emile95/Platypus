using PlatypusAPI.User;

namespace PlatypusAPI.Network.ClientRequest
{
    public class AddUserClientRequest : PlatypusClientRequest
    {
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<UserPermissionFlag> UserPermissionFlags { get; set; }
    }
}
