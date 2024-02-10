using PlatypusContainer.Service.Resolver;

namespace PlatypusContainer.Service
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Func<object>> _serviceGetters;

        public ServiceProvider()
        {
            _serviceGetters = new Dictionary<Type, Func<object>>();
        }

        internal void Initialize(ContainerBuilder containerBuilder)
        {
            int totalSingletonService = containerBuilder.ServiceResolvers.Count;

            int serviceInstanciated = 0;

            while(serviceInstanciated < totalSingletonService)
            {
                foreach(IServiceResolver resolver in containerBuilder.ServiceResolvers)
                {
                    bool resolved = resolver.Resolve(_serviceGetters);
                    if(resolved) serviceInstanciated++;
                }
            }
        }

        public ServiceType GetService<ServiceType>() 
            where ServiceType : class
        {
            return _serviceGetters[typeof(ServiceType)]() as ServiceType;
        }

        public object GetService(Type serviceType)
        {
            return _serviceGetters[serviceType]();
        }
    }
}
