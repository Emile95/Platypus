using PlatypusAPI.User;

namespace Core.Abstract
{
    internal interface IServerConnector
    {
        UserAccount Connect(string connectionMethodGuid, Dictionary<string, object> credentials);
    }
}
