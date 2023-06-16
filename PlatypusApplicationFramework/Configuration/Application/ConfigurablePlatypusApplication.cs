using Newtonsoft.Json;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;

namespace PlatypusApplicationFramework.Configuration.Application
{
    public abstract class ConfigurablePlatypusApplication<ConfigurationType> : PlatypusApplicationBase
        where ConfigurationType : class, new()
    {
        protected ConfigurationType Configuration { get; set; }

        public ConfigurablePlatypusApplication()
        {
            Configuration = new ConfigurationType();
        }

        public override void Install(ApplicationInstallEnvironment env)
        {
            InitializeDefaultConfiguration();
            env.ApplicationRepository.SaveApplicationConfiguration(env.ApplicationGuid, GetConfigurationJsonObject());
        }

        public override void Initialize(ApplicationInitializeEnvironment env)
        {
            string jsonObject = env.ApplicationRepository.GetConfigurationJsonObject(env.ApplicationGuid);
            Configuration = JsonConvert.DeserializeObject<ConfigurationType>(jsonObject);
        }

        public string GetConfigurationJsonObject()
        {
            return JsonConvert.SerializeObject(Configuration);
        }

        public void LoadConfigurationJsonObject(string jsonObject)
        {
            Configuration = JsonConvert.DeserializeObject<ConfigurationType>(jsonObject);
        }

        [ActionDefinition(
            Name = "UpdateConfiguration",
            ParameterRequired = true)]
        public object UpdateConfiguration(ApplicationActionEnvironment<ConfigurationType> env)
        {
            ConfigurationType previousConfig = Configuration;

            if(ValidateConfiguration(env.Parameter))
                Configuration = env.Parameter;

            string jsonObject = JsonConvert.SerializeObject(Configuration);
            env.ApplicationRepository.SaveApplicationConfigurationByBasePath(ApplicationDirectoryPath, jsonObject);

            OnConfigurationUpdate(previousConfig);

            return null;
        }

        protected virtual bool ValidateConfiguration(ConfigurationType configuration) { return true; }
        protected virtual void OnConfigurationUpdate(ConfigurationType previousConfiguration) { }

        private void InitializeDefaultConfiguration()
        {
            ConfigurationType defaultConfig = new ConfigurationType();

            Type typeOfConfig = typeof(ConfigurationType);

            PropertyInfo[] propertyInfos = typeOfConfig.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ParameterEditorAttribute parameterEditor = propertyInfo.GetCustomAttribute<ParameterEditorAttribute>();
                if (parameterEditor == null) continue;

                if (parameterEditor.DefaultValue != null)
                    propertyInfo.SetValue(defaultConfig, parameterEditor.DefaultValue);
            }

            Configuration = defaultConfig;
        }

    }
}
