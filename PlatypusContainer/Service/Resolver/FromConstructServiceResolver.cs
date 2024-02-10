using System.Reflection;

namespace PlatypusContainer.Service.Resolver
{
    internal abstract class FromConstructServiceResolver
    {
        protected readonly Type _implmentationType;

        internal FromConstructServiceResolver(Type implmentationType)
        {
            _implmentationType = implmentationType;
        }

        protected Func<object> FindConstructor(Dictionary<Type, Func<object>> serviceGetters)
        {
            ConstructorInfo[] constructorInfos = _implmentationType.GetConstructors();

            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                ParameterInfo[] parametersInfos = constructorInfo.GetParameters();
                int nbParameterCorrespondingToRegisteredServices = 0;

                List<object> serviceParameters = new List<object>();

                foreach (ParameterInfo parametersInfo in parametersInfos)
                {
                    if (serviceGetters.ContainsKey(parametersInfo.ParameterType) == false) break;
                    serviceParameters.Add(serviceGetters[parametersInfo.ParameterType]());
                    nbParameterCorrespondingToRegisteredServices++;
                }
                if (nbParameterCorrespondingToRegisteredServices == parametersInfos.Length)
                    return () => constructorInfo.Invoke(serviceParameters.ToArray());
            }

            return null;
        }
    }
}
