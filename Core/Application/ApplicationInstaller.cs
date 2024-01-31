using Persistance.Repository;
using Persistance.Entity;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusUtils;
using PlatypusFramework.Configuration.User;
using Core.Exceptions;
using Persistance;
using System.IO.Compression;

namespace Core.Application
{
    public class ApplicationInstaller
    {
        private readonly Repository<ApplicationEntity, string> _applicationRepository;
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly UserRepository _userRepository;

        public ApplicationInstaller(
            Repository<ApplicationEntity, string> applicationRepository,
            ApplicationActionRepository applicationActionRepository,
            UserRepository userRepository
        )
        {
            _applicationRepository = applicationRepository;
            _applicationActionRepository = applicationActionRepository;
            _userRepository = userRepository;
        }

        public PlatypusApplicationBase InstallApplication(string newGuid, string applicationPath)
        {
            FileInfo applicationFileInfo = new FileInfo(applicationPath);
            if (applicationFileInfo.Extension != ".platypus")
                throw new InvalidPlatypusApplicationPackageException(applicationFileInfo.Name, "wrong file extension");

            ApplicationEntity entity = new ApplicationEntity()
            {
                Guid = newGuid,
                DirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(newGuid)
            };

            Directory.CreateDirectory(entity.DirectoryPath);

            ExtractPackage(entity, applicationPath);

            _applicationRepository.Add(entity);

            try
            {
                PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);

                Type type = platypusApplication.GetType();
                MethodInfo[] methods = type.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    if (InstallAction(newGuid, method)) continue;
                    if (InstallUserConnectionMethod(platypusApplication, newGuid, method)) continue;
                }

                ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
                env.ApplicationGuid = newGuid;

                platypusApplication.Install(env);

                return platypusApplication;
            } catch(Exception)
            {
                _applicationRepository.Remove(newGuid);
                return null;
            }
        }

        public UninstallApplicationDetails UninstallApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationGuid = applicationGuid;

            application.Uninstall(env);

            _applicationRepository.Remove(applicationGuid);

            UninstallApplicationDetails details = new UninstallApplicationDetails()
            {
                ActionGuids = _applicationActionRepository.RemoveActionsOfApplication(applicationGuid),
                UserConnectionMethodGuids = _userRepository.RemoveUserCredentialMethodOfApplication(applicationGuid)
            };

            return details;
        }

        private bool InstallAction(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return false;

            _applicationActionRepository.SaveAction(actionDefinition.Name+ applicationGuid);
            return true;
        }

        public bool InstallUserConnectionMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
        {
            UserConnectionMethodCreatorAttribute userConnectionMethodCreatorAttribute = methodInfo.GetCustomAttribute<UserConnectionMethodCreatorAttribute>();
            if (userConnectionMethodCreatorAttribute == null) return false;

            IUserConnectionMethod connectionMethod = methodInfo.Invoke(application, new object[] { }) as IUserConnectionMethod;
            string connectionMethodName = connectionMethod.GetName();
            _userRepository.SaveUserConnectionMethod(new UserConnectionMethodEntity()
            {
                Description = connectionMethod.GetDescription(),
                Name = connectionMethodName,
                Guid = connectionMethodName+applicationGuid
            });

            return true;
        }

        private void ExtractPackage(ApplicationEntity entity, string sourceDirectoryPath)
        {
            string temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), entity.Guid);
            Directory.CreateDirectory(temporaryDirectoryPath);

            ZipFile.ExtractToDirectory(sourceDirectoryPath, temporaryDirectoryPath);
            string[] dllFiles = Directory.GetFiles(temporaryDirectoryPath, "*.dll");
            if (dllFiles.Length == 0) return;

            entity.AssemblyRaw = File.ReadAllBytes(dllFiles[0]);

            string[] directoriesPath = Directory.GetDirectories(temporaryDirectoryPath);
            if (directoriesPath.Length > 0)
                foreach (string directoryPath in directoriesPath)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    Directory.Move(directoryPath, Path.Combine(entity.DirectoryPath, directoryInfo.Name));
                }
        }
    }
}
