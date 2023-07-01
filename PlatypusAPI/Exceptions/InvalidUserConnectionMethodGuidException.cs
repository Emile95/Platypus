namespace PlatypusAPI.Exceptions
{
    public class InvalidUserConnectionMethodGuidException : Exception
    {
        public InvalidUserConnectionMethodGuidException(string providedGuid)
            : base(Common.Utils.GetString("NoUserConnectionMethodWithGuid", providedGuid)) {}
    }
}
