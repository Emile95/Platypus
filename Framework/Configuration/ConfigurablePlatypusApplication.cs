namespace PlatypusApplicationFramework.Configuration
{
    public abstract class ConfigurablePlatypusApplication<ConfigurationType> : PlatypusApplicationBase
        where ConfigurationType : class, new()
    {
        protected ConfigurationType Configuration { get; set; }

        public ConfigurablePlatypusApplication()
        {
            Configuration = new ConfigurationType();
        }

        public abstract bool ValidateConfiguration(ConfigurationType configuration);
        protected abstract void OnConfigurationUpdate(ConfigurationType previousConfiguration);
        
    }
}
