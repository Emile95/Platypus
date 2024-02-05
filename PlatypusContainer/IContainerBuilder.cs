using PlatypusContainer.Service;

namespace PlatypusContainer
{
    public interface IContainerBuilder
    {
        IContainer Build();
        void ConfigureServices(Action<IServiceCollection> servicesConsumer);
    }
}
