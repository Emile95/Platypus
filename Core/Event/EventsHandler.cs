using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.Event;
using PlatypusApplicationFramework.Core.Event;
using System.Reflection;

namespace Core.Event
{
    public class EventsHandler
    {
        private readonly Dictionary<EventHandlerType, List<EventHandler>> _eventHandlers;

        public EventsHandler()
        {
            _eventHandlers = new Dictionary<EventHandlerType, List<EventHandler>>();
        }

        public void AddEventHandler(PlatypusApplicationBase application, EventHandlerAttribute eventHandlerAttribute, MethodInfo methodInfo)
        {
            if (_eventHandlers.ContainsKey(eventHandlerAttribute.EventHandlerType) == false)
                _eventHandlers[eventHandlerAttribute.EventHandlerType] = new List<EventHandler>();

            EventHandler eventhandler = new EventHandler(application, eventHandlerAttribute, methodInfo);
            _eventHandlers[eventHandlerAttribute.EventHandlerType].Add(eventhandler);
        }

        public void RunEventHandlers(EventHandlerType type, EventHandlerEnvironment env)
        {
            if (_eventHandlers.ContainsKey(type) == false) return;
            foreach (EventHandler eventHandler in _eventHandlers[type])
                eventHandler.RunEventHandler(env);
        }
    }
}
