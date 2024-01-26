using PlatypusFramework.Core.Event;

namespace PlatypusFramework.Configuration.Event
{
    public class EventHandlerAttribute : Attribute
    {
        public EventHandlerType EventHandlerType { get; set; }
    }
}
