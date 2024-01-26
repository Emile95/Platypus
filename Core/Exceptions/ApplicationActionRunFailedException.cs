using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class ApplicationActionRunFailedException : Exception
    {
        public string FailedMessage {get; set;}

        public ApplicationActionRunFailedException(string failedMessage)
            : base(Utils.GetString(Strings.ResourceManager, "ApplicationActionFailed"))
        {
            FailedMessage = failedMessage;
        }
    }
}
