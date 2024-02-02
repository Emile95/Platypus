using Core.Persistance.Entity;
using Core.User.Abstract;
using PlatypusRepository;

namespace Core.User
{
    internal class UsersHandler : IRepository<UserEntity>
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

        public UserEntity Add(UserEntity entity)
        {
            _userValidator.Validate(entity.ConnectionMethodGuid, entity.Data);
            return _userRepository.Add(entity);
        }

        public UserEntity Update(UserEntity entity)
        {
            _userValidator.Validate(entity.ConnectionMethodGuid, entity.Data);
            return _userRepository.Update(entity);
        }

        public void Remove(UserEntity entity)
        {
            _userRepository.Remove(entity);
        }

        public void Consume(Action<UserEntity> consumer, Predicate<UserEntity> condition = null)
        {
            _userRepository.Consume(consumer, condition);
        }
    }
}
