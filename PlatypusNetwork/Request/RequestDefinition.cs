using PlatypusNetwork.Exceptions;
using PlatypusUtils;

namespace PlatypusNetwork.Request
{
    public class RequestDefinition<ExceptionEnumType, RequestType, ClientRequestType, ServerResponseType> : RequestDefinitionBase<ExceptionEnumType>
        where ExceptionEnumType:  Enum
        where RequestType : Enum
        where ClientRequestType : ClientRequestBase, new()
        where ServerResponseType : ServerResponseBase<ExceptionEnumType>
    {
        private RequestType _requestType;
        private Dictionary<string, ServerResponseWaiter<ExceptionEnumType, ServerResponseType>> _serverResponseWaiters;

        public RequestDefinition(
            ExceptionFactory<ExceptionEnumType> exceptionFactory, 
            RequestType requestType
        )
            : base(exceptionFactory) 
        {
            _serverResponseWaiters = new Dictionary<string, ServerResponseWaiter<ExceptionEnumType, ServerResponseType>>();
            _requestType = requestType;
        }

        public ServerResponseType HandleClientRequest(Action<RequestData<RequestType>> requestAction, Action<ClientRequestType> consumer = null)
        {
            string guid = Utils.GenerateGuidFromEnumerable(_serverResponseWaiters.Keys);

            ServerResponseWaiter<ExceptionEnumType, ServerResponseType> serverResponseWaiter = new ServerResponseWaiter<ExceptionEnumType, ServerResponseType>();

            _serverResponseWaiters.Add(guid, serverResponseWaiter);

            ClientRequestType clientRequest = new ClientRequestType()
            {
                RequestKey = guid
            };

            if (consumer != null)
                consumer(clientRequest);

            RequestData<RequestType> clientRequestData = new RequestData<RequestType>()
            {
                RequestType = _requestType,
                Data = Utils.GetBytesFromObject(clientRequest)
            };

            requestAction(clientRequestData);

            while (serverResponseWaiter.Received == false) Thread.Sleep(200);

            if (serverResponseWaiter.Response.ExceptionThrowed)
                throw _exceptionFactory.CreateException(serverResponseWaiter.Response.FactorisableExceptionType, serverResponseWaiter.Response.FactorisableExceptionParameters);

            _serverResponseWaiters.Remove(guid);

            return serverResponseWaiter.Response;
        }

        public override void ServerResponseCallBack(byte[] bytes)
        {
            ServerResponseType serverResponse = Utils.GetObjectFromBytes<ServerResponseType>(bytes);

            if (_serverResponseWaiters.ContainsKey(serverResponse.RequestKey) == false) return;

            ServerResponseWaiter<ExceptionEnumType, ServerResponseType> serverResponseWaiter = _serverResponseWaiters[serverResponse.RequestKey];

            serverResponseWaiter.Received = true;
            serverResponseWaiter.Response = serverResponse;
        }
    }
}
