using PlatypusApplicationFramework.Core.Event;

namespace PlatypusApplicationFramework.Configuration.Event
{
    public class EventHandlerAttribute : Attribute
    {
        public EventHandlerType EventHandlerType { get; set; }
    }
}
