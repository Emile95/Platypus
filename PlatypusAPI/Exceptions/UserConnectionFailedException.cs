using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    [Serializable]
    public class UserConnectionFailedException : PlatypusException
    {
        public UserConnectionFailedException(string message)
            : base(PlatypusExceptionType.UserConnectionFailed, message) { }
    }
}
