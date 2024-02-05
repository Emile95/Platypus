using Core.Persistance.Entity;
using Core.User.Abstract;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusRepository;

namespace Core.User
{
    public class UsersHandler : 
        IRepositoryAddOperator<UserCreationParameter>,
        IRepositoryUpdateOperator<UserUpdateParameter>,
        IRepositoryRemoveOperator<RemoveUserParameter, string>
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IUserValidator _userValidator;

        public UsersHandler(
            IRepository<UserEntity, string> userRepository,
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

        public void Remove(string id)
        {
            _userRepository.Remove(id);
        }
    }
}
