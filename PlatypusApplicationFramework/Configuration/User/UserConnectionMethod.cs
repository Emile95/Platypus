using PlatypusAPI.User;
using PlatypusApplicationFramework.Confugration;

namespace PlatypusApplicationFramework.Configuration.User
{
    public abstract class UserConnectionMethod<CredentialType> : IUserConnectionMethod
        where CredentialType : class, new()
    {
        public bool Login(Dictionary<string, object> credential, ref string loginAttemptMessage, ref UserAccount userAccount)
        {
            CredentialType resolvedCredential = ParameterEditorObjectResolver.ResolveByDictionnary<CredentialType>(credential);

            UserConnectEnvironment<CredentialType> env = new UserConnectEnvironment<CredentialType>()
            {
                Credential = resolvedCredential
            };

            bool loginSucceeded = LoginImplementation(env);

            loginAttemptMessage = env.LoginAttemptMessage;
            userAccount = env.UserAccount;

            return loginSucceeded;
        }

        public abstract string GetName();

        public virtual string GetDescription() { return ""; }

        protected abstract bool LoginImplementation(UserConnectEnvironment<CredentialType> env);
    }
}
