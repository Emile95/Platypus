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
using System.Net.Sockets;

namespace Core.Network
{
    public class PlatypusServerSocketHandler : ServerSocketHandler<FactorisableExceptionType, RequestType>, ISeverPortListener<PlatypusServerSocketHandler>
    {
        private readonly Dictionary<string, UserAccount> _connectedUserOnSockets;

        public PlatypusServerSocketHandler(
            ServerInstance serverInstance,
            ProtocolType protocol
        ) : base(protocol, new PlatypusRequestsProfile(false))
        {
            _connectedUserOnSockets = new Dictionary<string, UserAccount>();
            MapServerActions(serverInstance);
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return Utils.GenerateGuidFromEnumerable(currentKeys);
        }

        public override void OnAccept(ClientReceivedState receivedState)
        {
            Console.WriteLine($"new client connected, key='{receivedState.ClientKey}'");
            _connectedUserOnSockets.Add(receivedState.ClientKey, null);
        }

        public override void OnLostSocket(ClientReceivedState receivedState)
        {
            Console.WriteLine($"client lost, key='{receivedState.ClientKey}'");
        }

        private void MapServerActions(ServerInstance serverInstance)
        {
            _requestsProfile.MapServerAction<UserConnectionClientRequest, UserConnectionServerResponse>(RequestType.UserConnection, (clientKey, clientRequest, serverResponse) =>
            {
                serverResponse.UserAccount = serverInstance.UserConnect(new UserConnectionParameter()
                {
                    ConnectionMethodGuid = clientRequest.ConnectionMethodGuid,
                    Credential = clientRequest.Credential,
                });
                _connectedUserOnSockets[clientKey] = serverResponse.UserAccount;
            });

            _requestsProfile.MapServerAction<StartActionClientRequest, StartActionServerResponse>(RequestType.RunApplicationAction, (clientKey, clientRequest, serverResponse) =>
            {
                serverResponse.Result = serverInstance.RunAction(clientRequest.UserAccount, clientRequest.Parameters);
            });

            _requestsProfile.MapServerAction<AddUserClientRequest, AddUserServerResponse>(RequestType.AddUser, (clientKey, clientRequest, serverResponse) =>
            {
                serverResponse.UserAccount = serverInstance.AddUser(
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
            });

            _requestsProfile.MapServerAction<UpdateUserClientRequest, UpdateUserServerResponse>(RequestType.UpdateUser, (clientKey, clientRequest, serverResponse) =>
            {
                serverResponse.UserAccount = serverInstance.UpdateUser(
                    clientRequest.UserAccount,
                    new UserUpdateParameter()
                    {
                        Guid = clientRequest.Guid,
                        ConnectionMethodGuid = clientRequest.ConnectionMethodGuid,
                        Email = clientRequest.Email,
                        Data = clientRequest.Data,
                        FullName = clientRequest.FullName,
                        UserPermissionFlags = clientRequest.UserPermissionFlags
                    }
                );
            });

            _requestsProfile.MapServerAction<RemoveUserClientRequest, RemoveUserServerResponse>(RequestType.RemoveUser, (clientKey, clientRequest, serverResponse) =>
            {
                serverInstance.RemoveUser(
                    clientRequest.UserAccount,
                    new RemoveUserParameter()
                    {
                        Guid = clientRequest.Guid,
                    }
                );
            });

            _requestsProfile.MapServerAction<PlatypusClientRequest, GetRunningApplicationActionsServerResponse>(RequestType.GetRunningActions, (clientKey, clientRequest, serverResponse) =>
            {
                IEnumerable<ApplicationActionRunInfo> result = serverInstance.GetRunningApplicationActions(clientRequest.UserAccount);
                if (result.Count() != 0)
                    serverResponse.ApplicationActionRunInfos = result.ToList();
                else
                    serverResponse.ApplicationActionRunInfos = new List<ApplicationActionRunInfo>();
            });

            _requestsProfile.MapServerAction<PlatypusClientRequest, GetApplicationActionInfosServerResponse>(RequestType.GetActionInfos, (clientKey, clientRequest, serverResponse) =>
            {
                IEnumerable<ApplicationActionInfo> result = serverInstance.GetApplicationActionInfos(clientRequest.UserAccount);
                if (result.Count() != 0)
                    serverResponse.ApplicationActionInfos = result.ToList();
                else
                    serverResponse.ApplicationActionInfos = new List<ApplicationActionInfo>();
            });

            _requestsProfile.MapServerAction<CancelRunningApplicationRunClientRequest, PlatypusServerResponse>(RequestType.CancelRunningAction, (clientKey, clientRequest, serverResponse) =>
            {
                serverInstance.CancelRunningApplicationAction(
                    clientRequest.UserAccount,
                    new CancelRunningActionParameter()
                    {
                        Guid = clientRequest.ApplicationRunGuid
                    }
                );
            });
        }

        public void InitializeServerPortListener(int port)
        {
            Initialize(port);
        }
    }
}
