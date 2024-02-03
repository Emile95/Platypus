using PlatypusAPI.User;

namespace Core.User.Abstract
{
    public interface IUserAuthentificator
    {
        UserAccount Authentify(string connectionMethodGuid, Dictionary<string, object> credentials);
    }
}
