using PlatypusContainer.Service.Resolver;

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

        public void Add<ServiceType>() 
            where ServiceType : class
        {
            Type type = AssertServiceType(typeof(ServiceType));
            _containerBuilder.ServiceResolvers.Add(new BasicFromConstructServiceResolver(type, type));
        }

        public void Add<ServiceType, ServiceImplementation>()
            where ServiceType : class
            where ServiceImplementation : class, ServiceType
        {
            _containerBuilder.ServiceResolvers.Add(
                new BasicFromConstructServiceResolver(
                    AssertServiceType(typeof(ServiceType)),
                    typeof(ServiceImplementation)
                )
            );
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
            Type type = AssertServiceType(serviceType);
            _containerBuilder.ServiceResolvers.Add(
                new SingletonFromConstructServiceResolver(type,type)
            );
        }

        public void AddSingleton<ServiceType>(ServiceType instance) where ServiceType : class
        {
            AddSingleton(typeof(ServiceType), instance);
        }

        public void AddSingleton(Type serviceType, object instance)
        {
            _containerBuilder.ServiceResolvers.Add(
                new SingletonServiceResolver(
                    AssertServiceType(serviceType),
                    instance
                )
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
            _containerBuilder.ServiceResolvers.Add(
                new SingletonFromConstructServiceResolver(
                    AssertServiceType(serviceType),
                    typeof(ServiceImplementation)
                )
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
            _containerBuilder.ServiceResolvers.Add(
                new SingletonServiceResolver(AssertServiceType(serviceType), instance)
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
            _containerBuilder.ServiceResolvers.Add(
                new SingletonFromProviderServiceResolver(
                    AssertServiceType(serviceType),
                    typeof(ServiceImplementation),
                    _containerBuilder.ServiceProvider
                )
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
