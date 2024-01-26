using Core.Ressource;
using Persistance.Entity;
using PlatypusFramework.Configuration.User;

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
            UserEntity foundUser = null;
            foreach(UserEntity user in env.Users)
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
