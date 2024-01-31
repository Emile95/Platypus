using Core.Ressource;
using PlatypusFramework.Configuration.User;
using PlatypusFramework.Core.User;

namespace Core.User
{
    public class PlatypusUserConnectionMethod : UserConnectionMethod<PlatypusUserCredential>
    {
        public override string GetName()
        {
            return Strings.PlatypusUserConnectionMethodName;
        }

        public override string GetDescription()
        {
            return Strings.PlatypusUserConnectionMethodDescription;
        }

        protected override bool LoginImplementation(UserConnectEnvironment<PlatypusUserCredential> env)
        {
            UserInformation foundUser = null;
            foreach(UserInformation user in env.Users)
            {
                if(user.Data["UserName"].Equals(env.Credential.UserName) &&
                   user.Data["Password"].Equals(env.Credential.Password))
                    foundUser = user;
            }
            
            if(foundUser == null)
            {
                env.LoginAttemptMessage = "bad credential";
                return false;
            }

            env.CorrespondingUser = foundUser;

            return true;
        }
    }
}
