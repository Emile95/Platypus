namespace PlatypusFramework.Core.ApplicationAction
{
    public class ApplicationActionEnvironment<ParameterType> : ApplicationActionEnvironmentBase
    {
        public ParameterType Parameter { get; set; }
    }
}
