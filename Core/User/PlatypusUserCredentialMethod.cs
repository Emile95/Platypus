using Persistance.Repository;
using PlatypusApplicationFramework.Configuration.User;

namespace Core.User
{
    public class PlatypusUserCredentialMethod : UserCredentialMethod<PlatypusUserCredential>
    {
        private readonly UserRepository _userRepository;

        public PlatypusUserCredentialMethod(
            UserRepository _userRepository
        )
        {
            _userRepository = _userRepository;
        }

        protected override void LoginImplementation(PlatypusUserCredential credential)
        {
            
        }
    }
}
