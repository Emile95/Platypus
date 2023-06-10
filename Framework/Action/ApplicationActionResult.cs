namespace PlatypusApplicationFramework.Action
{
    public class ApplicationActionResult
    {
        public ApplicationActionResultStatus Status { get; set; }
        public string Message { get; set; }
        public object ResultObject { get; set; }
    }
}
