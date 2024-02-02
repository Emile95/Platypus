using PlatypusAPI.User;
using PlatypusFramework.Confugration;
using PlatypusFramework.Core.User;

namespace PlatypusFramework.Configuration.User
{
    public abstract class UserConnectionMethod<CredentialType> : IUserConnectionMethod
        where CredentialType : class, new()
    {
        public bool Login(List<UserInformation> users, Dictionary<string, object> credential, ref string loginAttemptMessage, ref UserAccount userAccount)
        {
            CredentialType resolvedCredential = ParameterEditorObjectResolver.ResolveByDictionnary<CredentialType>(credential);

            UserConnectEnvironment<CredentialType> env = new UserConnectEnvironment<CredentialType>()
            {
                Credential = resolvedCredential,
                Users = users
            };

            bool loginSucceeded = LoginImplementation(env);

            loginAttemptMessage = env.LoginAttemptMessage;

            if(env.CorrespondingUser != null)
                userAccount = new UserAccount()
                {
                    Guid = env.CorrespondingUser.Guid,
                    FullName = env.CorrespondingUser.FullName,
                    Email = env.CorrespondingUser.Email,
                    UserPermissionFlags = env.CorrespondingUser.UserPermissionFlag
                };

            return loginSucceeded;
        }

        public abstract string GetName();

        public virtual string GetDescription() { return ""; }

        protected abstract bool LoginImplementation(UserConnectEnvironment<CredentialType> env);
    }
}
