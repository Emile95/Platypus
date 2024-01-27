using System.Net.Sockets;

namespace Core.Network.SocketHandler
{
    public class PlatypusSocketsHandler
    {
        private readonly ServerInstance _serverInstance;
        private readonly List<PlatypusServerSocketHandler> _serverSocketsHandler;

        public PlatypusSocketsHandler(
            ServerInstance serverInstance
        )
        {
            _serverInstance = serverInstance;
            _serverSocketsHandler = new List<PlatypusServerSocketHandler>();
        }

        public void Initialize(ServerConfig serverConfig, string host = null)
        {
            _serverSocketsHandler.Add(new PlatypusServerSocketHandler(_serverInstance, ProtocolType.Tcp, serverConfig.TcpSocketPort));

            foreach (PlatypusServerSocketHandler serverSocketHandler in _serverSocketsHandler)
                serverSocketHandler.Initialize(host);
        }
    }
}
