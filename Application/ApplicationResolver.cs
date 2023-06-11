using Application.ApplicationAction;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;

namespace Application
{
    public class ApplicationResolver
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public ApplicationResolver(ApplicationActionsHandler applicationActionsHandler)
        {
            _applicationActionsHandler = applicationActionsHandler;
        }

        public void ResolvePlatypusApplication(PlatypusApplicationBase platypusApplication, string applicationGuid)
        {
            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach(MethodInfo method in methods)
                ResolvePlatypusApplicationMethod(platypusApplication, applicationGuid, method);
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
