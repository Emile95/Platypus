namespace PlatypusContainer.Service.Resolver
{
    internal class SingletonFromProviderServiceResolver : IServiceResolver
    {
        private readonly Type _serviceType;
        private readonly Type _implementationType;
        private readonly IServiceProvider _serviceProvider;

        public SingletonFromProviderServiceResolver(
            Type serviceType,
            Type implmentationType,
            IServiceProvider serviceProvider
        )
        {
            _serviceType = serviceType;
            _implementationType = implmentationType;
            _serviceProvider = serviceProvider;
        }

        public bool Resolve(Dictionary<Type, Func<object>> serviceGetters)
        {
            if (serviceGetters.ContainsKey(_serviceType) ||
                serviceGetters.ContainsKey(_implementationType) == false) return false;
            serviceGetters.Add(_serviceType, () => _serviceProvider.GetService(_implementationType));
            return true;
        }
    }
}
