using PlatypusAPI.User;
using PlatypusApplicationFramework.Configuration.User;

namespace Core.User
{
    public class UsersOfConnectionMethod
    {
        public IUserConnectionMethod UserConnectionMethod { get; set; }
        public List<UserAccount> Users = new List<UserAccount>();
    }
}
