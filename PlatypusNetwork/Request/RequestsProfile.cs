using PlatypusNetwork.Exceptions;

namespace PlatypusNetwork.Request
{
    public abstract class RequestsProfile<ExceptionEnumType, RequestType>
        where RequestType : Enum
        where ExceptionEnumType : Enum
    {
        public Dictionary<RequestType, RequestDefinitionBase<ExceptionEnumType>> Requests { get; private set; }
        private ExceptionFactory<ExceptionEnumType> _exceptionFactory;

        public RequestsProfile(ExceptionFactoryProfile<ExceptionEnumType> exceptionFactoryProfile)
        {
            Requests = new Dictionary<RequestType, RequestDefinitionBase<ExceptionEnumType>>();
            _exceptionFactory = new ExceptionFactory<ExceptionEnumType>(exceptionFactoryProfile);
        }

        protected void MapRequest<ClientRequestType, ServerResponseType>(RequestType requestType) 
            where ClientRequestType : ClientRequestBase, new()
            where ServerResponseType : ServerResponseBase<ExceptionEnumType>
        {
            RequestDefinition<ExceptionEnumType, RequestType, ClientRequestType, ServerResponseType> definition = new RequestDefinition<ExceptionEnumType, RequestType, ClientRequestType, ServerResponseType>(_exceptionFactory, requestType);

            if (Requests.ContainsKey(requestType))
            {
                Requests[requestType] = definition;
                return;
            }

            Requests.Add(requestType, definition);
        }
    }
}
