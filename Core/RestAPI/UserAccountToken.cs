using PlatypusAPI.User;

namespace Core.RestAPI
{
    internal class UserAccountToken
    {
        public UserAccount UserAccount { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }
}
