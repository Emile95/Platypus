using Core.Abstract;
using Core.Persistance.Entity;
using Core.Persistance.Repository;
using PlatypusAPI.Exceptions;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.User;
using PlatypusFramework.Confugration;
using PlatypusFramework.Core.User;
using System.Reflection;

namespace Core.User
{
    internal class UsersHandler :
        IApplicationAttributeMethodResolver<UserConnectionMethodCreatorAttribute>
    {
        private readonly Dictionary<string, IUserConnectionMethod> _connectionMethods;
        private readonly UserRepository _userRepository;

        internal UsersHandler(
            UserRepository userRepository
        )
        {
            _connectionMethods = new Dictionary<string, IUserConnectionMethod>();
            _userRepository = userRepository;
        }

        public void Resolve(PlatypusApplicationBase application, UserConnectionMethodCreatorAttribute attribute, MethodInfo method)
        {
            IUserConnectionMethod credentialMethod = method.Invoke(application, new object[] { }) as IUserConnectionMethod;
            AddConnectionMethod(credentialMethod, application.ApplicationGuid);
        }

        internal void AddBuiltInConnectionMethod(IUserConnectionMethod credentialMethod, string guid)
        {
            _connectionMethods.Add(guid, credentialMethod);
        }

        internal string AddConnectionMethod(IUserConnectionMethod connectionMethod, string applicationGuid)
        {
            string newGuid = connectionMethod.GetName() + applicationGuid;
            _connectionMethods.Add(newGuid, connectionMethod);
            return newGuid;
        }

        internal void RemoveConnectionMethod(string connectionMethodGuid)
        {
            if( _connectionMethods.ContainsKey(connectionMethodGuid))
                _connectionMethods.Remove(connectionMethodGuid);
        }

        internal UserAccount AddUser(UserCreationParameter parameter)
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

        internal UserAccount UpdateUser(UserUpdateParameter parameter)
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

        internal void RemoveUser(RemoveUserParameter parameter)
        {
            _userRepository.RemoveUser(parameter.ConnectionMethodGuid, parameter.ID);
        }


        internal UserAccount SaveUser(string connectionMethod, UserEntity userEntity, bool isNew)
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
            List<UserInformation> users = _userRepository.GetUsersByConnectionMethod(parameter.ConnectionMethodGuid).Select((o) => 
                new UserInformation()
                {
                    ID = o.ID,
                    Email = o.Email,
                    FullName = o.FullName,
                    Data = o.Data,
                    UserPermissionFlag = (UserPermissionFlag)o.UserPermissionBits
                }
            ).ToList();
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
