using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusUtils;
using Core.Exceptions;
using PlatypusRepository;
using System.IO.Compression;
using Core.Persistance.Entity;
using Core.Application.Abstract;
using PlatypusFramework.Core.Event;
using Core.Event.Abstract;
using Core.Application.Exception;
using Core.Ressource;
using Core.Persistance;
using System.IO;

namespace Core.Application
{
    public class ApplicationInstaller : IApplicationInstaller
    {
        private readonly IRepositoryAddOperator<ApplicationEntity> _applicationEntityAddOperator;
        private readonly IRepositoryAddOperator<PlatypusApplicationBase> _applicationAddOperator;
        private readonly IRepositoryAddOperator<ApplicationActionEntity> _applicationActionEntityAddOperator;
        private readonly IEventHandlerRunner _eventhHandlerRunner;

        public ApplicationInstaller(
            IRepositoryAddOperator<ApplicationEntity> applicationEntityAddOperator,
            IRepositoryAddOperator<PlatypusApplicationBase> applicationAddOperator,
            IRepositoryAddOperator<ApplicationActionEntity> applicationActionEntityAddOperator,
            IEventHandlerRunner eventhHandlerRunner
        )
        {
            _applicationEntityAddOperator = applicationEntityAddOperator;
            _applicationAddOperator = applicationAddOperator;
            _applicationActionEntityAddOperator = applicationActionEntityAddOperator;
            _eventhHandlerRunner = eventhHandlerRunner;
        }

        public void Install(string sourcePath)
        {
            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment();

            _eventhHandlerRunner.Run<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            FileInfo applicationFileInfo = new FileInfo(sourcePath);
            if (applicationFileInfo.Extension != ".platypus")
                throw new InvalidPlatypusApplicationPackageException(applicationFileInfo.Name, "wrong file extension");

            ApplicationEntity entity = new ApplicationEntity();

            string temporaryDirectoryPath = ExtractPackageAndLoadDll(entity, sourcePath);

            PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);

            _applicationEntityAddOperator.Add(entity);

            MoveTemporaryFolderContentToApplicationFolder(temporaryDirectoryPath, Path.Combine(ApplicationPaths.APPLICATIONSDIRECTORYPATHS, entity.Guid));

            platypusApplication.ApplicationGuid = entity.Guid;

            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
                if (InstallAction(entity.Guid, method)) continue;
            
            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationGuid = entity.Guid;

            platypusApplication.Install(env);

            if (platypusApplication == null) return;

            _applicationAddOperator.Add(platypusApplication);

            _eventhHandlerRunner.Run<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);
        }

        private bool InstallAction(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return false;

            _applicationActionEntityAddOperator.Add(new ApplicationActionEntity() { 
                Guid = actionDefinition.Name + applicationGuid
            });
            return true;
        }

        private void MoveTemporaryFolderContentToApplicationFolder(string temporaryDirectoryPath, string applicationDirectoryPath)
        {
            string[] directoriePaths = Directory.GetDirectories(temporaryDirectoryPath);

            foreach (string directoryPath in directoriePaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                Directory.Move(directoryPath, Path.Combine(applicationDirectoryPath, directoryInfo.Name));
            }
            
            string[] filePaths = Directory.GetFiles(temporaryDirectoryPath);
            foreach (string filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                File.Copy(filePath, Path.Combine(applicationDirectoryPath, fileInfo.Name));
            }
                
            Directory.Delete(temporaryDirectoryPath, true);
        }

        private string ExtractPackageAndLoadDll(ApplicationEntity entity, string sourceDirectoryPath)
        {
            string temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), "platyPusInstall");
            Directory.CreateDirectory(temporaryDirectoryPath);

            ZipFile.ExtractToDirectory(sourceDirectoryPath, temporaryDirectoryPath);

            string applicationDllFilePath = Path.Combine(temporaryDirectoryPath,"platypusApplication.dll");
            if (File.Exists(applicationDllFilePath) == false)
                throw new InstallApplicationException(Utils.GetString(Strings.ResourceManager, "NoDllFoundInPakcage", "platypusApplication.dll"));

            entity.AssemblyRaw = File.ReadAllBytes(Path.Combine(temporaryDirectoryPath, applicationDllFilePath));

            File.Delete(applicationDllFilePath);

            return temporaryDirectoryPath;
        }
    }
}
