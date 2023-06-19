using Persistance.Repository;
using PlatypusApplicationFramework.Configuration.User;

namespace Core.User
{
    public class PlatypusUserConnectionMethod : UserConnectionMethod<PlatypusUserCredential>
    {
        private readonly UserRepository _userRepository;

        public PlatypusUserConnectionMethod(
            UserRepository _userRepository
        )
        {
            _userRepository = _userRepository;
        }

        public override string GetName()
        {
            return Strings.PlatypusUserCredentialMethodName;
        }

        public override string GetDescription()
        {
            return Strings.PlatypusUserCredentialMethodDescription;
        }

        protected override void LoginImplementation(PlatypusUserCredential credential)
        {
            
        }
    }
}
