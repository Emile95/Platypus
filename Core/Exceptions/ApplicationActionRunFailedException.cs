namespace Core.Exceptions
{
    public class ApplicationActionRunFailedException : Exception
    {
        public string FailedMessage {get; set;}

        public ApplicationActionRunFailedException(string failedMessage)
            : base(PlatypusNetwork.Utils.GetString("ApplicationActionFailed"))
        {
            FailedMessage = failedMessage;
        }
    }
}
