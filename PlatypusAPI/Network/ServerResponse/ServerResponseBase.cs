using PlatypusAPI.Exceptions;

namespace PlatypusAPI.Network.ServerResponse
{
    public class ServerResponseBase
    {
        public string RequestKey { get; set; }
        public FactorisableExceptionType FactorisableExceptionType { get; set; }
        public object[] FactorisableExceptionParameters { get; set; }
    }
}
