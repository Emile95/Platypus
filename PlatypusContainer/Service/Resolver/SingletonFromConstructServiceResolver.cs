namespace PlatypusContainer.Service.Resolver
{
    internal class SingletonFromConstructServiceResolver : FromConstructServiceResolver, IServiceResolver
    {
        private readonly Type _serviceType;
        private object _instance;

        public SingletonFromConstructServiceResolver(
            Type serviceType,
            Type implmentationType,
            object instance
        ) : base(implmentationType)
        {
            _serviceType = serviceType;
            _instance = instance;
        }

        public SingletonFromConstructServiceResolver(
            Type serviceType,
            Type implmentationType
        ) : this(serviceType, implmentationType, null) {}

        public bool Resolve(Dictionary<Type, Func<object>> serviceGetters)
        {
            if (serviceGetters.ContainsKey(_serviceType)) return false;
            if (_instance == null)
            {
                _instance = Construct(serviceGetters);
                if (_instance == null) return false;
            }
            serviceGetters.Add(_serviceType, () => _instance);
            return true;
        }
    }
}
