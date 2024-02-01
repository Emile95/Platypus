using PlatypusRepository.Configuration;

namespace Core.Persistance.Entity
{
    internal class UserEntity
    {
        [RepositoryEntityID]
        public int ID { get; set; }
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string,object> Data { get; set; }
        public int UserPermissionBits { get; set; }
    }
}
