using PlatypusAPI.User;
using PlatypusRepository.Configuration;

namespace Core.Persistance.Entity
{
    internal class UserEntity
    {
        [RepositoryEntityID]
        public string Guid { get; set; }
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string,object> Data { get; set; }
        public UserPermissionFlag UserPermissionBits { get; set; }
    }
}
