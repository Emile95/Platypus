namespace Core.Exceptions
{
    public class ApplicationActionRunFailedException : Exception
    {
        public string FailedMessage {get; set;}

        public ApplicationActionRunFailedException(string failedMessage)
            : base("Action as failed")
        {
            FailedMessage = failedMessage;
        }
    }
}
