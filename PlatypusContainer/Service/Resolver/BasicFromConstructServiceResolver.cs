namespace PlatypusContainer.Service.Resolver
{
    internal class BasicFromConstructServiceResolver : FromConstructServiceResolver, IServiceResolver
    {
        private readonly Type _serviceType;

        public BasicFromConstructServiceResolver(
            Type serviceType,
            Type implmentationType
        ) : base(implmentationType)
        {
            _serviceType = serviceType;
        }

        public bool Resolve(Dictionary<Type, Func<object>> serviceGetters)
        {
            if (serviceGetters.ContainsKey(_serviceType)) return false;
            Func<object> constructor = FindConstructor(serviceGetters);
            if(constructor == null) return false;
            serviceGetters.Add(_serviceType, constructor);
            return true;
        }
    }
}
