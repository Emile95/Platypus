using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class ApplicationActionCanceledException : Exception
    {
        public string CancelMessage { get; set; }
        public ApplicationActionCanceledException(string cancelMessage)
            : base(Utils.GetString(Strings.ResourceManager,"ApplicationActionCancelled"))
        {
            CancelMessage = cancelMessage;
        }
    }
}
