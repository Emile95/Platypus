using Core.ApplicationAction;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using Core.Event;
using PlatypusFramework.Configuration.Event;
using PlatypusFramework.Configuration.User;
using Core.User;
using Core.Application.Abstract;

namespace Core.Application
{
    internal class ApplicationResolver : IApplicationResolver<PlatypusApplicationBase>
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandler;
        private readonly UsersHandler _usersHandler;

        internal ApplicationResolver(
            ApplicationActionsHandler applicationActionsHandler,
            EventsHandler eventsHandler,
            UsersHandler usersHandler
        )
        {
            _applicationActionsHandler = applicationActionsHandler;
            _eventsHandler = eventsHandler;
            _usersHandler = usersHandler;
        }

        public void Resolve(PlatypusApplicationBase platypusApplication)
        {
            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            ApplicationInitializeEnvironment env = new ApplicationInitializeEnvironment();
            env.ApplicationGuid = platypusApplication.ApplicationGuid;

            platypusApplication.Initialize(env);

            foreach (MethodInfo methodInfo in methods)
            {
                if(ResolvePlatypusApplicationMethod<ActionDefinitionAttribute>(methodInfo, (attribute) => _applicationActionsHandler.AddAction(platypusApplication, platypusApplication.ApplicationGuid, attribute, methodInfo) )) continue;
                if(ResolvePlatypusApplicationMethod<EventHandlerAttribute>(methodInfo, (attribute) => _eventsHandler.AddEventHandler(platypusApplication, attribute, methodInfo) )) continue;
                if(ResolvePlatypusApplicationMethod<UserConnectionMethodCreatorAttribute>(methodInfo, (attribute) => _usersHandler.AddConnectionMethod(platypusApplication, platypusApplication.ApplicationGuid, methodInfo))) continue;
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
