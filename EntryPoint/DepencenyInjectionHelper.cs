using PlatypusContainer.Service;

namespace EntryPoint
{
    internal static class DepencenyInjectionHelper
    {
        internal static void AddServiceInterfacesSharedReference<ClassType>(IServiceCollection services)
            where ClassType : class
        {
            services.AddSingleton<ClassType>();
            AddServiceInterfacesSharedReference<ClassType>(services, (i) => services.AddSingleton(i, provider => provider.GetService<ClassType>()));
        }

        internal static void AddServiceInterfacesSharedReference<ClassType>(IServiceCollection services, ClassType instance)
            where ClassType : class
        {
            AddServiceInterfacesSharedReference<ClassType>(services ,(i) => services.AddSingleton(i, instance));
        }

        private static void AddServiceInterfacesSharedReference<ClassType>(IServiceCollection services, Action<Type> interfaceConsumer)
            where ClassType : class
        {
            Type[] interfaces = typeof(ClassType).GetInterfaces();

            if (interfaces == null || interfaces.Length == 0) return;

            foreach (Type i in interfaces)
                interfaceConsumer(i);
        }
    }
}
