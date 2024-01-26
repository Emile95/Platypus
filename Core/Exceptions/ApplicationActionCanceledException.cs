namespace Core.Exceptions
{
    public class ApplicationActionCanceledException : Exception
    {
        public string CancelMessage { get; set; }
        public ApplicationActionCanceledException(string cancelMessage)
            : base(PlatypusNetwork.Utils.GetString("ApplicationActionCancelled"))
        {
            CancelMessage = cancelMessage;
        }
    }
}
