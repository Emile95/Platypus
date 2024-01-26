using Persistance.Repository;
using PlatypusFramework.Core.ApplicationAction.Logger;

namespace PlatypusFramework.Core.ApplicationAction
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
