using Microsoft.Extensions.DependencyInjection;

namespace EntryPoint
{
    internal static class DepencenyInjectionHelper
    {
        internal static void InjectAllInterfacesFromType<ClassType>(IServiceCollection services)
            where ClassType : class
        {
            Type[] interfaces = typeof(ClassType).GetInterfaces();

            if (interfaces == null || interfaces.Length == 0) return;

            services.AddSingleton<ClassType>();
            foreach (Type i in interfaces)
                services.AddSingleton(i, provider => provider.GetRequiredService<ClassType>());
        }

        internal static void InjectAllInterfacesFromType<ClassType>(IServiceCollection services, ClassType instance)
            where ClassType : class
        {
            Type[] interfaces = typeof(ClassType).GetInterfaces();

            if (interfaces == null || interfaces.Length == 0) return;

            services.AddSingleton<ClassType>();
            foreach (Type i in interfaces)
                services.AddSingleton(i, instance);
        }
    }
}
