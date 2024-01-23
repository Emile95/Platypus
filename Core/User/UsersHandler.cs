using Persistance.Entity;
using Persistance.Repository;
using PlatypusAPI.Exceptions;
using PlatypusAPI.ServerFunctionParameter;
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

        public UserAccount AddUser(UserCreationParameter parameter)
        {
            UserEntity userEntity = new UserEntity()
            {
                FullName = parameter.FullName,
                Email = parameter.Email,
                Data = parameter.Data,
                UserPermissionBits = (int)CreateUserPermissionFlasgWithList(parameter.UserPermissionFlags)
            };

            return SaveUser(parameter.ConnectionMethodGuid, userEntity, true);
        }

        public UserAccount UpdateUser(UserUpdateParameter parameter)
        {
            UserEntity userEntity = new UserEntity()
            {
                ID = parameter.ID,
                FullName = parameter.FullName,
                Email = parameter.Email,
                Data = parameter.Data,
                UserPermissionBits = (int)CreateUserPermissionFlasgWithList(parameter.UserPermissionFlags)
            };

            return SaveUser(parameter.ConnectionMethodGuid, userEntity, false);
        }

        public void RemoveUser(RemoveUserParameter parameter)
        {
            _userRepository.RemoveUser(parameter.ConnectionMethodGuid, parameter.ID);
        }


        public UserAccount SaveUser(string connectionMethod, UserEntity userEntity, bool isNew)
        {
            if (_connectionMethods.ContainsKey(connectionMethod) == false) throw new InvalidUserConnectionMethodGuidException(connectionMethod);

            IUserConnectionMethod connectionMethods = _connectionMethods[connectionMethod];
            Type[] genericTypes = connectionMethods.GetType().BaseType.GetGenericArguments();
            if (genericTypes.Length == 0) return null;

            ParameterEditorObjectResolver.ValidateDictionnary(genericTypes[0], userEntity.Data);

            UserAccount userAccount = new UserAccount()
            {
                ID = userEntity.ID,
                FullName = userEntity.FullName,
                Email = userEntity.Email
            };

            if (isNew)
            {
                _userRepository.AddUser(connectionMethod, userEntity);
                return userAccount;
            }

            _userRepository.SaveUser(connectionMethod, userEntity);

            return userAccount;
        }

        public UserAccount Connect(UserConnectionParameter parameter)
        {
            if(_connectionMethods.ContainsKey(parameter.ConnectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(parameter.ConnectionMethodGuid);
            string loginAttemtMessage = "";
            UserAccount userAccount = null;
            List<UserEntity> users = _userRepository.GetUsersByConnectionMethod(parameter.ConnectionMethodGuid);
            bool success = _connectionMethods[parameter.ConnectionMethodGuid].Login(users, parameter.Credential, ref loginAttemtMessage, ref userAccount);
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
