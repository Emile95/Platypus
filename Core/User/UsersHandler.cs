using Persistance.Entity;
using Persistance.Repository;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.User;
using System.Reflection;

namespace Core.User
{
    public class UsersHandler
    {
        private readonly Dictionary<string, IUserConnectionMethod> _credentialMethods;
        private readonly Dictionary<string, List<UserDefinition>> _users;

        private readonly UserRepository _userRepository;

        public UsersHandler(
            UserRepository userRepository
        )
        {
            _credentialMethods = new Dictionary<string, IUserConnectionMethod>();
            _users = new Dictionary<string, List<UserDefinition>>();

            _userRepository = userRepository;
        }

        public void AddCredentialMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
        {
            IUserConnectionMethod credentialMethod = methodInfo.Invoke(application, new object[] { }) as IUserConnectionMethod;
            AddCredentialMethod(credentialMethod, applicationGuid);
        }

        public void AddDefaultCredentialMethod(IUserConnectionMethod credentialMethod, string guid)
        {
            _credentialMethods.Add(guid, credentialMethod);
            _users.Add(guid, new List<UserDefinition>());
        }

        public string AddCredentialMethod(IUserConnectionMethod credentialMethod, string applicationGuid)
        {
            string newGuid = credentialMethod.GetName() + applicationGuid;
            _credentialMethods.Add(newGuid, credentialMethod);
            _users.Add(newGuid, new List<UserDefinition>());
            return newGuid;
        }

        public void RemoveCredentialMethod(string credentialMethodGuid)
        {
            _credentialMethods.Remove(credentialMethodGuid);
            _users.Remove(credentialMethodGuid);
        }

        public void AddUser(string credentialMethodGuid, Dictionary<string,object> credential, string userName)
        {
            UserDefinition userDefinition = new UserDefinition();
            userDefinition.Credential = credential;

            int userID = 0;
            /*if(_users.Count == 0)
            {
                userID = 1;
            } else
            {
                UserAccount lastUserAccount = GetLastUserAccount();
                userID = lastUserAccount.ID+1;
            }*/

            UserAccount userAccount = new UserAccount(userID, userName);
            userDefinition.UserAccount = userAccount;

            _users[credentialMethodGuid].Add(userDefinition);

            string userDirectoryPath = _userRepository.SaveUser(new UserEntity()
            {
                ID = userID,
                Name = credentialMethodGuid,
                CredentialMethodGUID = credentialMethodGuid,
            });

            _userRepository.SaveUserCredentialByBasePath(userDirectoryPath, credential);
        }

        public void LoadUsers()
        {

        }

        public void LoadUser()
        {

        }

        private UserAccount GetLastUserAccount()
        {
            return null;
        }

    }
}
