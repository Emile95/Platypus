using PlatypusAPI.Exceptions;
using PlatypusAPI.Network;
using PlatypusAPI.User;
using PlatypusNetwork.SocketHandler;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        private readonly ClientSocketHandler<FactorisableExceptionType, RequestType> _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        public PlatypusServerApplication(
            ClientSocketHandler<FactorisableExceptionType, RequestType> socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;
        }

        public void Disconnect()
        {

        }
    }
}
