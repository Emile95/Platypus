using PlatypusAPI.User;

namespace PlatypusAPI.ServerFunctionParameter
{
    public class UserUpdateParameter
    {
        public int ID { get; set; }
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<UserPermissionFlag> UserPermissionFlags { get; set; }
    }
}
