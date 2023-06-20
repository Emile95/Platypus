using PlatypusAPI.User;

namespace PlatypusApplicationFramework.Configuration.User
{
    public class UserConnectEnvironment<CredentialType>
    {
        public UserAccount UserAccount { get; set; }
        public string LoginAttemptMessage { get; set; }
        public CredentialType Credential { get; set; }
    }
}
