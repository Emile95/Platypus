namespace Persistance.Entity
{
    public class UserEntity
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string,object> Data { get; set; }
        public int UserPermissionBits { get; set; }
    }
}
