using PlatypusNetwork.Exceptions;

namespace PlatypusAPI.Exceptions
{
    public class UserConnectionFailedException : FactorisableException<FactorisableExceptionType>
    {
        public UserConnectionFailedException(string message)
            : base(FactorisableExceptionType.UserConnectionFailed, message) { }

        public override object[] GetParameters()
        {
            return new object[] { Message };
        }
    }
}
