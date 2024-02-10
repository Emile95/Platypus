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
            Func<object> constructor = FindConstructor(serviceGetters);
            if (constructor == null) return false;
            _instance = constructor();
            serviceGetters.Add(_serviceType, () => _instance);
            return true;
        }
    }
}
