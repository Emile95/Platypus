using PlatypusAPI.User;
using PlatypusFramework.Core.User;

namespace PlatypusFramework.Configuration.User
{
    public interface IUserConnectionMethod
    {
        bool Login(List<UserInformation> userstOfConnectionMethod, Dictionary<string,object> credential, ref string loginAttemptMessage, ref UserAccount userAccount);
        string GetName();
        string GetDescription();
    }
}
