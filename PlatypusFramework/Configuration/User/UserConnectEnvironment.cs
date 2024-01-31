using PlatypusFramework.Core.User;

namespace PlatypusFramework.Configuration.User
{
    public class UserConnectEnvironment<CredentialType>
    {
        public List<UserInformation> Users { get; set; }
        public UserInformation CorrespondingUser { get; set; }
        public string LoginAttemptMessage { get; set; }
        public CredentialType Credential { get; set; }
    }
}
