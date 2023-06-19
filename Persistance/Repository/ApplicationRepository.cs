using Persistance.Entity;

namespace Persistance.Repository
{
    public class ApplicationRepository
    {
        public string SaveApplication(ApplicationEntity entity)
        {
            string newApplicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(entity.Guid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationDllFilePath = ApplicationPaths.GetApplicationDllFilePathByBasePath(newApplicationDirectoryPath); ;
            File.Copy(entity.DllFilePath, newApplicationDllFilePath, true);
            return newApplicationDllFilePath;
        }

        public void RemoveApplication(string applicationGuid)
        {
            string applicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(applicationGuid);
            Directory.Delete(applicationDirectoryPath, true);
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
