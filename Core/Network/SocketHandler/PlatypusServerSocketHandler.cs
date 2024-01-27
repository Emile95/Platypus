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
using PlatypusNetwork.Exceptions;
using PlatypusNetwork;
using System.Net.Sockets;
using PlatypusNetwork.Request;

namespace Core.Network.SocketHandler
{
    internal class PlatypusServerSocketHandler : ServerSocketHandler<FactorisableExceptionType, RequestType, string>
    {
        private readonly ServerInstance _serverInstance;
        private readonly int _port;
        private readonly Dictionary<string, UserAccount> _connectedUserOnSockets;

        public PlatypusServerSocketHandler(
            ServerInstance serverInstance,
            ProtocolType protocol,
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
            RequestData<RequestType> clientRequest = Utils.GetObjectFromBytes<RequestData<RequestType>>(receivedState.BytesRead);

            if (clientRequest == null) return;

            switch (clientRequest.RequestType)
            {
                case RequestType.UserConnection: ReceiveUserConnectionClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.RunApplicationAction: ReceiveStartApplicationActionClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.AddUser: ReceiveAddUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.UpdateUser: ReceiveUpdateUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.RemoveUser: ReceiveRemoveUserClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.GetRunningActions: ReceiveGetRunningApplicationActionsClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.GetActionInfos: ReceiveGetApplicationActionInfosClientRequest(receivedState.ClientKey, clientRequest); break;
                case RequestType.CancelRunningAction: ReceiveCancelRunningApplicationActionRunClientRequest(receivedState.ClientKey, clientRequest); break;
            }
        }

        public void Initialize(string host)
        {
            Initialize(_port, host);
        }

        private void ReceiveUserConnectionClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            bool exceptionThrowed = HandleClientRequest<UserConnectionClientRequest, UserConnectionServerResponse>(
                clientKey, clientRequestData, RequestType.UserConnection,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.UserAccount = _serverInstance.UserConnect(new UserConnectionParameter()
                    {
                        ConnectionMethodGuid = clientRequest.ConnectionMethodGuid,
                        Credential = clientRequest.Credential,
                    });
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    _connectedUserOnSockets[clientKey] = serverResponse.UserAccount;
                }
            );
            if (exceptionThrowed)
                _clientSockets.Remove(clientKey);
        }

        private void ReceiveStartApplicationActionClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<StartActionClientRequest, StartActionServerResponse>(
                clientKey, clientRequestData, RequestType.RunApplicationAction,
                (userAccount, clientRequest, serverResponse) =>
                {
                    serverResponse.RequestKey = clientRequest.RequestKey;
                    serverResponse.Result = _serverInstance.RunAction(clientRequest.UserAccount, clientRequest.Parameters);
                }
            );
        }

        private void ReceiveAddUserClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<AddUserClientRequest, AddUserServerResponse>(
                clientKey, clientRequestData, RequestType.AddUser,
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

        private void ReceiveUpdateUserClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<UpdateUserClientRequest, UpdateUserServerResponse>(
                clientKey, clientRequestData, RequestType.UpdateUser,
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

        private void ReceiveRemoveUserClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<RemoveUserClientRequest, RemoveUserServerResponse>(
                clientKey, clientRequestData, RequestType.RemoveUser,
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

        private void ReceiveGetRunningApplicationActionsClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<PlatypusClientRequest, GetRunningApplicationActionsServerResponse>(
                clientKey, clientRequestData, RequestType.GetRunningActions,
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

        private void ReceiveGetApplicationActionInfosClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<PlatypusClientRequest, GetApplicationActionInfosServerResponse>(
                clientKey, clientRequestData, RequestType.GetActionInfos,
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

        private void ReceiveCancelRunningApplicationActionRunClientRequest(string clientKey, RequestData<RequestType> clientRequestData)
        {
            HandleClientRequest<CancelRunningApplicationRunClientRequest, AddUserServerResponse>(
                clientKey, clientRequestData, RequestType.CancelRunningAction,
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

        private bool HandleClientRequest<ClientRequestType, ResponseType>(string clientKey, RequestData<RequestType> clientRequestData, PlatypusAPI.Network.RequestType serverResponseType, Action<UserAccount, ClientRequestType, ResponseType> action)
            where ClientRequestType : ClientRequestBase
            where ResponseType : PlatypusServerResponse, new()
        {
            UserAccount userMakingRequest = _connectedUserOnSockets[clientKey];

            bool exceptionThrowed = false;
            RequestData<RequestType> serverResponseData = new RequestData<RequestType>()
            {
                RequestType = serverResponseType
            };
            ResponseType serverResponse = new ResponseType();
            ClientRequestType clientRequest = Utils.GetObjectFromBytes<ClientRequestType>(clientRequestData.Data);
            try
            {
                action(userMakingRequest, clientRequest, serverResponse);
            }
            catch (FactorisableException<FactorisableExceptionType> e)
            {
                serverResponse.ExceptionThrowed = true;
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
