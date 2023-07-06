using Persistance.Entity;
using Persistance.Repository;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.User;
using PlatypusApplicationFramework.Confugration;
using System.Reflection;

namespace Core.User
{
    public class UsersHandler
    {
        private readonly Dictionary<string, UsersOfConnectionMethod> _userAccounts;
        private readonly UserRepository _userRepository;

        public UsersHandler(
            UserRepository userRepository
        )
        {
            _userAccounts = new Dictionary<string, UsersOfConnectionMethod>();
            _userRepository = userRepository;
        }

        public void AddConnectionMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
        {
            IUserConnectionMethod credentialMethod = methodInfo.Invoke(application, new object[] { }) as IUserConnectionMethod;
            AddConnectionMethod(credentialMethod, applicationGuid);
        }

        public void AddBuiltInConnectionMethod(IUserConnectionMethod credentialMethod, string guid)
        {
            _userAccounts.Add(guid, new UsersOfConnectionMethod() { UserConnectionMethod = credentialMethod });
        }

        public string AddConnectionMethod(IUserConnectionMethod credentialMethod, string applicationGuid)
        {
            string newGuid = credentialMethod.GetName() + applicationGuid;
            _userAccounts.Add(newGuid, new UsersOfConnectionMethod() { UserConnectionMethod = credentialMethod });
            return newGuid;
        }

        public void RemoveConnectionMethod(string connectionMethodGuid)
        {
            _userAccounts.Remove(connectionMethodGuid);
        }

        public UserAccount AddUser(UserCreationParameter userCreationParameter)
        {
            if (_userAccounts.ContainsKey(userCreationParameter.ConnectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(userCreationParameter.ConnectionMethodGuid);

            UsersOfConnectionMethod usersOfConnectionMethod = _userAccounts[userCreationParameter.ConnectionMethodGuid];
            Type[] genericTypes = usersOfConnectionMethod.UserConnectionMethod.GetType().BaseType.GetGenericArguments();
            if (genericTypes.Length == 0) return null;

            ParameterEditorObjectResolver.ValidateDictionnary(genericTypes[0], userCreationParameter.Data);

            int userID = _userRepository.SaveUser(userCreationParameter.ConnectionMethodGuid, new UserEntity()
            {
                FullName = userCreationParameter.FullName,
                Email = userCreationParameter.Email,
                Data = userCreationParameter.Data,
                UserPermissionBits = (int)userCreationParameter.UserPermissionFlags
            });

            UserAccount userAccount = new UserAccount()
            {
                ID = userID,
                FullName = userCreationParameter.FullName,
                Email = userCreationParameter.Email
            };
            usersOfConnectionMethod.Users.Add(userAccount);

            return userAccount;
        }

        public UserAccount Connect(Dictionary<string, object> credential, string connectionMethodGuid)
        {
            if(_userAccounts.ContainsKey(connectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(connectionMethodGuid);
            string loginAttemtMessage = "";
            UserAccount userAccount = null;
            bool success = _userAccounts[connectionMethodGuid].UserConnectionMethod.Login(credential, ref loginAttemtMessage, ref userAccount);
            if (success) return userAccount;
            throw new UserConnectionFailedException(loginAttemtMessage);
        }
    }
}
