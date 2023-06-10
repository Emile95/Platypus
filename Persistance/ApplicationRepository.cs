using PlatypusApplicationFramework;
using Utils;

namespace Persistance
{
    public class ApplicationRepository
    {
        public void SaveApplication(string guid, string dllFilePath)
        {
            string newApplicationDirectoryPath = Path.Combine(ApplicationPaths.APPLICATIONSDIRECTORYPATHS, guid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationDllFilePath = Path.Combine(newApplicationDirectoryPath, ApplicationPaths.APPLICATIONDLLFILENAME);
            File.Copy(dllFilePath, newApplicationDllFilePath, true);
        }

        public List<Tuple<PlatypusApplicationBase,string>> LoadApplications()
        {
            List<Tuple<PlatypusApplicationBase, string>> applications = new List<Tuple<PlatypusApplicationBase, string>>();
            string[] applicationDirectoriesPath = Directory.GetDirectories(ApplicationPaths.APPLICATIONSDIRECTORYPATHS);
            foreach (string applicationDirectoryPath in applicationDirectoriesPath)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(applicationDirectoryPath);
                string dllFilePath = ApplicationPaths.GetApplicationDllFilePath(directoryInfo.Name);
                PlatypusApplicationBase applicationFromDll = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(dllFilePath);
                applications.Add(new Tuple<PlatypusApplicationBase, string>(applicationFromDll, directoryInfo.Name));
            }

            return applications;
        }
    }
}
