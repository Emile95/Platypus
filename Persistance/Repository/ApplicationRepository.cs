using Persistance.Entity;

namespace Persistance.Repository
{
    public class ApplicationRepository
    {
        public string SaveApplication(ApplicationEntity entity)
        {
            string newApplicationDirectoryPath = Path.Combine(ApplicationPaths.APPLICATIONSDIRECTORYPATHS, entity.Guid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationDllFilePath = Path.Combine(newApplicationDirectoryPath, ApplicationPaths.APPLICATIONDLLFILENAME);
            File.Copy(entity.DllFilePath, newApplicationDllFilePath, true);
            return newApplicationDllFilePath;
        }

        public List<ApplicationEntity> LoadApplications()
        {
            List<ApplicationEntity> applicationEntities = new List<ApplicationEntity>();
            string[] applicationDirectoriesPath = Directory.GetDirectories(ApplicationPaths.APPLICATIONSDIRECTORYPATHS);
            foreach (string applicationDirectoryPath in applicationDirectoriesPath)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(applicationDirectoryPath);
                string dllFilePath = ApplicationPaths.GetApplicationDllFilePath(directoryInfo.Name);
                applicationEntities.Add(new ApplicationEntity()
                {
                    Guid = directoryInfo.Name,
                    DllFilePath = dllFilePath,
                });
            }

            return applicationEntities;
        }

        public void SaveApplicationConfiguration(string applicationGuid, string configuration)
        {
            string configFilePath = ApplicationPaths.GetApplicationConfigFilePath(applicationGuid);
            File.WriteAllText(configFilePath, configuration); 
        }

        public void SaveApplicationConfigurationByBasePath(string basePath, string configuration)
        {
            string configFilePath = ApplicationPaths.GetApplicationConfigFilePathByBasePath(basePath);
            File.WriteAllText(configFilePath, configuration);
        }

        public string GetConfigurationJsonObject(string applicationGuid)
        {
            string configFilePath = ApplicationPaths.GetApplicationConfigFilePath(applicationGuid);
            return File.ReadAllText(configFilePath);
        }
    }
}
