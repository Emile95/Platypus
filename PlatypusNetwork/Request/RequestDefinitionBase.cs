using PlatypusNetwork.Exceptions;

namespace PlatypusNetwork.Request
{
    public abstract class RequestDefinitionBase<ExceptionEnumType>
        where ExceptionEnumType : Enum
    {
        protected ExceptionFactory<ExceptionEnumType> _exceptionFactory;

        public RequestDefinitionBase(ExceptionFactory<ExceptionEnumType> exceptionFactory)
        {
            _exceptionFactory = exceptionFactory;
        }

        public abstract void ServerResponseCallBack(byte[] bytes);
    }
}
