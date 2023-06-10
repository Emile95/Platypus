using PlatypusApplicationFramework;

namespace Persistance
{
    public class ApplicationRepository
    {
        public string SaveApplication(PlatypusApplicationBase platypusApplication, string guid, string dllFilePath)
        {
            string newApplicationDirectoryPath = Path.Combine(ApplicationPaths.APPLICATIONSDIRECTORYPATHS, guid);
            Directory.CreateDirectory(newApplicationDirectoryPath);
            string newApplicationDllFilePath = Path.Combine(newApplicationDirectoryPath, ApplicationPaths.APPLICATIONDLLFILENAME);
            File.Copy(dllFilePath, newApplicationDllFilePath, true);
            return newApplicationDllFilePath;
        }
    }
}
