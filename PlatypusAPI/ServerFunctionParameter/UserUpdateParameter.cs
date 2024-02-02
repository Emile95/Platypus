using PlatypusAPI.User;

namespace PlatypusAPI.ServerFunctionParameter
{
    public class UserUpdateParameter
    {
        public string Guid { get; set; }
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public UserPermissionFlag UserPermissionFlags { get; set; }
    }
}
