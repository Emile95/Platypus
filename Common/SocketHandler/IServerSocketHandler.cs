using System.Net.Sockets;

namespace Common.SocketHandler
{
    public interface IServerSocketHandler<ClientSocketKeyType> : IServerSocketEventHandler<ClientSocketKeyType>
        where ClientSocketKeyType : class
    {
        Dictionary<ClientSocketKeyType, Socket> ClientSockets { get; set; }
        void SendToClient(ClientSocketKeyType clientSocketKey, byte[] bytes);
        void SendToAllClients(byte[] bytes);
    }
}
