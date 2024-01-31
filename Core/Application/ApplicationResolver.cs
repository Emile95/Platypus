using Core.ApplicationAction;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using Core.Event;
using PlatypusFramework.Configuration.Event;
using PlatypusFramework.Configuration.User;
using Core.User;
using Persistance.Entity;
using Persistance;

namespace Core.Application
{
    public class ApplicationResolver
    {
        private readonly Repository<ApplicationEntity, string> _applicationRepository;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandler;
        private readonly UsersHandler _usersHandler;

        public ApplicationResolver(
            Repository<ApplicationEntity, string> applicationRepository,
            ApplicationActionsHandler applicationActionsHandler,
            EventsHandler eventsHandler,
            UsersHandler usersHandler
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionsHandler = applicationActionsHandler;
            _eventsHandler = eventsHandler;
            _usersHandler = usersHandler;
        }

        public void ResolvePlatypusApplication(PlatypusApplicationBase platypusApplication, string applicationGuid)
        {
            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            ApplicationInitializeEnvironment env = new ApplicationInitializeEnvironment();
            env.ApplicationGuid = applicationGuid;

            platypusApplication.Initialize(env);

            foreach (MethodInfo methodInfo in methods)
            {
                if(ResolvePlatypusApplicationMethod<ActionDefinitionAttribute>(methodInfo, (attribute) => _applicationActionsHandler.AddAction(platypusApplication, applicationGuid, attribute, methodInfo) )) continue;
                if(ResolvePlatypusApplicationMethod<EventHandlerAttribute>(methodInfo, (attribute) => _eventsHandler.AddEventHandler(platypusApplication, attribute, methodInfo) )) continue;
                if(ResolvePlatypusApplicationMethod<UserConnectionMethodCreatorAttribute>(methodInfo, (attribute) => _usersHandler.AddConnectionMethod(platypusApplication, applicationGuid, methodInfo))) continue;
            }
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
