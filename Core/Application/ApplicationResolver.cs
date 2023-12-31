﻿using Core.ApplicationAction;
using Persistance.Repository;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Core.Event;
using PlatypusApplicationFramework.Configuration.Event;
using PlatypusApplicationFramework.Configuration.User;
using Core.User;

namespace Core.Application
{
    public class ApplicationResolver
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandler;
        private readonly UsersHandler _usersHandler;

        public ApplicationResolver(
            ApplicationRepository applicationRepository,
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
            env.ApplicationRepository = _applicationRepository;
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
