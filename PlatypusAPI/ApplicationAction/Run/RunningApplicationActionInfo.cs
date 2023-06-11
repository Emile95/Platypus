namespace PlatypusAPI.ApplicationAction.Run
{
    public class RunningApplicationActionInfo
    {
        public string Guid { get; set; }
        public RunningApplicationActionStatus Status { get; set; }
        public int RunNumber { get; set; }
    }
}
