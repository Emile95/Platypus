using Persistance.Entity;
using PlatypusAPI.User;

namespace PlatypusFramework.Configuration.User
{
    public interface IUserConnectionMethod
    {
        bool Login(List<UserEntity> userstOfConnectionMethod, Dictionary<string,object> credential, ref string loginAttemptMessage, ref UserAccount userAccount);
        string GetName();
        string GetDescription();
    }
}
