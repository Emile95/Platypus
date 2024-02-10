namespace PlatypusContainer.Service.Resolver
{
    internal class SingletonServiceResolver : IServiceResolver
    {
        private readonly Type _serviceType;
        private readonly object _instance;

        public SingletonServiceResolver(
            Type serviceType,
            object instance
        )
        {
            _serviceType = serviceType;
            _instance = instance;
        }

        public bool Resolve(Dictionary<Type, Func<object>> serviceGetters)
        {
            if (serviceGetters.ContainsKey(_serviceType)) return false;
            serviceGetters.Add(_serviceType, () => _instance);
            return true;
        }
    }
}
