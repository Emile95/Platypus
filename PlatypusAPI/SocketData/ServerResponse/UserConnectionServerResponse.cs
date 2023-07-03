using Common.Exceptions;
using PlatypusAPI.User;

namespace PlatypusAPI.SocketData.ServerResponse
{
    public class UserConnectionServerResponse
    {
        public UserAccount UserAccount { get; set; }
        public FactorisableExceptionType FactorisableExceptionType { get; set; }
        public object[] FactorisableExceptionParameters { get; set; }
    }
}
