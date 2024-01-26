using Core.Exceptions;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.Event;
using PlatypusFramework.Core.Event;
using System.Reflection;

namespace Core.Event
{
    public class EventHandler
    {
        public string Name { get; private set; }
        public EventHandlerType Type { get; private set; }

        private readonly Action<EventHandlerEnvironment> _action;
        
        public EventHandler(PlatypusApplicationBase application, EventHandlerAttribute actionDefinitionAttribute, MethodInfo methodInfo)
        {
            _action = (env) => methodInfo.Invoke(application, new object[] { env });
            Name = application.GetType().Name + "-" + methodInfo.Name + "-" + actionDefinitionAttribute.EventHandlerType.ToString();
            Type = actionDefinitionAttribute.EventHandlerType;
        }

        public void RunEventHandler(EventHandlerEnvironment env)
        {
            try
            {
                _action(env);
            } catch(TargetInvocationException ex)
            {
                if(ex.InnerException is null)
                    throw new EventHandlerException(Type, Name, ex.Message);
                throw new EventHandlerException(Type, Name, ex.InnerException.Message);
            }
        }
    }
}
