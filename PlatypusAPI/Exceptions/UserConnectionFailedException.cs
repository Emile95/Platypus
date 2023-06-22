namespace PlatypusAPI.Exceptions
{
    public class UserConnectionFailedException : Exception
    {
        public UserConnectionFailedException(string message)
            : base(message) { }
    }
}
