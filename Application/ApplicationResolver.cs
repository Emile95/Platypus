using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using System.Reflection;

namespace Application
{
    internal class ApplicationResolver
    {
        private readonly ApplicationInstance _applicationInstance;

        public ApplicationResolver(ApplicationInstance applicationInstance)
        {
            _applicationInstance = applicationInstance;
        }

        public void ResolvePlatypusApplication(PlatypusApplicationBase platypusApplication)
        {
            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach(MethodInfo method in methods)
                ResolvePlatypusApplicationMethod(platypusApplication, method);
        }

        private void ResolvePlatypusApplicationMethod(PlatypusApplicationBase platypusApplication, MethodInfo methodInfo)
        {
            ResolveActionDefinition(platypusApplication, methodInfo);
        }

        private void ResolveActionDefinition(PlatypusApplicationBase platypusApplication, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return;

            _applicationInstance.AddAction(platypusApplication, actionDefinition, methodInfo);
        }
    }
}
