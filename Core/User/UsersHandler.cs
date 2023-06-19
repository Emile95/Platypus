using PlatypusApplicationFramework.Configuration.User;

namespace Core.User
{
    public class UsersHandler
    {
        private readonly Dictionary<string, IUserCredentialMethod> _credentialMethod;
        private readonly Dictionary<int, UserDefinition> _users;

        public UsersHandler()
        {
            _credentialMethod = new Dictionary<string, IUserCredentialMethod>();
            _users = new Dictionary<int, UserDefinition>();
        }

        
    }
}
