namespace PlatypusContainer.Service
{
    public interface IServiceCollection
    {
        void AddHostedService<HostedServiceType>()
            where HostedServiceType : IHostedService;

        void Add<ServiceType, ServiceImplementation>()
            where ServiceType : class
            where ServiceImplementation : class, ServiceType;
        void Add<ServiceType>()
            where ServiceType : class;

        void AddSingleton<ServiceType>()
            where ServiceType : class;
        void AddSingleton<ServiceType>(ServiceType instance)
            where ServiceType : class;
        void AddSingleton(Type serviceType);
        void AddSingleton(Type serviceType, object instance);

        void AddSingleton<ServiceType, ServiceImplementation>()
            where ServiceImplementation : class, ServiceType;
        void AddSingleton<ServiceImplementation>(Type serviceType)
            where ServiceImplementation : class;

        void AddSingleton<ServiceType, ServiceImplementation>(ServiceImplementation instance)
            where ServiceImplementation : class, ServiceType;
        void AddSingleton<ServiceImplementation>(Type serviceType, ServiceImplementation instance)
            where ServiceImplementation : class;

        void AddSingleton<ServiceType, ServiceImplementation>(Func<IServiceProvider, ServiceImplementation> getter)
            where ServiceImplementation : class, ServiceType;
        void AddSingleton<ServiceImplementation>(Type serviceType, Func<IServiceProvider, ServiceImplementation> getter)
            where ServiceImplementation : class;
    }
}
