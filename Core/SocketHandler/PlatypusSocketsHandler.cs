using Core.ApplicationAction;
using Core.Sockethandler;
using Core.User;

namespace Core.SocketHandler
{
    public class PlatypusSocketsHandler
    {
        private readonly List<PlatypusServerSocketHandler> _serverSocketsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly UsersHandler _usersHandler;

        public PlatypusSocketsHandler(
            ApplicationActionsHandler applicationActionsHandler,
            UsersHandler usersHandler
        )
        {
            _applicationActionsHandler = applicationActionsHandler;
            _usersHandler = usersHandler;
            _serverSocketsHandler = new List<PlatypusServerSocketHandler>();
            
        }

        public void InitializeSocketHandlers(ServerConfig serverConfig, string host = null)
        {
            _serverSocketsHandler.Add(new PlatypusServerSocketHandler("tcp", serverConfig.TcpSocketPort, _applicationActionsHandler, _usersHandler));

            foreach (PlatypusServerSocketHandler serverSocketHandler in _serverSocketsHandler)
                serverSocketHandler.Initialize(host);
        }
    }
}
