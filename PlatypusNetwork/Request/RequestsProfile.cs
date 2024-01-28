using PlatypusNetwork.Exceptions;
using PlatypusNetwork.Request.Data;
using PlatypusNetwork.Request.Definition;

namespace PlatypusNetwork.Request
{
    public abstract class RequestsProfile<ExceptionEnumType, RequestTypeEnumType>
        where RequestTypeEnumType : Enum
        where ExceptionEnumType : Enum
    {
        public Dictionary<RequestTypeEnumType, RequestDefinitionBase<ExceptionEnumType, RequestTypeEnumType>> RequestDefinitions { get; private set; }

        private ExceptionFactory<ExceptionEnumType> _exceptionFactory;
        private bool _profileForClient;

        public RequestsProfile(ExceptionFactoryProfile<ExceptionEnumType> exceptionFactoryProfile, bool profileForClient = true)
        {
            RequestDefinitions = new Dictionary<RequestTypeEnumType, RequestDefinitionBase<ExceptionEnumType, RequestTypeEnumType>>();

            if(profileForClient)
                _exceptionFactory = new ExceptionFactory<ExceptionEnumType>(exceptionFactoryProfile);

            _profileForClient = profileForClient;
        }

        protected void MapRequest<ClientRequestType, ServerResponseType>(RequestTypeEnumType requestType) 
            where ClientRequestType : ClientRequestBase, new()
            where ServerResponseType : ServerResponseBase<ExceptionEnumType>, new()
        {
            RequestDefinitionBase<ExceptionEnumType, RequestTypeEnumType> requestDefinition = null;

            if (_profileForClient) requestDefinition = new ClientRequestSenderDefinition<ExceptionEnumType, RequestTypeEnumType, ClientRequestType, ServerResponseType>(requestType, _exceptionFactory);
            else requestDefinition = new ClientRequestReceiverDefinition<ExceptionEnumType, RequestTypeEnumType, ClientRequestType, ServerResponseType>(requestType);

            if (RequestDefinitions.ContainsKey(requestType))
            {
                RequestDefinitions[requestType] = requestDefinition;
                return;
            }

            RequestDefinitions.Add(requestType, requestDefinition);

        }

        public void MapServerAction<ClientRequestType, ServerResponseType>(RequestTypeEnumType requestType, Action<string, ClientRequestType, ServerResponseType> serverAction)
            where ClientRequestType : ClientRequestBase
            where ServerResponseType : ServerResponseBase<ExceptionEnumType>, new()
        {
            if (RequestDefinitions.ContainsKey(requestType) == false) return;
            var clientReceiverDefinition = RequestDefinitions[requestType] as ClientRequestReceiverDefinition<ExceptionEnumType, RequestTypeEnumType, ClientRequestType, ServerResponseType>;
            clientReceiverDefinition.ServerAction = serverAction;
        }
    }
}
