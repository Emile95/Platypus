using PlatypusNetwork.Exceptions;

namespace PlatypusNetwork.Request
{
    public abstract class ClientRequestSenderDefinitionBase<ExceptionEnumType, RequestTypeEnum> : RequestDefinitionBase<ExceptionEnumType, RequestTypeEnum>
        where ExceptionEnumType : Enum
        where RequestTypeEnum : Enum
    {
        protected ExceptionFactory<ExceptionEnumType> _exceptionFactory;

        public ClientRequestSenderDefinitionBase(RequestTypeEnum requestType, ExceptionFactory<ExceptionEnumType> exceptionFactory)
            : base(requestType)
        {
            _exceptionFactory = exceptionFactory;
        }

        public abstract void ServerResponseCallBack(byte[] bytes);
    }
}
