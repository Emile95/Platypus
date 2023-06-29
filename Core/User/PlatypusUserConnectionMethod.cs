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
            UserEntity userEntity = _userRepository.GetUserByData(
                BuiltInUserConnectionMethodGuid.PlatypusUser,
                (userEntity) => {

                    return userEntity.Data["UserName"].Equals(env.Credential.UserName) &&
                           userEntity.Data["Password"].Equals(env.Credential.Password);
                }
            );
            
            if(userEntity == null)
            {
                env.LoginAttemptMessage = "bad credential";
                return false;
            }

            env.UserAccount = new UserAccount()
            {
                ID = userEntity.ID,
            };

            return true;
        }
    }
}
