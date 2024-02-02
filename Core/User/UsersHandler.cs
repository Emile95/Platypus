using Core.Persistance.Entity;
using Core.Persistance.Repository;
using Core.User.Abstract;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;

namespace Core.User
{
    internal class UsersHandler
    {
        private readonly UserRepository _userRepository;
        private readonly IUserValidator _userValidator;

        internal UsersHandler(
            UserRepository userRepository,
            IUserValidator userValidator
        )
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
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
            _userValidator.Validate(connectionMethod, userEntity.Data);

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

        private UserPermissionFlag CreateUserPermissionFlasgWithList(List<UserPermissionFlag> userPermissionFlags)
        {
            UserPermissionFlag flags = 0;
            foreach (UserPermissionFlag userPermissionFlag in userPermissionFlags)
                flags |= userPermissionFlag;
            return flags;
        }

        
    }
}
