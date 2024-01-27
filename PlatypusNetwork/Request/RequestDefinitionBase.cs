namespace PlatypusNetwork.Request
{
    public class RequestDefinitionBase<ExceptionEnumType, RequestType>
        where ExceptionEnumType : Enum
        where RequestType : Enum
    {
        protected RequestType _requestType;

        public RequestDefinitionBase(RequestType requestType) 
        {
            _requestType = requestType;
        }
    }
}
