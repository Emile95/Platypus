using PlatypusAPI.User;

namespace Core.User
{
    public class UserDefinition
    {
        public UserAccount UserAccount { get; set; }
        public Dictionary<string, object> Credential { get; set; }
    }
}
