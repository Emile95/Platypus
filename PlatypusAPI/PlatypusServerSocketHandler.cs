using Common.SocketData.ServerResponse;
using Common.SocketHandler;
using Common.SocketHandler.State;

namespace PlatypusAPI
{
    public class PlatypusClientSocketHandler : ClientSocketHandler
    {
        public Dictionary<ServerResponseType, List<Action<byte[]>>> ServerResponseCallBacks;

        public PlatypusClientSocketHandler(string protocol) 
            : base(protocol) 
        {
            ServerResponseCallBacks = new Dictionary<ServerResponseType, List<Action<byte[]>>>();
            ServerResponseCallBacks.Add(ServerResponseType.UserConnection, new List<Action<byte[]>>());
            ServerResponseCallBacks.Add(ServerResponseType.ApplicationActionRunResult, new List<Action<byte[]>>());
        }

        public override void OnConnect(ServerReceivedState state)
        {
            
        }

        public override void OnLostSocket(ServerReceivedState receivedState)
        {
            
        }

        public override void OnReceive(ServerReceivedState receivedState)
        {
            ServerResponseData serverResponse = Common.Utils.GetObjectFromBytes<ServerResponseData>(receivedState.BytesRead);

            if (serverResponse == null) return;
            if (ServerResponseCallBacks.ContainsKey(serverResponse.ServerResponseType) == false) return;
            if (ServerResponseCallBacks[serverResponse.ServerResponseType].Count == 0) return;

            foreach(Action<byte[]> serverResponseCallBack in ServerResponseCallBacks[serverResponse.ServerResponseType])
                serverResponseCallBack(serverResponse.Data);
        }
    }
}
