using Core.Abstract;
using Core.Persistance.Entity;
using Core.User.Abstract;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.User;
using PlatypusFramework.Confugration;
using PlatypusFramework.Core.User;
using PlatypusRepository;
using System.Reflection;

namespace Core.User
{
    internal class UserAuthentificationHandler :
        IApplicationAttributeMethodResolver<UserConnectionMethodCreatorAttribute>,
        IUserAuthentificator,
        IUserValidator
    {
        private readonly Dictionary<string, IUserConnectionMethod> _connectionMethods;
        private readonly IRepository<UserEntity> _userRepository;

        internal UserAuthentificationHandler(
            IRepository<UserEntity> userRepository
        )
        {
            _connectionMethods = new Dictionary<string, IUserConnectionMethod>();
            _userRepository = userRepository;
        }

        public UserAccount Connect(string connectionMethodGuid, Dictionary<string, object> credentials)
        {
            if (_connectionMethods.ContainsKey(connectionMethodGuid) == false) throw new InvalidUserConnectionMethodGuidException(connectionMethodGuid);
            string loginAttemtMessage = "";
            UserAccount userAccount = null;

            List<UserInformation> users = new List<UserInformation>();
            _userRepository.Consume(
                (userEntity) =>
                {
                    users.Add(new UserInformation()
                    {
                        Guid = userEntity.Guid,
                        Email = userEntity.Email,
                        FullName = userEntity.FullName,
                        Data = userEntity.Data,
                        UserPermissionFlag = userEntity.UserPermissionBits
                    });
                },
                (userEntity) => userEntity.ConnectionMethodGuid == connectionMethodGuid
            );

            bool success = _connectionMethods[connectionMethodGuid].Login(users, credentials, ref loginAttemtMessage, ref userAccount);
            if (success) return userAccount;
            throw new UserConnectionFailedException(loginAttemtMessage);
        }

        public void Resolve(PlatypusApplicationBase application, UserConnectionMethodCreatorAttribute attribute, MethodInfo method)
        {
            IUserConnectionMethod connectionMethod = method.Invoke(application, new object[] { }) as IUserConnectionMethod;
            AddConnectionMethod(connectionMethod, application.ApplicationGuid);
        }

        internal void AddBuiltInConnectionMethod(IUserConnectionMethod connectionMethod, string guid)
        {
            _connectionMethods.Add(guid, connectionMethod);
        }

        private string AddConnectionMethod(IUserConnectionMethod connectionMethod, string applicationGuid)
        {
            string newGuid = connectionMethod.GetName() + applicationGuid;
            _connectionMethods.Add(newGuid, connectionMethod);
            return newGuid;
        }

        internal void RemoveConnectionMethod(string connectionMethodGuid)
        {
            if (_connectionMethods.ContainsKey(connectionMethodGuid))
                _connectionMethods.Remove(connectionMethodGuid);
        }

        public bool Validate(string connectionMethod, Dictionary<string, object> userData)
        {
            if (_connectionMethods.ContainsKey(connectionMethod) == false) throw new InvalidUserConnectionMethodGuidException(connectionMethod);

            IUserConnectionMethod connectionMethods = _connectionMethods[connectionMethod];
            Type[] genericTypes = connectionMethods.GetType().BaseType.GetGenericArguments();
            if (genericTypes.Length == 0) return false;

            ParameterEditorObjectResolver.ValidateDictionnary(genericTypes[0], userData);

            return true;
        }
    }
}
