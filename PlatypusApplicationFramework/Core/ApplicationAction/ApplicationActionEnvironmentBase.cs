using Persistance.Repository;
using PlatypusApplicationFramework.Core.ApplicationAction.Logger;

namespace PlatypusApplicationFramework.Core.ApplicationAction
{
    public class ApplicationActionEnvironmentBase
    {
        public bool ActionCancelled { get; set; }
        public Action<string> AssertFailed { get; set; }
        public Action<string, Action> AssertCanceled { get; set; }
        public RunningApplicationActionFileLogger RunningActionFileLogger { get; set; }
        public ApplicationRepository ApplicationRepository { get; set; }
    }
}
