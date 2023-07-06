using Common.Exceptions;
using PlatypusAPI.User;

namespace PlatypusAPI.Exceptions
{
    public class UserPermissionException : FactorisableException
    {
        public UserAccount UserAccount { get; set; }
        public UserPermissionException(UserAccount userAccount, UserPermissionFlag userPermissionFlag)
            : base(FactorisableExceptionType.UserNotPermitted, Common.Utils.GetString("UserNotPermitted", userPermissionFlag.ToString()))
        {
            UserAccount = userAccount;
        }
        public override object[] GetParameters()
        {
            return new object[] { UserAccount };
        }
    }
}
