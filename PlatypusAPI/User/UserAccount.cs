namespace PlatypusAPI.User
{
    public class UserAccount
    {
        public string Guid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserPermissionFlag UserPermissionFlags { get; set; }
    }
}
