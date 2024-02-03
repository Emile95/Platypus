using PlatypusAPI.User;

namespace Core.User.Abstract
{
    internal interface IUserAuthentificator
    {
        UserAccount Authentify(string connectionMethodGuid, Dictionary<string, object> credentials);
    }
}
