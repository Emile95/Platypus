using Persistance.Entity;
using System.IO.Compression;

namespace Persistance.Repository
{
    public class ApplicationRepository
    {
        public string SaveApplication(ApplicationEntity entity, string compressedFilePath)
        {
            string newApplicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(entity.Guid);
            Directory.CreateDirectory(newApplicationDirectoryPath);

            string temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), entity.Guid);
            Directory.CreateDirectory(temporaryDirectoryPath);

            ZipFile.ExtractToDirectory(compressedFilePath, temporaryDirectoryPath);
            string[] dllFiles = Directory.GetFiles(temporaryDirectoryPath, "*.dll");
            if (dllFiles.Length == 0) return null;

            string[] directoriesPath = Directory.GetDirectories(temporaryDirectoryPath);
            if(directoriesPath.Length > 0)
                foreach(string directoryPath in directoriesPath)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    Directory.Move(directoryPath, Path.Combine(newApplicationDirectoryPath, directoryInfo.Name));
                }

            string newApplicationDllFilePath = ApplicationPaths.GetApplicationDllFilePathByBasePath(newApplicationDirectoryPath);
            File.Copy(dllFiles[0], newApplicationDllFilePath, true);

            Directory.Delete(temporaryDirectoryPath, true);

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

            if (Directory.Exists(ApplicationPaths.APPLICATIONSDIRECTORYPATHS) == false) return applicationEntities;

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
