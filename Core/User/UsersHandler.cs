using Persistance.Repository;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.User;
using Utils.GuidGeneratorHelper;

namespace Core.User
{
    public class UsersHandler
    {
        private readonly Dictionary<string, IUserCredentialMethod> _credentialMethod;
        private readonly Dictionary<int, UserDefinition> _users;

        private readonly UserRepository _userRepository;

        public UsersHandler(
            UserRepository userRepository
        )
        {
            _credentialMethod = new Dictionary<string, IUserCredentialMethod>();
            _users = new Dictionary<int, UserDefinition>();

            _userRepository = userRepository;
        }

        public void AddCredentialMethod(IUserCredentialMethod credentialMethod)
        {
            string newGuid = GuidGenerator.GenerateFromEnumerable(_credentialMethod.Keys);
            _credentialMethod.Add(newGuid, credentialMethod);
        }

        public void AddUser(string credentialMethodGuid)
        {
            IUserCredentialMethod credentialMethod = _credentialMethod[credentialMethodGuid];
            UserDefinition userDefinition = new UserDefinition();
            userDefinition.CredentialMethod = credentialMethod;

            int userID = 0;
            if(_users.Count == 0)
            {
                userID = 1;
            } else
            {
                UserAccount lastUserAccount = GetLastUserAccount();
                userID = lastUserAccount.ID+1;
            }

            UserAccount userAccount = new UserAccount(userID);
            userDefinition.UserAccount = userAccount;

            _users.Add(userID, userDefinition);
        }

        private UserAccount GetLastUserAccount()
        {
            return _users[_users.Count - 1].UserAccount;
        }

    }
}
