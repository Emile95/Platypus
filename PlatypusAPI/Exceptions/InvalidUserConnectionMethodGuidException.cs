using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    public class InvalidUserConnectionMethodGuidException : PlatypusException
    {
        public InvalidUserConnectionMethodGuidException(string providedGuid)
            : base(PlatypusExceptionType.InvalidUserConnectionMethodGuid, Common.Utils.GetString("NoUserConnectionMethodWithGuid", providedGuid)) 
        {}
    }
}
