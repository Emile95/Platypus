using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.Sockets.ClientRequest
{
    public class StartActionClientRequest
    {
        public string StartActionKey { get; set; }
        public ApplicationActionRunParameter Parameters { get; set; }
    }
}
