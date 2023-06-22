namespace PlatypusAPI.Exceptions
{
    public class InvalidUserConnectionMethodGuidException : Exception
    {
        public InvalidUserConnectionMethodGuidException(string providedGuid)
            : base($"there is no user connection method with the guid '{providedGuid}'") {}
    }
}
