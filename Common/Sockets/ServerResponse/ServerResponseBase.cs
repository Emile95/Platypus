using Common.Exceptions;

namespace Common.Sockets.ServerResponse
{
    public abstract class ServerResponseBase
    {
        public FactorisableExceptionType FactorisableExceptionType { get; set; }
        public object[] FactorisableExceptionParameters { get; set; }
    }
}
