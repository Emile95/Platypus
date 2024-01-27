using PlatypusNetwork.SocketHandler;
using PlatypusNetwork.SocketHandler.State;
using PlatypusAPI.Exceptions;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;
using PlatypusUtils;
using PlatypusAPI.Network;
using PlatypusAPI.Network.ServerResponse;
using PlatypusAPI.Network.ClientRequest;

namespace Core.Network.SocketHandler
{
    internal class PlatypusServerSocketHandler : ServerSocketHandler<string>
    {
        private readonly ServerInstance _serverInstance;
        private readonly int _port;
        private readonly Dictionary<string, UserAccount> _connectedUserOnSockets;

        public PlatypusServerSocketHandler(
            ServerInstance serverInstance,
            string protocol,
            int port
        ) : base(protocol)
        {
            _serverInstance = serverInstance;
            _port = port;
            _connectedUserOnSockets = new Dictionary<string, UserAccount>();
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return Utils.GenerateGuidFromEnumerable(currentKeys);
        }

        public override void OnAccept(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"new client connected, key='{receivedState.ClientKey}'");
            _connectedUserOnSockets.Add(receivedState.ClientKey, null);
        }

        public override void OnLostSocket(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"client lost, key='{receivedState.ClientKey}'");
        }

        public override void OnReceive(ClientReceivedState<string> receivedState)
        {
            SocketData clientRequest = Utils.GetObjectFromBytes<SocketData>(receivedState.BytesRead);

            if (clientRequest == null) return;

            switch (clientRequest.SocketDataType)
            {
                case SocketDataType.UserConnection: ReceiveUserConnectionClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.RunApplicationAction: ReceiveStartApplicationActionClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.AddUser: ReceiveAddUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.UpdateUser: ReceiveUpdateUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.RemoveUser: ReceiveRemoveUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.GetRunningActions: ReceiveGetRunningApplicationActionsClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.GetActionInfos: ReceiveGetApplicationActionInfosClientRequest(receivedState.ClientKey, clientRequest); break;
                case SocketDataType.CancelRunningAction: ReceiveCancelRunningApplicationActionRunClientRequest(receivedState.ClientKey, clientRequest); break;
            }
        }

        public void Initialize(string host)
        {
            Initialize(_port, host);
        }

        private void ReceiveUserConnectionClientRequest(string clientKey, SocketData clientRequestData)
        {
            bool exceptionThrowed = HandleClientRequest<UserConnectionParameter, UserConnectionServerResponse>(
                clientKey, clientRequestData, SocketDataType.UserConnection,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.UserAccount = _serverInstance.UserConnect(clientRequest);
                    _connectedUserOnSockets[clientKey] = serverResponse.UserAccount;
                }
            );
            if (exceptionThrowed)
                _clientSockets.Remove(clientKey);
        }

        private void ReceiveStartApplicationActionClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<StartActionClientRequest, StartActionServerResponse>(
                clientKey, clientRequestData, SocketDataType.RunApplicationAction,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.Result = _serverInstance.RunAction(clientRequest.UserAccount, clientRequest.Parameters);
                }
            );
        }

        private void ReceiveAddUserClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<AddUserClientRequest, AddUserServerResponse>(
                clientKey, clientRequestData, SocketDataType.AddUser,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.UserAccount = _serverInstance.AddUser(
                        clientRequest.UserAccount,
                        new UserCreationParameter()
                        {
                            ConnectionMethodGuid = clientRequest.ConnectionMethodGuid,
                            Email = clientRequest.Email,
                            Data = clientRequest.Data,
                            FullName = clientRequest.FullName,
                            UserPermissionFlags = clientRequest.UserPermissionFlags
                        }
                    );
                }
            );
        }

        private void ReceiveUpdateUserClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<UpdateUserClientRequest, UpdateUserServerResponse>(
                clientKey, clientRequestData, SocketDataType.UpdateUser,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.UserAccount = _serverInstance.UpdateUser(
                        clientRequest.UserAccount,
                        new UserUpdateParameter()
                        {
                            ID = clientRequest.UserID,
                            ConnectionMethodGuid = clientRequest.ConnectionMethodGuid,
                            Email = clientRequest.Email,
                            Data = clientRequest.Data,
                            FullName = clientRequest.FullName,
                            UserPermissionFlags = clientRequest.UserPermissionFlags
                        }
                    );
                }
            );
        }

        private void ReceiveRemoveUserClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<RemoveUserClientRequest, RemoveUserServerResponse>(
                clientKey, clientRequestData, SocketDataType.RemoveUser,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    _serverInstance.RemoveUser(
                        clientRequest.UserAccount,
                        new RemoveUserParameter()
                        {
                            ID = clientRequest.ID,
                            ConnectionMethodGuid = clientRequest.ConnectionMethodGuid,
                        }
                    );
                }
            );
        }

        private void ReceiveGetRunningApplicationActionsClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<ClientRequestBase, GetRunningApplicationActionsServerResponse>(
                clientKey, clientRequestData, SocketDataType.GetRunningActions,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    IEnumerable<ApplicationActionRunInfo> result = _serverInstance.GetRunningApplicationActions(clientRequest.UserAccount);
                    if (result.Count() != 0)
                        serverResponse.ApplicationActionRunInfos = result.ToList();
                    else
                        serverResponse.ApplicationActionRunInfos = new List<ApplicationActionRunInfo>();
                }
            );
        }

        private void ReceiveGetApplicationActionInfosClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<ClientRequestBase, GetApplicationActionInfosServerResponse>(
                clientKey, clientRequestData, SocketDataType.GetActionInfos,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    IEnumerable<ApplicationActionInfo> result = _serverInstance.GetApplicationActionInfos(clientRequest.UserAccount);
                    if (result.Count() != 0)
                        serverResponse.ApplicationActionInfos = result.ToList();
                    else
                        serverResponse.ApplicationActionInfos = new List<ApplicationActionInfo>();
                }
            );
        }

        private void ReceiveCancelRunningApplicationActionRunClientRequest(string clientKey, SocketData clientRequestData)
        {
            HandleClientRequest<CancelRunningApplicationRunClientRequest, AddUserServerResponse>(
                clientKey, clientRequestData, SocketDataType.CancelRunningAction,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    _serverInstance.CancelRunningApplicationAction(
                        clientRequest.UserAccount,
                        new CancelRunningActionParameter()
                        {
                            Guid = clientRequest.ApplicationRunGuid
                        }
                    );
                }
            );
        }

        private bool HandleClientRequest<RequestType, ResponseType>(string clientKey, SocketData clientRequestData, SocketDataType serverResponseType, Action<UserAccount, RequestType, ResponseType> action)
            where ResponseType : ServerResponseBase, new()
            where RequestType : class, new()
        {
            UserAccount userMakingRequest = _connectedUserOnSockets[clientKey];

            bool exceptionThrowed = false;
            SocketData serverResponseData = new SocketData()
            {
                SocketDataType = serverResponseType
            };
            ResponseType serverResponse = new ResponseType();
            RequestType clientRequest = Utils.GetObjectFromBytes<RequestType>(clientRequestData.Data);
            try
            {
                action(userMakingRequest, clientRequest, serverResponse);
            }
            catch (FactorisableException e)
            {
                serverResponse.FactorisableExceptionType = e.FactorisableExceptionType;
                serverResponse.FactorisableExceptionParameters = e.GetParameters();
                exceptionThrowed = true;
            }
            serverResponseData.Data = Utils.GetBytesFromObject(serverResponse);
            SendToClient(clientKey, Utils.GetBytesFromObject(serverResponseData));
            return exceptionThrowed;
        }
    }
}
