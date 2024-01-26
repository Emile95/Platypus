using Core.Ressource;
using PlatypusApplicationFramework.Core.Event;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class EventHandlerException : Exception
    {
        public string EventHandlerName { get; private set; }
        public EventHandlerType EventHandlerType { get; private set; }
        public EventHandlerException(EventHandlerType eventHandlerType, string eventHandlerName, string message)
            : base(Utils.GetString(Strings.ResourceManager, "ErrorDuringEventHandler", eventHandlerName, message))
        {
            EventHandlerType = eventHandlerType;
            EventHandlerName = eventHandlerName;
        }
    }
}
