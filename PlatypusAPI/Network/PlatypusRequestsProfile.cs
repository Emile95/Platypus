﻿using PlatypusAPI.Exceptions;
using PlatypusAPI.Network.ClientRequest;
using PlatypusAPI.Network.ServerResponse;
using PlatypusNetwork.Request;

namespace PlatypusAPI.Network
{
    public class PlatypusRequestsProfile : RequestsProfile<FactorisableExceptionType, RequestType>
    {
        public PlatypusRequestsProfile(bool profileForCLient = true)
            : base(new PlatypusExceptionFactoryProfile(), profileForCLient)
        {
            MapRequest<StartActionClientRequest, StartActionServerResponse>(RequestType.RunApplicationAction);
            MapRequest<CancelRunningApplicationRunClientRequest, PlatypusServerResponse>(RequestType.CancelRunningAction);
            MapRequest<PlatypusClientRequest, GetRunningApplicationActionsServerResponse>(RequestType.GetRunningActions);
            MapRequest<PlatypusClientRequest, GetApplicationActionInfosServerResponse>(RequestType.GetActionInfos);
            MapRequest<PlatypusClientRequest, GetApplicationInfosServerResponse>(RequestType.GetApplicationInfos);
            MapRequest<UserConnectionClientRequest, UserConnectionServerResponse>(RequestType.UserConnection);
            MapRequest<UpdateUserClientRequest, UpdateUserServerResponse>(RequestType.UpdateUser);
            MapRequest<AddUserClientRequest, AddUserServerResponse>(RequestType.AddUser);
            MapRequest<RemoveUserClientRequest, RemoveUserServerResponse>(RequestType.RemoveUser);
        }
    }
}
