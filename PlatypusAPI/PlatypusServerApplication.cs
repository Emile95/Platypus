using Common.Exceptions;
using Common.Sockets;
using Common.Sockets.ClientRequest;
using Common.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.Exceptions;
using PlatypusAPI.Sockets.ClientRequest;
using PlatypusAPI.Sockets.ServerResponse;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace PlatypusAPI
{
    public class PlatypusServerApplication
    {
        private readonly PlatypusClientSocketHandler _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        private readonly Dictionary<string, StartActionServerResponseWaiter> _startActionServerResponseWaiters;
        private readonly Dictionary<string, AddUserServerResponseWaiter> _addUserServerResponseWaiters;
        private readonly Dictionary<string, GetRunningApplicationActionsWaiter> _getRunningApplicationActionsServerResponseWaiters;

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;

            _startActionServerResponseWaiters = new Dictionary<string, StartActionServerResponseWaiter>();
            _addUserServerResponseWaiters = new Dictionary<string, AddUserServerResponseWaiter>();
            _getRunningApplicationActionsServerResponseWaiters = new Dictionary<string, GetRunningApplicationActionsWaiter>();

            _socketHandler.ServerResponseCallBacks[SocketDataType.StartApplicationAction].Add(StartApplicationServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.AddUser].Add(AddUserServerResponseCallBack);
            _socketHandler.ServerResponseCallBacks[SocketDataType.GetRunningActions].Add(GetRunningApplicationActionsServerResponseCallBack);
        }

        public void Disconnect()
        {

        }

        public ApplicationActionRunResult RunApplicationAction(ApplicationActionRunParameter applicationActionRunparameter)
        {
            ApplicationActionRunResult result = null;
            RunClientRequest<StartActionServerResponse, StartActionServerResponseWaiter, StartActionClientRequest>(
                 _startActionServerResponseWaiters, SocketDataType.StartApplicationAction,
                 (clientRequest) => {
                     clientRequest.Parameters = applicationActionRunparameter;
                 },
                 (serverResponseWaiter) => {
                     result = serverResponseWaiter.Result;
                 }
            );
            return result;
        }

        public UserAccount AddUser(string credentialMethodGUID, string fullName, string email, Dictionary<string, object> data)
        {
            UserAccount userAccount = null;
            RunClientRequest<AddUserServerResponse, AddUserServerResponseWaiter, AddUserClientRequest>(
                 _addUserServerResponseWaiters, SocketDataType.AddUser,
                 (clientRequest) => {
                     clientRequest.CredentialMethodGUID = credentialMethodGUID;
                     clientRequest.FullName = fullName;
                     clientRequest.Email = email;
                     clientRequest.Data = data;
                 },
                 (serverResponseWaiter) => {
                     userAccount = serverResponseWaiter.UserAccount;
                 }
            );
            return userAccount;
        }

        public List<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            List<ApplicationActionRunInfo> result = new List<ApplicationActionRunInfo>();
            RunClientRequest<GetRunningApplicationActionsServerResponse, GetRunningApplicationActionsWaiter, ClientRequestBase>(
                 _getRunningApplicationActionsServerResponseWaiters, SocketDataType.GetRunningActions, null,
                 (serverResponseWaiter) => {
                     result = serverResponseWaiter.ApplicationActionRunInfos;
                 }
            );
            return result;
        }

        private void StartApplicationServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<StartActionServerResponse, StartActionServerResponseWaiter>(
                _startActionServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.Result = serverResponse.Result;
                }
            );
        }

        private void AddUserServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<AddUserServerResponse, AddUserServerResponseWaiter>(
                _addUserServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.UserAccount = serverResponse.UserAccount;
                }
            );
        }

        private void GetRunningApplicationActionsServerResponseCallBack(byte[] bytes)
        {
            ServerResponseCallBack<GetRunningApplicationActionsServerResponse, GetRunningApplicationActionsWaiter>(
                _getRunningApplicationActionsServerResponseWaiters, bytes,
                (serverResponseWaiter, serverResponse) => {
                    serverResponseWaiter.ApplicationActionRunInfos = serverResponse.ApplicationActionRunInfos;
                }
            );
        }

        private void RunClientRequest<ServerResponseType, ServerResponseWaiterType, ClientRequestType>(Dictionary<string, ServerResponseWaiterType> serverResponseWaiters, SocketDataType socketDataType, Action<ClientRequestType> consumer, Action<ServerResponseWaiterType> callBack = null)
            where ServerResponseType : ServerResponseBase
            where ServerResponseWaiterType : ServerResponseWaiter, new()
            where ClientRequestType : ClientRequestBase, new()
        {
            string guid = GuidGenerator.GenerateFromEnumerable(serverResponseWaiters.Keys);

            ServerResponseWaiterType serverResponseWaiter = new ServerResponseWaiterType();

            serverResponseWaiters.Add(guid, serverResponseWaiter);

            ClientRequestType clientRequest = new ClientRequestType()
            {
                RequestKey = guid
            };

            if(consumer != null)
                consumer(clientRequest);

            SocketData clientRequestData = new SocketData()
            {
                SocketDataType = socketDataType,
                Data = Common.Utils.GetBytesFromObject(clientRequest)
            };

            _socketHandler.SendToServer(Common.Utils.GetBytesFromObject(clientRequestData));

            while (serverResponseWaiter.Received == false)
                Thread.Sleep(1000);

            if(callBack != null)
                callBack(serverResponseWaiter);

            serverResponseWaiters.Remove(guid);
        }

        private void ServerResponseCallBack<ServerResponseType, ServerResponseWaiterType>(Dictionary<string, ServerResponseWaiterType> serverResponseWaiters, byte[] bytes, Action<ServerResponseWaiterType, ServerResponseType> consumer = null)
            where ServerResponseType : ServerResponseBase
            where ServerResponseWaiterType : ServerResponseWaiter
        {
            ServerResponseType serverResponse = Common.Utils.GetObjectFromBytes<ServerResponseType>(bytes);
            if (serverResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;
            
            ServerResponseWaiterType serverResponseWaiter = serverResponseWaiters[serverResponse.RequestKey];
            serverResponseWaiter.Exception = ExceptionFactory.CreateException(serverResponse.FactorisableExceptionType, serverResponse.FactorisableExceptionParameters);
            serverResponseWaiter.Received = true;

            if(consumer != null)
                consumer(serverResponseWaiter, serverResponse);
        }

        private abstract class ServerResponseWaiter
        {
            public bool Received { get; set; }
            public FactorisableException Exception { get; set; }
        }

        private class StartActionServerResponseWaiter : ServerResponseWaiter
        {
            public ApplicationActionRunResult Result { get; set; }
        }

        private class AddUserServerResponseWaiter : ServerResponseWaiter
        {
            public UserAccount UserAccount { get; set; }
        }

        private class GetRunningApplicationActionsWaiter : ServerResponseWaiter
        {
            public List<ApplicationActionRunInfo> ApplicationActionRunInfos { get; set; }
        }
    }
}
