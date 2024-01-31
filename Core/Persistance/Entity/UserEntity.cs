using PlatypusRepository;

namespace Core.Persistance.Entity
{
    public class UserEntity : IEntity<int>
    {
        public int ID { get; set; }
        public string ConnectionMethodGuid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string,object> Data { get; set; }
        public int UserPermissionBits { get; set; }

        public int GetID()
        {
            return ID;
        }
    }
}
