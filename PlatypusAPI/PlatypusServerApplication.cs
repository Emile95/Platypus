using Common.SocketHandler;

namespace PlatypusAPI
{
    public class PlatypusServerApplication
    {
        private ClientSocketHandler _socketHandler;

        public PlatypusServerApplication(
            ClientSocketHandler socketHandler
        )
        {
            _socketHandler = socketHandler;
        }
    }
}
