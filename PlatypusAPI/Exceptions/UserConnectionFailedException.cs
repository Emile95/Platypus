using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    [Serializable]
    public class UserConnectionFailedException : FactorisableException
    {
        public UserConnectionFailedException(string message)
            : base(FactorisableExceptionType.UserConnectionFailed, message) { }

        public override object[] GetParameters()
        {
            return new object[] { Message };
        }
    }
}
