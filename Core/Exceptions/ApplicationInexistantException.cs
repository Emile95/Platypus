using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class ApplicationInexistantException : Exception
    {
        public ApplicationInexistantException(string applicationGuid)
            : base(Utils.GetString(Strings.ResourceManager, "NoApplicationWithGuid", applicationGuid))
        { }
    }
}
