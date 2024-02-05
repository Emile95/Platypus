namespace PlatypusContainer.Service
{
    public interface IServiceProvider
    {
        ServiceType GetService<ServiceType>()
            where ServiceType : class;

        object GetService(Type serviceType);
    }
}
