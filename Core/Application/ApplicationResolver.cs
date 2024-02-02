using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusFramework.Configuration.Event;
using PlatypusFramework.Configuration.User;
using Core.Application.Abstract;
using Core.Abstract;

namespace Core.Application
{
    internal class ApplicationResolver : IApplicationResolver<PlatypusApplicationBase>
    {
        private readonly IApplicationAttributeMethodResolver<ActionDefinitionAttribute> _applicationActionResolver;
        private readonly IApplicationAttributeMethodResolver<EventHandlerAttribute> _eventsHandlerResolver;
        private readonly IApplicationAttributeMethodResolver<UserConnectionMethodCreatorAttribute> _connectionMethodResolver;

        internal ApplicationResolver(
            IApplicationAttributeMethodResolver<ActionDefinitionAttribute> applicationActionResolver,
            IApplicationAttributeMethodResolver<EventHandlerAttribute> eventsHandlerResolver,
            IApplicationAttributeMethodResolver<UserConnectionMethodCreatorAttribute> connectionMethodResolver
        )
        {
            _applicationActionResolver = applicationActionResolver;
            _eventsHandlerResolver = eventsHandlerResolver;
            _connectionMethodResolver = connectionMethodResolver;
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
                if(ResolveMethod<ActionDefinitionAttribute>(methodInfo, (attribute) => _applicationActionResolver.Resolve(platypusApplication, attribute, methodInfo) )) continue;
                if(ResolveMethod<EventHandlerAttribute>(methodInfo, (attribute) => _eventsHandlerResolver.Resolve(platypusApplication, attribute, methodInfo) )) continue;
                if(ResolveMethod<UserConnectionMethodCreatorAttribute>(methodInfo, (attribute) => _connectionMethodResolver.Resolve(platypusApplication, attribute, methodInfo))) continue;
            }
        }

        private bool ResolveMethod<AttributeType>(MethodInfo methodInfo, Action<AttributeType> consumer)
            where AttributeType : Attribute
        {
            AttributeType attribute = methodInfo.GetCustomAttribute<AttributeType>();
            if (attribute == null) return false;
            consumer(attribute);
            return true;
        }
    }
}
