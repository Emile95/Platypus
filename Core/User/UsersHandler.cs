using Persistance.Entity;
using Persistance.Repository;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.User;
using System.Reflection;

namespace Core.User
{
    public class UsersHandler
    {
        private readonly Dictionary<string, IUserConnectionMethod> _credentialMethods;

        private readonly UserRepository _userRepository;

        public UsersHandler(
            UserRepository userRepository
        )
        {
            _credentialMethods = new Dictionary<string, IUserConnectionMethod>();

            _userRepository = userRepository;
        }

        public void AddCredentialMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
        {
            IUserConnectionMethod credentialMethod = methodInfo.Invoke(application, new object[] { }) as IUserConnectionMethod;
            AddCredentialMethod(credentialMethod, applicationGuid);
        }

        public void AddBuiltInCredentialMethod(IUserConnectionMethod credentialMethod, string guid)
        {
            _credentialMethods.Add(guid, credentialMethod);
        }

        public string AddCredentialMethod(IUserConnectionMethod credentialMethod, string applicationGuid)
        {
            string newGuid = credentialMethod.GetName() + applicationGuid;
            _credentialMethods.Add(newGuid, credentialMethod);
            return newGuid;
        }

        public void RemoveCredentialMethod(string credentialMethodGuid)
        {
            _credentialMethods.Remove(credentialMethodGuid);
        }

        public void AddPlatypusUser(string userName, string password)
        {
            _userRepository.SaveUser(new UserEntity()
            {
                UserName = userName,
                Password = password
            });
        }

        public UserAccount Connect(Dictionary<string, object> credential, string connectionMethodGuid)
        {
            if(_credentialMethods.ContainsKey(connectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(connectionMethodGuid);
            string loginAttemtMessage = "";
            UserAccount userAccount = null;
            bool success = _credentialMethods[connectionMethodGuid].Login(credential, ref loginAttemtMessage, ref userAccount);
            if (success) return userAccount;
            throw new UserConnectionFailedException(loginAttemtMessage);
        }

    }
}
