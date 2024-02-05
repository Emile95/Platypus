using System.Reflection;

namespace PlatypusContainer.Service
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _singletonServices;

        public ServiceProvider(ContainerBuilder containerBuilder)
        {
            _singletonServices = new Dictionary<Type, object>();

            int totalSingletonService =
                containerBuilder.SingletonServiceTypes.Count +
                containerBuilder.SingletonServiceInstances.Count +
                containerBuilder.NotAbstractSingletonServiceTypeInstances.Count +
                containerBuilder.NotAbstractSingletonServiceTypes.Count +
                containerBuilder.SingletonServicesGetFromProvider.Count;

            int serviceInstanciated = 0;

            foreach (KeyValuePair<Type, object> singletonServiceInstance in containerBuilder.SingletonServiceInstances)
            {
                _singletonServices.Add(singletonServiceInstance.Key, singletonServiceInstance.Value);
                serviceInstanciated++;
            }

            foreach(KeyValuePair<Type, object> notAbstractSingletonServiceTypeInstance in containerBuilder.NotAbstractSingletonServiceTypeInstances)
            {
                _singletonServices.Add(notAbstractSingletonServiceTypeInstance.Key, notAbstractSingletonServiceTypeInstance.Value);
                serviceInstanciated++;
            }

            while (serviceInstanciated < totalSingletonService)
            {
                foreach(KeyValuePair<Type, (Type, Func<IServiceProvider, object>)> singletonServiceGetFromProvider in containerBuilder.SingletonServicesGetFromProvider)
                {
                    if (_singletonServices.ContainsKey(singletonServiceGetFromProvider.Key)) continue;
                    if (_singletonServices.ContainsKey(singletonServiceGetFromProvider.Value.Item1) == false) continue;
                    _singletonServices.Add(singletonServiceGetFromProvider.Key, singletonServiceGetFromProvider.Value.Item2(this));
                    serviceInstanciated++;
                }

                foreach (Type notAbstractSingletonServiceType in containerBuilder.NotAbstractSingletonServiceTypes)
                {
                    if (_singletonServices.ContainsKey(notAbstractSingletonServiceType)) continue;

                    Type instanceType = notAbstractSingletonServiceType;
                    ConstructorInfo[] constructorInfos = instanceType.GetConstructors();

                    bool validConstructorFound = false;
                    foreach (ConstructorInfo constructorInfo in constructorInfos)
                    {
                        ParameterInfo[] parametersInfos = constructorInfo.GetParameters();
                        int nbParameterCorrespondingToRegisteredServices = 0;

                        List<object> serviceParameters = new List<object>();

                        foreach (ParameterInfo parametersInfo in parametersInfos)
                        {
                            if (_singletonServices.ContainsKey(parametersInfo.ParameterType) == false) break;
                            serviceParameters.Add(_singletonServices[parametersInfo.ParameterType]);
                            nbParameterCorrespondingToRegisteredServices++;
                        }
                        if (nbParameterCorrespondingToRegisteredServices == parametersInfos.Length)
                        {
                            _singletonServices.Add(notAbstractSingletonServiceType, constructorInfo.Invoke(serviceParameters.ToArray()));
                            validConstructorFound = true;
                            break;
                        }
                    }
                    if (validConstructorFound == false) continue;
                    serviceInstanciated++;
                }

                foreach (KeyValuePair<Type, Type> singletonServiceType in containerBuilder.SingletonServiceTypes)
                {
                    if (_singletonServices.ContainsKey(singletonServiceType.Key)) continue;

                    Type instanceType = singletonServiceType.Key;
                    ConstructorInfo[] constructorInfos = instanceType.GetConstructors();

                    bool validConstructorFound = false;
                    foreach(ConstructorInfo constructorInfo in constructorInfos)
                    {
                        ParameterInfo[] parametersInfos = constructorInfo.GetParameters();
                        int nbParameterCorrespondingToRegisteredServices = 0;

                        List<object> serviceParameters = new List<object>();

                        foreach(ParameterInfo parametersInfo in parametersInfos)
                        {
                            if(_singletonServices.ContainsKey(parametersInfo.ParameterType) == false) break;
                            serviceParameters.Add(_singletonServices[parametersInfo.ParameterType]);
                            nbParameterCorrespondingToRegisteredServices++;
                        }
                        if (nbParameterCorrespondingToRegisteredServices == parametersInfos.Length)
                        {
                            _singletonServices.Add(singletonServiceType.Key, constructorInfo.Invoke(serviceParameters.ToArray()));
                            validConstructorFound = true;
                            break;
                        }
                    }
                    if (validConstructorFound == false) continue;
                    serviceInstanciated++;
                }
            }
        }

        public ServiceType GetService<ServiceType>() 
            where ServiceType : class
        {
            return _singletonServices[typeof(ServiceType)] as ServiceType;
        }

        public object GetService(Type serviceType)
        {
            return _singletonServices[serviceType];
        }
    }
}
