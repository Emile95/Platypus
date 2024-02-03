using Core.Persistance.Entity;
using Core.User.Abstract;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusRepository;

namespace Core.User
{
    internal class UsersHandler : 
        IRepositoryAddOperator<UserCreationParameter>,
        IRepositoryUpdateOperator<UserUpdateParameter>,
        IRepositoryRemoveOperator<RemoveUserParameter>,
        IRepositoryConsumeOperator<UserEntity>
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IUserValidator _userValidator;

        internal UsersHandler(
            IRepository<UserEntity> userRepository,
            IUserValidator userValidator
        )
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
        }

        public UserCreationParameter Add(UserCreationParameter parameter)
        {
            _userValidator.Validate(parameter.ConnectionMethodGuid, parameter.Data);
            _userRepository.Add(new UserEntity()
            {
                ConnectionMethodGuid = parameter.ConnectionMethodGuid,
                Data = parameter.Data,
                FullName = parameter.FullName,
                UserPermissionBits = parameter.UserPermissionFlags,
                Email = parameter.Email
            });
            return parameter;
        }

        public UserUpdateParameter Update(UserUpdateParameter parameter)
        {
            _userValidator.Validate(parameter.ConnectionMethodGuid, parameter.Data);
            _userRepository.Update(new UserEntity()
            {
                Guid = parameter.Guid,
                ConnectionMethodGuid = parameter.ConnectionMethodGuid,
                Data = parameter.Data,
                FullName = parameter.FullName,
                UserPermissionBits = parameter.UserPermissionFlags,
                Email = parameter.Email
            });
            return parameter;
        }

        public void Remove(RemoveUserParameter parameter)
        {
            _userRepository.Remove(new UserEntity()
            {
                Guid = parameter.Guid
            });
        }

        public void Consume(Action<UserEntity> consumer, Predicate<UserEntity> condition = null)
        {
            _userRepository.Consume(consumer, condition);
        }
    }
}
