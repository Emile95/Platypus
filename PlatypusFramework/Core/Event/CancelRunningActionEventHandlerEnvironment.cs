namespace PlatypusFramework.Core.Event
{
    public class CancelRunningActionEventHandlerEnvironment : EventHandlerEnvironment
    {
        public string RunningActionGuid { get; set; }
    }
}
