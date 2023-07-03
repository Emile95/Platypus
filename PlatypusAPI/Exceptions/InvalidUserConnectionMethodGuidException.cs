using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    public class InvalidUserConnectionMethodGuidException : FactorisableException
    {
        public string UserConnectionMethodGuid { get; private set; }

        public InvalidUserConnectionMethodGuidException(string providedGuid)
            : base(FactorisableExceptionType.InvalidUserConnectionMethodGuid, Common.Utils.GetString("NoUserConnectionMethodWithGuid", providedGuid)) 
        {
            UserConnectionMethodGuid = providedGuid;
        }

        public override object[] GetParameters()
        {
            return new object[] { UserConnectionMethodGuid };
        }
    }
}
