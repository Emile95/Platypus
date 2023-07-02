using Common.Exceptions;
using PlatypusAPI.User;

namespace PlatypusAPI.SocketData.ServerResponse
{
    public class UserConnectionServerResponse
    {
        public UserAccount UserAccount { get; set; }
        public PlatypusExceptionType ExceptionType { get; set; }
        public string ExcetionStringParameter{ get; set; }
    }
}
