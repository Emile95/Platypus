using Core.Exceptions;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.Event;
using PlatypusFramework.Core.Event;
using System.Reflection;

namespace Core.Event
{
    public class EventsHandler
    {
        private readonly Dictionary<EventHandlerType, List<EventHandler>> _eventHandlers;

        public EventsHandler()
        {
            _eventHandlers = new Dictionary<EventHandlerType, List<EventHandler>>();

            foreach (EventHandlerType eventHandlerType in Enum.GetValues(typeof(EventHandlerType)))
                _eventHandlers.Add(eventHandlerType, new List<EventHandler>());
        }

        public void AddEventHandler(PlatypusApplicationBase application, EventHandlerAttribute eventHandlerAttribute, MethodInfo methodInfo)
        {
            EventHandler eventhandler = new EventHandler(application, eventHandlerAttribute, methodInfo);
            _eventHandlers[eventHandlerAttribute.EventHandlerType].Add(eventhandler);
        }

        public T RunEventHandlers<T>(EventHandlerType eventHandlerType, EventHandlerEnvironment eventEnv, Func<EventHandlerException, T> exceptionObjectCreator)
            where T : class
        {
            try
            {
                RunEventHandlers(eventHandlerType, eventEnv);
            }
            catch (EventHandlerException ex)
            {
                return exceptionObjectCreator(ex);
            }

            return null;
        }

        private void RunEventHandlers(EventHandlerType type, EventHandlerEnvironment env)
        {
            foreach (EventHandler eventHandler in _eventHandlers[type])
                eventHandler.RunEventHandler(env);
        }
    }
}
