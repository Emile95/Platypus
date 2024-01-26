using PlatypusAPI.Ressources;
using PlatypusUtils;

namespace PlatypusAPI.Exceptions
{
    public class InvalidUserConnectionMethodGuidException : FactorisableException
    {
        public string UserConnectionMethodGuid { get; private set; }

        public InvalidUserConnectionMethodGuidException(string providedGuid)
            : base(FactorisableExceptionType.InvalidUserConnectionMethodGuid, Utils.GetString(Strings.ResourceManager, "NoUserConnectionMethodWithGuid", providedGuid)) 
        {
            UserConnectionMethodGuid = providedGuid;
        }

        public override object[] GetParameters()
        {
            return new object[] { UserConnectionMethodGuid };
        }
    }
}
