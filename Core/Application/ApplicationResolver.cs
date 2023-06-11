using Core.ApplicationAction;
using Persistance;
using PlatypusApplicationFramework.Application;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;

namespace Core.Application
{
    public class ApplicationResolver
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly ApplicationRepository _applicationRepository;

        public ApplicationResolver(
            ApplicationActionsHandler applicationActionsHandler,
            ApplicationRepository applicationRepository
        )
        {
            _applicationActionsHandler = applicationActionsHandler;
            _applicationRepository = applicationRepository;
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
