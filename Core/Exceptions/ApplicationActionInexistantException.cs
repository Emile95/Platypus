using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class ApplicationActionInexistantException : Exception
    {
        public ApplicationActionInexistantException(string actionGuid)
            : base(Utils.GetString(Strings.ResourceManager, "NoApplicationActionWithGuid", actionGuid))
        {}
    }
}
