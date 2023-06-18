using Core.ApplicationAction;
using Core.Sockethandler;

namespace Core.SocketHandler
{
    public class PlatypusSocketsHandler
    {
        private readonly List<PlatypusServerSocketHandler> _serverSocketsHandler;

        public PlatypusSocketsHandler(
            ApplicationActionsHandler applicationActionsHandler
        )
        {
            _serverSocketsHandler = new List<PlatypusServerSocketHandler>();
            _serverSocketsHandler.Add(new PlatypusServerSocketHandler("tcp", 2000, applicationActionsHandler));
        }

        public void InitializeSocketHandlers(string host = null)
        {
            foreach(PlatypusServerSocketHandler serverSocketHandler in _serverSocketsHandler)
                serverSocketHandler.Initialize(host);
        }
    }
}
