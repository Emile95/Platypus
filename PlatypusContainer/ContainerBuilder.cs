using PlatypusContainer.Service;
using PlatypusContainer.Service.Resolver;
using System.Reflection;

namespace PlatypusContainer
{
    public class ContainerBuilder : IContainerBuilder
    {
        internal Type HostedService { get; set; }
        internal List<IServiceResolver> ServiceResolvers { get; set; }
        internal Service.ServiceProvider ServiceProvider { get; }
        private readonly IServiceCollection _services;
        

        public ContainerBuilder()
        {
            ServiceResolvers = new List<IServiceResolver>();
            ServiceProvider = new ServiceProvider();
            _services = new ServiceCollection(this);
        }

        public IContainer Build()
        {
            IHostedService hostedService = InstanciateHostedService();
            return new Container(hostedService);
        }

        public void ConfigureServices(Action<IServiceCollection> servicesConsumer)
        {
            servicesConsumer(_services);
        }

        private IHostedService InstanciateHostedService()
        {
            ServiceProvider.Initialize(this);

            ConstructorInfo[] constructorInfos = HostedService.GetConstructors();

            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                ParameterInfo[] parametersInfos = constructorInfo.GetParameters();
                int nbParameterCorrespondingToRegisteredServices = 0;

                List<object> serviceParameters = new List<object>();

                foreach (ParameterInfo parametersInfo in parametersInfos)
                {
                    serviceParameters.Add(ServiceProvider.GetService(parametersInfo.ParameterType));
                    nbParameterCorrespondingToRegisteredServices++;
                }
                if (nbParameterCorrespondingToRegisteredServices == parametersInfos.Length)
                    return constructorInfo.Invoke(serviceParameters.ToArray()) as IHostedService;
            }

            return null;
        }
    }
}
