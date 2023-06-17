using Core.ApplicationAction;
using Persistance;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Core.Event;
using PlatypusApplicationFramework.Configuration.Event;

namespace Core.Application
{
    public class ApplicationResolver
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandler;

        public ApplicationResolver(
            ApplicationRepository applicationRepository,
            ApplicationActionsHandler applicationActionsHandler,
            EventsHandler eventsHandler
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionsHandler = applicationActionsHandler;
            _eventsHandler = eventsHandler;
        }

        public void ResolvePlatypusApplication(PlatypusApplicationBase platypusApplication, string applicationGuid)
        {
            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach(MethodInfo methodInfo in methods)
            {
                if(ResolvePlatypusApplicationMethod<ActionDefinitionAttribute>(methodInfo, (attribute) => _applicationActionsHandler.AddAction(platypusApplication, applicationGuid, attribute, methodInfo) )) continue;
                if(ResolvePlatypusApplicationMethod<EventHandlerAttribute>(methodInfo, (attribute) => _eventsHandler.AddEventHandler(platypusApplication, attribute, methodInfo) )) continue;
            }

            ApplicationInitializeEnvironment env = new ApplicationInitializeEnvironment();
            env.ApplicationRepository = _applicationRepository;
            env.ApplicationGuid = applicationGuid;

            platypusApplication.Initialize(env);
        }

        private bool ResolvePlatypusApplicationMethod<AttributeType>(MethodInfo methodInfo, Action<AttributeType> consumer)
            where AttributeType : Attribute
        {
            AttributeType attribute = methodInfo.GetCustomAttribute<AttributeType>();
            if (attribute == null) return false;
            consumer(attribute);
            return true;
        }
    }
}
