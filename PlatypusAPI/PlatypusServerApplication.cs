using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.Exceptions;
using PlatypusAPI.Network;
using PlatypusAPI.Network.ClientRequest;
using PlatypusAPI.Network.ServerResponse;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;
using PlatypusNetwork.SocketHandler;
using System.Collections.Concurrent;
using PlatypusUtils;
using PlatypusNetwork.Request.Data;

namespace PlatypusAPI
{
    public partial class PlatypusServerApplication
    {
        private readonly ClientSocketHandler<FactorisableExceptionType, RequestType> _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        private ConcurrentQueue<string> _requestTickets;

        public PlatypusServerApplication(
            ClientSocketHandler<FactorisableExceptionType, RequestType> socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;
            _requestTickets = new ConcurrentQueue<string>();
        }

        public void Disconnect()
        {

        }

        public ApplicationActionRunResult RunApplicationAction(ApplicationActionRunParameter applicationActionRunparameter)
        {


            StartActionServerResponse response = _socketHandler.HandleClientRequest<StartActionClientRequest, StartActionServerResponse>(
                RequestType.RunApplicationAction,
                (clientRequest) => {
                    clientRequest.UserAccount = ConnectedUser;
                    clientRequest.Parameters = applicationActionRunparameter;
                }
            );
            return response.Result;
        }

        public UserAccount AddUser(UserCreationParameter parameter)
        {
            AddUserServerResponse response = _socketHandler.HandleClientRequest<AddUserClientRequest, AddUserServerResponse>(
                RequestType.AddUser,
                (clientRequest) => {
                    clientRequest.UserAccount = ConnectedUser;
                    clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                    clientRequest.FullName = parameter.FullName;
                    clientRequest.Email = parameter.Email;
                    clientRequest.Data = parameter.Data;
                    clientRequest.UserPermissionFlags = parameter.UserPermissionFlags;
                }
            );
            return response.UserAccount;
        }

        public UserAccount UpdateUser(UserUpdateParameter parameter)
        {
            UpdateUserServerResponse response = _socketHandler.HandleClientRequest<UpdateUserClientRequest, UpdateUserServerResponse>(
                RequestType.UpdateUser,
                (clientRequest) => {
                    clientRequest.UserAccount = ConnectedUser;
                    clientRequest.Guid = parameter.Guid;
                    clientRequest.ConnectionMethodGuid = parameter.ConnectionMethodGuid;
                    clientRequest.FullName = parameter.FullName;
                    clientRequest.Email = parameter.Email;
                    clientRequest.Data = parameter.Data;
                    clientRequest.UserPermissionFlags = parameter.UserPermissionFlags;
                }
            );
            return response.UserAccount;
        }

        public void RemoveUser(RemoveUserParameter parameter)
        {
            RemoveUserServerResponse response = _socketHandler.HandleClientRequest<RemoveUserClientRequest, RemoveUserServerResponse>(
                RequestType.RemoveUser,
                (clientRequest) => {
                    clientRequest.UserAccount = ConnectedUser;
                    clientRequest.Guid = parameter.Guid;
                }
            );
        }

        public List<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            GetRunningApplicationActionsServerResponse response = _socketHandler.HandleClientRequest<PlatypusClientRequest, GetRunningApplicationActionsServerResponse>(
                RequestType.GetRunningActions,
                (clientRequest) => clientRequest.UserAccount = ConnectedUser
            );
            return response.ApplicationActionRunInfos;
        }

        public List<ApplicationActionInfo> GetApplicationActionInfos()
        {
            GetApplicationActionInfosServerResponse response = _socketHandler.HandleClientRequest<PlatypusClientRequest, GetApplicationActionInfosServerResponse>(
                RequestType.GetActionInfos,
                (clientRequest) => clientRequest.UserAccount = ConnectedUser
            );
            return response.ApplicationActionInfos;
        }

        public void CancelRunningApplicationAction(string applicationRunGuid)
        {
            PlatypusServerResponse response = _socketHandler.HandleClientRequest<CancelRunningApplicationRunClientRequest, PlatypusServerResponse>(
                RequestType.CancelRunningAction,
                (clientRequest) => {
                    clientRequest.UserAccount = ConnectedUser;
                    clientRequest.ApplicationRunGuid = applicationRunGuid;
                }
            );
        }
    }
}
