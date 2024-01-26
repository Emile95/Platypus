using PlatypusAPI.Ressources;
using PlatypusAPI.User;
using PlatypusUtils;
namespace PlatypusAPI.Exceptions
{
    public class UserPermissionException : FactorisableException
    {
        public UserAccount UserAccount { get; set; }
        public UserPermissionException(UserAccount userAccount)
            : base(FactorisableExceptionType.UserNotPermitted, Utils.GetString(Strings.ResourceManager, "UserNotPermitted"))
        {
            UserAccount = userAccount;
        }
        public override object[] GetParameters()
        {
            return new object[] { UserAccount };
        }
    }
}
