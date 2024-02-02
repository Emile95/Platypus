using PlatypusAPI.User;

namespace Core.User.Abstract
{
    internal interface IUserAuthentificator
    {
        UserAccount Connect(string connectionMethodGuid, Dictionary<string, object> credentials);
    }
}
