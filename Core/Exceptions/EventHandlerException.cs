using PlatypusApplicationFramework.Core.Event;

namespace Core.Exceptions
{
    public class EventHandlerException : Exception
    {
        public string EventHandlerName { get; private set; }
        public EventHandlerType EventHandlerType { get; private set; }
        public EventHandlerException(EventHandlerType eventHandlerType, string eventHandlerName, string message)
            : base($"error during event handler '{eventHandlerName}', message : {message}")
        {
            EventHandlerType = eventHandlerType;
            EventHandlerName = eventHandlerName;
        }
    }
}
