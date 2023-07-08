using Newtonsoft.Json;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using PlatypusApplicationFramework.Confugration;
using Logging;
using Common.Logger.FileLogger;
using PlatypusApplicationFramework.Core.Application.Logger;

namespace PlatypusApplicationFramework.Configuration.Application
{
    public abstract class ConfigurablePlatypusApplication<ConfigurationType> : PlatypusApplicationBase
        where ConfigurationType : class, new()
    {
        protected ConfigurationType Configuration { get; set; }
        protected LoggerBase Logger { get; set; }

        public ConfigurablePlatypusApplication()
        {
            Configuration = new ConfigurationType();
        }

        public override void Install(ApplicationInstallEnvironment env)
        {
            Configuration = ParameterEditorObjectResolver.CreateDefaultObject<ConfigurationType>();
            env.ApplicationRepository.SaveApplicationConfiguration(env.ApplicationGuid, GetConfigurationJsonObject());
        }

        public override void Initialize(ApplicationInitializeEnvironment env)
        {
            string jsonObject = env.ApplicationRepository.GetConfigurationJsonObject(env.ApplicationGuid);
            LoadConfigurationJsonObject(jsonObject);
        }

        public string GetConfigurationJsonObject()
        {
            return JsonConvert.SerializeObject(Configuration);
        }

        public void LoadConfigurationJsonObject(string jsonObject)
        {
            Configuration = JsonConvert.DeserializeObject<ConfigurationType>(jsonObject);

            if(Configuration is IConfigurationWithLogger)
            {
                IConfigurationWithLogger configurationWithLogger = Configuration as IConfigurationWithLogger;
                PlatypusApplicationLoggerConfiguration loggerConfiguration = configurationWithLogger.GetLogger();
                if(loggerConfiguration != null)
                    Logger = CreateLoggerWithConfiguration(loggerConfiguration);
            }
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

        private LoggerBase CreateLoggerWithConfiguration(PlatypusApplicationLoggerConfiguration loggerConfiguration)
        {
            LoggerManager logger = new LoggerManager();
            
            if(loggerConfiguration.FileLoggers != null || loggerConfiguration.FileLoggers.Count > 0)
            {
                foreach(var fileLoggerConfiguration in loggerConfiguration.FileLoggers)
                    logger.CreateLogger<PlatypusApplicationFileLogger>(
                        fileLoggerConfiguration.DirectoryPath,
                        fileLoggerConfiguration.FileName,
                        fileLoggerConfiguration.FileRotation.MaxSize,
                        fileLoggerConfiguration.Format,
                        fileLoggerConfiguration.MinimumLoggingLevel,
                        fileLoggerConfiguration.MinimumLoggingLevel
                    );
            }

            return logger;
        }
    }
}
