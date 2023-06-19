using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.User;

namespace Core.User
{
    public class UserDefinition
    {
        public UserAccount UserAccount { get; set; }
        public IUserCredentialMethod CredentialMethod { get; set; }
    }
}
