namespace Core.Exceptions
{
    public class ApplicationActionCanceledException : Exception
    {
        public string CancelMessage { get; set; }
        public ApplicationActionCanceledException(string cancelMessage)
            : base("action has been canceled")
        {
            CancelMessage = cancelMessage;
        }
    }
}
