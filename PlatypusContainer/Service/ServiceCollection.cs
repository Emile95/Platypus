namespace PlatypusContainer.Service
{
    internal class ServiceCollection : IServiceCollection
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly HashSet<Type> _serviceRegistereds;

        internal ServiceCollection(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
            _serviceRegistereds = new HashSet<Type>();
        }

        public void AddHostedService<HostedServiceType>() 
            where HostedServiceType : IHostedService
        {
            _containerBuilder.HostedService = typeof(HostedServiceType);
        }

        public void AddSingleton<ServiceType>() 
            where ServiceType : class
        {
            AddSingleton(typeof(ServiceType));
        }

        public void AddSingleton(Type serviceType)
        {
            _containerBuilder.NotAbstractSingletonServiceTypes.Add(
                AssertServiceType(serviceType)
            );
        }

        public void AddSingleton<ServiceType>(ServiceType instance) where ServiceType : class
        {
            AddSingleton(typeof(ServiceType), instance);
        }

        public void AddSingleton(Type serviceType, object instance)
        {
            _containerBuilder.NotAbstractSingletonServiceTypeInstances.Add(
                 AssertServiceType(serviceType), 
                instance
            );
        }

        public void AddSingleton<ServiceType, ServiceImplementation>()
            where ServiceImplementation : class, ServiceType 
        {
            AddSingleton<ServiceImplementation>(typeof(ServiceType));
        }

        public void AddSingleton<ServiceImplementation>(Type serviceType) 
            where ServiceImplementation : class
        {
            _containerBuilder.SingletonServiceTypes.Add(
                AssertServiceType(serviceType),
                typeof(ServiceImplementation)
            );
        }

        public void AddSingleton<ServiceType, ServiceImplementation>(ServiceImplementation instance) 
            where ServiceImplementation : class, ServiceType
        {
            AddSingleton<ServiceImplementation>(typeof(ServiceType));
        }

        public void AddSingleton<ServiceImplementation>(Type serviceType, ServiceImplementation instance) 
            where ServiceImplementation : class
        {
            _containerBuilder.SingletonServiceInstances.Add(
                AssertServiceType(serviceType),
                instance
            );
        }

        public void AddSingleton<ServiceType, ServiceImplementation>(Func<IServiceProvider, ServiceImplementation> getter) 
            where ServiceImplementation : class, ServiceType
        {
            AddSingleton(typeof(ServiceType), getter);
        }

        public void AddSingleton<ServiceImplementation>(Type serviceType, Func<IServiceProvider, ServiceImplementation> getter) 
            where ServiceImplementation : class
        {
            _containerBuilder.SingletonServicesGetFromProvider.Add(
                AssertServiceType(serviceType),
                (typeof(ServiceImplementation),(provider) => getter(provider))
            );
        }

        private Type AssertServiceType(Type serviceType)
        {
            if (_serviceRegistereds.Contains(serviceType))
                throw new Exception($"service of type {serviceType.Name} is already registered");

            _serviceRegistereds.Add(serviceType);

            return serviceType;
        }
    }
}
