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
        private readonly Dictionary<string, IUserConnectionMethod> _connectionMethods;
        private readonly UserRepository _userRepository;

        public UsersHandler(
            UserRepository userRepository
        )
        {
            _connectionMethods = new Dictionary<string, IUserConnectionMethod>();
            _userRepository = userRepository;
        }

        public void AddConnectionMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
        {
            IUserConnectionMethod credentialMethod = methodInfo.Invoke(application, new object[] { }) as IUserConnectionMethod;
            AddConnectionMethod(credentialMethod, applicationGuid);
        }

        public void AddBuiltInConnectionMethod(IUserConnectionMethod credentialMethod, string guid)
        {
            _connectionMethods.Add(guid, credentialMethod);
        }

        public string AddConnectionMethod(IUserConnectionMethod connectionMethod, string applicationGuid)
        {
            string newGuid = connectionMethod.GetName() + applicationGuid;
            _connectionMethods.Add(newGuid, connectionMethod);
            return newGuid;
        }

        public void RemoveConnectionMethod(string connectionMethodGuid)
        {
            if( _connectionMethods.ContainsKey(connectionMethodGuid))
                _connectionMethods.Remove(connectionMethodGuid);
        }

        public UserAccount AddUser(UserCreationParameter userCreationParameter)
        {
            if (_connectionMethods.ContainsKey(userCreationParameter.ConnectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(userCreationParameter.ConnectionMethodGuid);

            IUserConnectionMethod connectionMethods = _connectionMethods[userCreationParameter.ConnectionMethodGuid];
            Type[] genericTypes = connectionMethods.GetType().BaseType.GetGenericArguments();
            if (genericTypes.Length == 0) return null;

            ParameterEditorObjectResolver.ValidateDictionnary(genericTypes[0], userCreationParameter.Data);

            int userID = _userRepository.SaveUser(userCreationParameter.ConnectionMethodGuid, new UserEntity()
            {
                FullName = userCreationParameter.FullName,
                Email = userCreationParameter.Email,
                Data = userCreationParameter.Data,
                UserPermissionBits = (int)CreateUserPermissionFlasgWithList(userCreationParameter.UserPermissionFlags)
            });

            UserAccount userAccount = new UserAccount()
            {
                ID = userID,
                FullName = userCreationParameter.FullName,
                Email = userCreationParameter.Email
            };

            return userAccount;
        }

        public UserAccount Connect(Dictionary<string, object> credential, string connectionMethodGuid)
        {
            if(_connectionMethods.ContainsKey(connectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(connectionMethodGuid);
            string loginAttemtMessage = "";
            UserAccount userAccount = null;
            List<UserEntity> users = _userRepository.GetUsersByConnectionMethod(connectionMethodGuid);
            bool success = _connectionMethods[connectionMethodGuid].Login(users, credential, ref loginAttemtMessage, ref userAccount);
            if (success) return userAccount;
            throw new UserConnectionFailedException(loginAttemtMessage);
        }

        private UserPermissionFlag CreateUserPermissionFlasgWithList(List<UserPermissionFlag> userPermissionFlags)
        {
            UserPermissionFlag flags = 0;
            foreach (UserPermissionFlag userPermissionFlag in userPermissionFlags)
                flags |= userPermissionFlag;
            return flags;
        }
    }
}
