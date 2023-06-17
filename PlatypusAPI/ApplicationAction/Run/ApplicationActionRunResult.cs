namespace PlatypusAPI.ApplicationAction.Run
{
    public class ApplicationActionRunResult
    {
        public ApplicationActionRunResultStatus Status { get; set; }
        public string Message { get; set; }
        public object ResultObject { get; set; }
    }
}
