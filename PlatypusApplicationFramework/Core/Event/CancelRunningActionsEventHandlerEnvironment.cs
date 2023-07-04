namespace PlatypusApplicationFramework.Core.Event
{
    public class CancelRunningActionsEventHandlerEnvironment : EventHandlerEnvironment
    {
        public string RunningActionGuid { get; set; }
    }
}
