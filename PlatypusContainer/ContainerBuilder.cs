using PlatypusContainer.Service;
using System.Reflection;

namespace PlatypusContainer
{
    public class ContainerBuilder : IContainerBuilder
    {
        internal Type HostedService { get; set; }
        internal List<Type> NotAbstractSingletonServiceTypes { get; set; }
        internal Dictionary<Type, object> NotAbstractSingletonServiceTypeInstances { get; set; }
        internal Dictionary<Type, Type> SingletonServiceTypes { get; set; }
        internal Dictionary<Type, object> SingletonServiceInstances { get; set; }
        internal Dictionary<Type, (Type,Func<Service.IServiceProvider, object>)> SingletonServicesGetFromProvider { get; set; }

        public IServiceCollection _services;

        public ContainerBuilder()
        {
            NotAbstractSingletonServiceTypes = new List<Type>();
            NotAbstractSingletonServiceTypeInstances = new Dictionary<Type, object>();
            SingletonServiceTypes = new Dictionary<Type, Type>();
            SingletonServiceInstances = new Dictionary<Type, object>();
            SingletonServicesGetFromProvider = new Dictionary<Type, (Type,Func<Service.IServiceProvider, object>)>();

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
            Service.IServiceProvider serviceProvider = new ServiceProvider(this);

            ConstructorInfo[] constructorInfos = HostedService.GetConstructors();

            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                ParameterInfo[] parametersInfos = constructorInfo.GetParameters();
                int nbParameterCorrespondingToRegisteredServices = 0;

                List<object> serviceParameters = new List<object>();

                foreach (ParameterInfo parametersInfo in parametersInfos)
                {
                    serviceParameters.Add(serviceProvider.GetService(parametersInfo.ParameterType));
                    nbParameterCorrespondingToRegisteredServices++;
                }
                if (nbParameterCorrespondingToRegisteredServices == parametersInfos.Length)
                    return constructorInfo.Invoke(serviceParameters.ToArray()) as IHostedService;
            }

            return null;
        }
    }
}
