using PlatypusAPI.User;

namespace PlatypusFramework.Core.User
{
    public class UserInformation
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public UserPermissionFlag UserPermissionFlag { get; set; }
    }
}
