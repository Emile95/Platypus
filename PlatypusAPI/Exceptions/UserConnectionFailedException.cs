using PlatypusNetwork.Exceptions;

namespace PlatypusAPI.Exceptions
{
    [Serializable]
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
