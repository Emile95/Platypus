namespace Core.Exceptions
{
    public class ApplicationActionCanceledException : Exception
    {
        public string CancelMessage { get; set; }
        public ApplicationActionCanceledException(string cancelMessage)
            : base(Common.Utils.GetString("ApplicationActionCancelled"))
        {
            CancelMessage = cancelMessage;
        }
    }
}
