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
            _eventHandlers.Add(EventHandlerType.BeforeApplicationActionRun, new List<EventHandler>());
            _eventHandlers.Add(EventHandlerType.AfterApplicationActionRun, new List<EventHandler>());
        }

        public void AddEventHandler(PlatypusApplicationBase application, EventHandlerAttribute eventHandlerAttribute, MethodInfo methodInfo)
        {
            EventHandler eventhandler = new EventHandler(application, eventHandlerAttribute, methodInfo);
            _eventHandlers[eventHandlerAttribute.EventHandlerType].Add(eventhandler);
        }

        public void RunEventHandlers(EventHandlerType type, EventHandlerEnvironment env)
        {
            foreach (EventHandler eventHandler in _eventHandlers[type])
                eventHandler.RunEventHandler(env);
        }
    }
}
