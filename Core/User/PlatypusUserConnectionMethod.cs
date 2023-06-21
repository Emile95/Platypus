using Persistance.Entity;
using Persistance.Repository;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.User;

namespace Core.User
{
    public class PlatypusUserConnectionMethod : UserConnectionMethod<PlatypusUserCredential>
    {
        private readonly UserRepository _userRepository;

        public PlatypusUserConnectionMethod(
            UserRepository userRepository
        )
        {
            _userRepository = userRepository;
        }

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
            UserEntity userEntity = _userRepository.GetUserByCredential(env.Credential.UserName, env.Credential.Password);
            
            if(userEntity == null)
            {
                env.LoginAttemptMessage = "bad credential";
                return false;
            }

            env.UserAccount = new UserAccount(userEntity.ID);

            return true;
        }
    }
}
