using Common.SocketHandler;
using Common.SocketHandler.State;

namespace PlatypusAPI
{
    public class PlatypusServerApplication
    {
        private BaseSocketHandler<ServerReceivedState> _socketHandler;

        public PlatypusServerApplication(
            BaseSocketHandler<ServerReceivedState> socketHandler
        )
        {
            _socketHandler = socketHandler;
        }
    }
}
