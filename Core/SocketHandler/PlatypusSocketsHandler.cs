using Core.ApplicationAction;
using Core.Sockethandler;

namespace Core.SocketHandler
{
    public class PlatypusSocketsHandler
    {
        private readonly List<PlatypusServerSocketHandler> _serverSocketsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public PlatypusSocketsHandler(
            ApplicationActionsHandler applicationActionsHandler
        )
        {
            _applicationActionsHandler = applicationActionsHandler;
            _serverSocketsHandler = new List<PlatypusServerSocketHandler>();
            
        }

        public void InitializeSocketHandlers(ServerConfig serverConfig, string host = null)
        {
            _serverSocketsHandler.Add(new PlatypusServerSocketHandler("tcp", serverConfig.TcpSocketPort, _applicationActionsHandler));

            foreach (PlatypusServerSocketHandler serverSocketHandler in _serverSocketsHandler)
                serverSocketHandler.Initialize(host);
        }
    }
}
