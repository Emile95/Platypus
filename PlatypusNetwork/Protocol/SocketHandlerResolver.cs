using System.Net.Sockets;

namespace PlatypusNetwork.SocketHandler.Protocol
{
    public abstract class SocketHandlerResolver<ReceivedStateType>
        where ReceivedStateType : ReceivedState
    {
        protected readonly int sizeOfInt = sizeof(int);
        protected int _receivedBufferSize;
        protected readonly Action<ReceivedStateType> _onLostSocket;
        protected readonly Action<ReceivedStateType> _onReceive;

        public SocketHandlerResolver(
            int receivedBufferSize,
            Action<ReceivedStateType> onLostSocket,
            Action<ReceivedStateType> onReceive
        )
        {
            _receivedBufferSize = receivedBufferSize;
            _onLostSocket = onLostSocket;
            _onReceive = onReceive;
        }
        public abstract Socket CreateSocket();
        public abstract void Send(Socket socket, byte[] bytes);
        public abstract void ReadCallBack(IAsyncResult ar);
        
    }
}
