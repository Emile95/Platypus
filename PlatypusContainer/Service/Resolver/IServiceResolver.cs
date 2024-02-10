namespace PlatypusContainer.Service.Resolver
{
    internal interface IServiceResolver
    {
        bool Resolve(Dictionary<Type, Func<object>> serviceGetters);
    }
}
