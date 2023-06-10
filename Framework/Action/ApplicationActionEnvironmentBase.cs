namespace PlatypusApplicationFramework.Action
{
    public class ApplicationActionEnvironmentBase
    {
        public bool ActionCancelled { get; set; }
        public Action<string> AssertFailed { get; set; }

    }
}
