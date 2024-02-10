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
            serviceGetters.Add(_serviceType, () => Construct(serviceGetters));
            return true;
        }
    }
}
