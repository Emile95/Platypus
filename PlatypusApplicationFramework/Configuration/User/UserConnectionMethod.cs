using Persistance.Entity;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Confugration;

namespace PlatypusApplicationFramework.Configuration.User
{
    public abstract class UserConnectionMethod<CredentialType> : IUserConnectionMethod
        where CredentialType : class, new()
    {
        public bool Login(List<UserEntity> usersOfConnectionMethod, Dictionary<string, object> credential, ref string loginAttemptMessage, ref UserAccount userAccount)
        {
            CredentialType resolvedCredential = ParameterEditorObjectResolver.ResolveByDictionnary<CredentialType>(credential);

            UserConnectEnvironment<CredentialType> env = new UserConnectEnvironment<CredentialType>()
            {
                Credential = resolvedCredential,
                UsersOfConnectionMethod = usersOfConnectionMethod
            };

            bool loginSucceeded = LoginImplementation(env);

            loginAttemptMessage = env.LoginAttemptMessage;

            if(env.CorrespondingUser != null)
                userAccount = new UserAccount()
                {
                    ID = env.CorrespondingUser.ID,
                    FullName = env.CorrespondingUser.FullName,
                    Email = env.CorrespondingUser.Email,
                    UserPermissionFlags = (UserPermissionFlag)env.CorrespondingUser.UserPermissionBits
                };

            return loginSucceeded;
        }

        public abstract string GetName();

        public virtual string GetDescription() { return ""; }

        protected abstract bool LoginImplementation(UserConnectEnvironment<CredentialType> env);
    }
}
