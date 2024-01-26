using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class ApplicationActionFieldRequired : Exception
    {
        public ApplicationActionFieldRequired(string fieldName)
            : base(Utils.GetString(Strings.ResourceManager ,"ApplicationActionFieldRequired", fieldName))
        { }
    }
}
