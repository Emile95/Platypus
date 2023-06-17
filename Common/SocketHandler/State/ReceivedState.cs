using System.Net.Sockets;

namespace Common.SocketHandler
{
    public abstract class ReceivedState
    {
        public int BufferSize { get; set; }
        public byte[] Buffer { get; set; }
        public Socket WorkSocket { get; set; }
        public byte[] BytesRead { get; set; }

        public abstract ReceivedState CreateCopy();
    }
}
