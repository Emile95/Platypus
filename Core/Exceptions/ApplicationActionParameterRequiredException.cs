using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class ApplicationActionParameterRequiredException : Exception
    {
        public ApplicationActionParameterRequiredException(string actionName)
            : base(Utils.GetString(Strings.ResourceManager,"ParameterRequieredForApplicationAction", actionName))
        { }
    }
}
