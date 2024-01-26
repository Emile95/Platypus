using PlatypusFramework.Core.Event;

namespace Core.Application
{
    public class UninstallApplicationDetails
    {
        public UninstallApplicationEventHandlerEnvironment EventEnv { get; set; }
        public List<string> ActionGuids { get; set; }
        public List<string> UserConnectionMethodGuids { get; set; }
    }
}
