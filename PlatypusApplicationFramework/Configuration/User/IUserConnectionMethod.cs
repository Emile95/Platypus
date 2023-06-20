using PlatypusAPI.User;

namespace PlatypusApplicationFramework.Configuration.User
{
    public interface IUserConnectionMethod
    {
        bool Login(Dictionary<string,object> credential, ref string loginAttemptMessage, ref UserAccount userAccount);
        string GetName();
        string GetDescription();
    }
}
