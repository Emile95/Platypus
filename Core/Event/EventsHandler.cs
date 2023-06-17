using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.Event;
using PlatypusApplicationFramework.Core.Event;
using System.Reflection;

namespace Core.Event
{
    public class EventsHandler
    {
        public Dictionary<EventHandlerType, List<Action<EventHandlerEnvironment>>> EventHandlers { get; private set; }

        public EventsHandler()
        {
            EventHandlers = new Dictionary<EventHandlerType, List<Action<EventHandlerEnvironment>>>();
        }

        public void AddEventHandler(PlatypusApplicationBase application, EventHandlerAttribute eventHandlerAttribute, MethodInfo methodInfo)
        {
            if (EventHandlers.ContainsKey(eventHandlerAttribute.EventHandlerType) == false)
                EventHandlers[eventHandlerAttribute.EventHandlerType] = new List<Action<EventHandlerEnvironment>>();

            Action<EventHandlerEnvironment> action = (env) => methodInfo.Invoke(application, new object[] { env });
            EventHandlers[eventHandlerAttribute.EventHandlerType].Add(action);
        }

        public void RunEventHandlers(EventHandlerType type, EventHandlerEnvironment env)
        {
            if (EventHandlers.ContainsKey(type) == false) return;
            foreach(Action<EventHandlerEnvironment> action in EventHandlers[type])
                action(env);
        }
    }
}
