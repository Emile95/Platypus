using Core.ApplicationAction;
using Persistance;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;

namespace Core.Application
{
    public class ApplicationResolver
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public ApplicationResolver(
            ApplicationRepository applicationRepository,
            ApplicationActionsHandler applicationActionsHandler
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionsHandler = applicationActionsHandler;
        }

        public void ResolvePlatypusApplication(PlatypusApplicationBase platypusApplication, string applicationGuid)
        {
            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach(MethodInfo method in methods)
                ResolvePlatypusApplicationMethod(platypusApplication, applicationGuid, method);

            ApplicationInitializeEnvironment env = new ApplicationInitializeEnvironment();
            env.ApplicationRepository = _applicationRepository;
            env.ApplicationGuid = applicationGuid;

            platypusApplication.Initialize(env);
        }

        private void ResolvePlatypusApplicationMethod(PlatypusApplicationBase platypusApplication, string applicationGuid, MethodInfo methodInfo)
        {
            ResolveActionDefinition(platypusApplication, applicationGuid, methodInfo);
        }

        private void ResolveActionDefinition(PlatypusApplicationBase platypusApplication, string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return;

            _applicationActionsHandler.AddAction(platypusApplication, applicationGuid, actionDefinition, methodInfo);
        }
    }
}
