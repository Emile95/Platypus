using PaltypusAPI.Sockets.ClientRequest;
using PlatypusAPI.User;

namespace PlatypusAPI.Sockets.ClientRequest
{
    public class UpdateUserClientRequest : ClientRequestBase
    {
        public int UserID { get; set; }
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<UserPermissionFlag> UserPermissionFlags { get; set; }
    }
}
