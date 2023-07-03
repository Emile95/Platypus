using Common.Exceptions;
using Common.SocketData.ClientRequest;
using Common.SocketData.ServerResponse;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.Exceptions;
using PlatypusAPI.SocketData.ClientRequest;
using PlatypusAPI.SocketData.ServerResponse;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace PlatypusAPI
{
    public class PlatypusServerApplication
    {
        private readonly PlatypusClientSocketHandler _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        private readonly Dictionary<string, WaitingApplicationRun> _waitingStartApplicationAction;

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;
            _waitingStartApplicationAction = new Dictionary<string, WaitingApplicationRun>();

            _socketHandler.ServerResponseCallBacks[ServerResponseType.ApplicationActionRunResult].Add(StartApplicationServerResponseCallBack);
        }

        public void Disconnect()
        {

        }

        public ApplicationActionRunResult RunApplicationAction(ApplicationActionRunParameter applicationActionRunparameter)
        {
            string guid = GuidGenerator.GenerateFromEnumerable(_waitingStartApplicationAction.Keys);

            WaitingApplicationRun waitingApplicationRun = new WaitingApplicationRun()
            {
                Running = true
            };

            _waitingStartApplicationAction.Add(guid, waitingApplicationRun);

            StartActionClientRequest startActionClientRequest = new StartActionClientRequest()
            {
                StartActionKey = guid,
                Parameters = applicationActionRunparameter
            };

            ClientRequestData clientRequestData = new ClientRequestData()
            {
                ClientRequestType = ClientRequestType.StartApplicationAction,
                Data = Common.Utils.GetBytesFromObject(startActionClientRequest)
            };

            _socketHandler.SendToServer(Common.Utils.GetBytesFromObject(clientRequestData));

            while (waitingApplicationRun.Running)
                Thread.Sleep(1000);

            ApplicationActionRunResult result = waitingApplicationRun.Result;
            _waitingStartApplicationAction.Remove(guid);

            return result;
        }

        private void StartApplicationServerResponseCallBack(byte[] bytes)
        {
            StartActionServerResponse serverResponse = Common.Utils.GetObjectFromBytes<StartActionServerResponse>(bytes);
            
            if (_waitingStartApplicationAction.ContainsKey(serverResponse.StartActionKey) == false) return;

            WaitingApplicationRun waitingApplicationRun = _waitingStartApplicationAction[serverResponse.StartActionKey];

            waitingApplicationRun.Exception = ExceptionFactory.CreateException(serverResponse.FactorisableExceptionType, serverResponse.FactorisableExceptionParameters);
            waitingApplicationRun.Result = serverResponse.Result;
            waitingApplicationRun.Running = false;
        }

        private class WaitingApplicationRun
        {
            public bool Running { get; set; }
            public ApplicationActionRunResult Result { get; set; }
            public FactorisableException Exception { get; set; }
        }
    }
}
