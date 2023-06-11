using PlatypusApplicationFramework.ApplicationAction.Logger;

namespace PlatypusApplicationFramework.ApplicationAction
{
    public class ApplicationActionEnvironmentBase
    {
        public bool ActionCancelled { get; set; }
        public Action<string> AssertFailed { get; set; }
        public Action<string, Action> AssertCanceled { get; set; }
        public ApplicationActionRunLoggers ActionLoggers { get; set; }

    }
}
