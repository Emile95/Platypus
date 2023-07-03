using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusAPI.SocketData.ClientRequest
{
    public class StartActionClientRequest
    {
        public string StartActionKey { get; set; }
        public ApplicationActionRunParameter Parameters { get; set; }
    }
}
