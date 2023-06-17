using Core.Exceptions;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.Event;
using PlatypusApplicationFramework.Core.Event;
using System.Reflection;

namespace Core.Event
{
    public class EventsHandler
    {
        public Dictionary<EventHandlerType, List<EventHandler>> EventHandlers { get; private set; }

        public EventsHandler()
        {
            EventHandlers = new Dictionary<EventHandlerType, List<EventHandler>>();
        }

        public void AddEventHandler(PlatypusApplicationBase application, EventHandlerAttribute eventHandlerAttribute, MethodInfo methodInfo)
        {
            if (EventHandlers.ContainsKey(eventHandlerAttribute.EventHandlerType) == false)
                EventHandlers[eventHandlerAttribute.EventHandlerType] = new List<EventHandler>();

            EventHandler eventhandler = new EventHandler(application, eventHandlerAttribute, methodInfo);
            EventHandlers[eventHandlerAttribute.EventHandlerType].Add(eventhandler);
        }

        public void RunEventHandlers(EventHandlerType type, EventHandlerEnvironment env)
        {
            if (EventHandlers.ContainsKey(type) == false) return;
            foreach (EventHandler eventHandler in EventHandlers[type])
                eventHandler.RunEventHandler(env);
        }
    }
}
