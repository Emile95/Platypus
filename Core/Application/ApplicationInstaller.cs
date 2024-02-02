using Core.Persistance.Repository;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusUtils;
using PlatypusFramework.Configuration.User;
using Core.Exceptions;
using PlatypusRepository;
using System.IO.Compression;
using Core.Persistance.Entity;
using Core.Application.Abstract;

namespace Core.Application
{
    internal class ApplicationInstaller : IApplicationPackageInstaller<PlatypusApplicationBase>
    {
        private readonly IRepositoryAddOperator<ApplicationEntity> _applicationRepositoryAddOperator;
        private readonly IRepositoryRemoveOperator<string> _applicationRepositoryRemoveOperator;

        private readonly IRepositoryAddOperator<ApplicationActionEntity> _applicationActionRepositoryAddOperator;
        

        private readonly UserRepository _userRepository;

        internal ApplicationInstaller(
            IRepositoryAddOperator<ApplicationEntity> applicationRepositoryAddOperator,
            IRepositoryRemoveOperator<string> applicationRepositoryRemoveOperator,
            IRepositoryAddOperator<ApplicationActionEntity> applicationActionRepositoryAddOperator,
            UserRepository userRepository
        )
        {
            _applicationRepositoryAddOperator = applicationRepositoryAddOperator;
            _applicationRepositoryRemoveOperator = applicationRepositoryRemoveOperator;

            _applicationActionRepositoryAddOperator = applicationActionRepositoryAddOperator;

            _userRepository = userRepository;
        }

        public PlatypusApplicationBase Install(string sourcePath)
        {
            FileInfo applicationFileInfo = new FileInfo(sourcePath);
            if (applicationFileInfo.Extension != ".platypus")
                throw new InvalidPlatypusApplicationPackageException(applicationFileInfo.Name, "wrong file extension");

            ApplicationEntity entity = new ApplicationEntity();

            ExtractPackage(entity, sourcePath);

            _applicationRepositoryAddOperator.Add(entity);

            try
            {
                PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);

                platypusApplication.ApplicationGuid = entity.Guid;

                Type type = platypusApplication.GetType();
                MethodInfo[] methods = type.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    if (InstallAction(entity.Guid, method)) continue;
                    if (InstallUserConnectionMethod(platypusApplication, entity.Guid, method)) continue;
                }

                ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
                env.ApplicationGuid = entity.Guid;

                platypusApplication.Install(env);

                return platypusApplication;
            }
            catch (Exception)
            {
                _applicationRepositoryRemoveOperator.Remove(entity.Guid);
                return null;
            }
        }

        private bool InstallAction(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return false;

            _applicationActionRepositoryAddOperator.Add(new ApplicationActionEntity() { 
                Guid = actionDefinition.Name + applicationGuid
            });
            return true;
        }

        private bool InstallUserConnectionMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
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
