using PlatypusAPI.Ressources;
using PlatypusNetwork.Exceptions;
using PlatypusUtils;

namespace PlatypusAPI.Exceptions
{
    public class InvalidUserConnectionMethodGuidException : FactorisableException<FactorisableExceptionType>
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
