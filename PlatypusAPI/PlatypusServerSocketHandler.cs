using PlatypusNetwork.SocketHandler;
using PlatypusNetwork.SocketHandler.State;
using PlatypusAPI.Sockets;

namespace PlatypusAPI
{
    public class PlatypusClientSocketHandler : ClientSocketHandler
    {
        public Dictionary<SocketDataType, List<Action<byte[]>>> ServerResponseCallBacks;

        public PlatypusClientSocketHandler(string protocol) 
            : base(protocol) 
        {
            ServerResponseCallBacks = new Dictionary<SocketDataType, List<Action<byte[]>>>();

            foreach(SocketDataType socketDataType in Enum.GetValues(typeof(SocketDataType)))
                ServerResponseCallBacks.Add(socketDataType, new List<Action<byte[]>>());
        }

        public override void OnConnect(ServerReceivedState state)
        {
            
        }

        public override void OnLostSocket(ServerReceivedState receivedState)
        {
            
        }

        public override void OnReceive(ServerReceivedState receivedState)
        {
            SocketData serverResponse = PlatypusNetwork.Utils.GetObjectFromBytes<SocketData>(receivedState.BytesRead);

            if (serverResponse == null) return;
            if (ServerResponseCallBacks.ContainsKey(serverResponse.SocketDataType) == false) return;
            if (ServerResponseCallBacks[serverResponse.SocketDataType].Count == 0) return;

            foreach(Action<byte[]> serverResponseCallBack in ServerResponseCallBacks[serverResponse.SocketDataType])
                serverResponseCallBack(serverResponse.Data);
        }
    }
}
