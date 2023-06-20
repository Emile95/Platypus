using Persistance.Repository;
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
            return Strings.PlatypusUserCredentialMethodName;
        }

        public override string GetDescription()
        {
            return Strings.PlatypusUserCredentialMethodDescription;
        }

        protected override bool LoginImplementation(UserConnectEnvironment<PlatypusUserCredential> env)
        {
            return true;
        }
    }
}
