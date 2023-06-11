using Persistance.Entity;

namespace Persistance
{
    public class ApplicationRepository
    {
        public void SaveApplication(ApplicationEntity entity)
        {
            string newApplicationDirectoryPath = Path.Combine(ApplicationPaths.APPLICATIONSDIRECTORYPATHS, entity.Guid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationDllFilePath = Path.Combine(newApplicationDirectoryPath, ApplicationPaths.APPLICATIONDLLFILENAME);
            File.Copy(entity.DllFilePath, newApplicationDllFilePath, true);
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
    }
}
