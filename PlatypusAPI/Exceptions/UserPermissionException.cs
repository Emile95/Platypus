using PlatypusAPI.User;

namespace PlatypusAPI.Exceptions
{
    public class UserPermissionException : FactorisableException
    {
        public UserAccount UserAccount { get; set; }
        public UserPermissionException(UserAccount userAccount)
            : base(FactorisableExceptionType.UserNotPermitted, PlatypusNetwork.Utils.GetString("UserNotPermitted"))
        {
            UserAccount = userAccount;
        }
        public override object[] GetParameters()
        {
            return new object[] { UserAccount };
        }
    }
}
