using PlatypusAPI.User;

namespace Core.Network.RestAPI
{
    internal class UserAccountToken
    {
        public UserAccount UserAccount { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }
}
