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

        public PlatypusApplicationBase Install(string sourcePath)
        {
            InstallApplicationEventHandlerEnvironment eventEnv = new InstallApplicationEventHandlerEnvironment();

            _eventhHandlerRunner.Run<object>(EventHandlerType.BeforeInstallApplication, eventEnv, (exception) => throw exception);

            FileInfo applicationFileInfo = new FileInfo(sourcePath);
            if (applicationFileInfo.Extension != ".platypus")
                throw new InvalidPlatypusApplicationPackageException(applicationFileInfo.Name, "wrong file extension");

            ApplicationEntity entity = new ApplicationEntity();

            ExtractPackage(entity, sourcePath);

            PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);

            _applicationEntityAddOperator.Add(entity);

            platypusApplication.ApplicationGuid = entity.Guid;

            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (InstallAction(entity.Guid, method)) continue;
            }

            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationGuid = entity.Guid;

                platypusApplication.Install(env);

            if (platypusApplication == null) return null;

            _applicationAddOperator.Add(platypusApplication);

            _eventhHandlerRunner.Run<object>(EventHandlerType.AfterInstallApplication, eventEnv, (exception) => throw exception);

            return platypusApplication;
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

        private void ExtractPackage(ApplicationEntity entity, string sourceDirectoryPath)
        {
            string temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), "platyPusInstall");
            Directory.CreateDirectory(temporaryDirectoryPath);

            ZipFile.ExtractToDirectory(sourceDirectoryPath, temporaryDirectoryPath);
            string[] dllFiles = Directory.GetFiles(temporaryDirectoryPath, "*.dll");
            if (dllFiles.Length == 0) return;

            entity.AssemblyRaw = File.ReadAllBytes(dllFiles[0]);

            Directory.Delete(temporaryDirectoryPath, true);

            /*string[] directoriesPath = Directory.GetDirectories(temporaryDirectoryPath);
            if (directoriesPath.Length > 0)
                foreach (string directoryPath in directoriesPath)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    Directory.Move(directoryPath, Path.Combine(entity.DirectoryPath, directoryInfo.Name));
                }*/
        }

        
    }
}
