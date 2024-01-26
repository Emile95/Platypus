using Persistance.Repository;
using Persistance.Entity;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusUtils;
using PlatypusFramework.Configuration.User;
using Core.Exceptions;

namespace Core.Application
{
    public class ApplicationInstaller
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly UserRepository _userRepository;

        public ApplicationInstaller(
            ApplicationRepository applicationRepository,
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

            string newDllFilePath = _applicationRepository.SaveApplication(new ApplicationEntity()
            {
                Guid = newGuid
            }, applicationPath);

            PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(newDllFilePath);

            Type type = platypusApplication.GetType();
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (InstallAction(newGuid, method)) continue;
                if (InstallUserConnectionMethod(platypusApplication, newGuid, method)) continue;
            }

            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationRepository = _applicationRepository;
            env.ApplicationGuid = newGuid;

            platypusApplication.Install(env);

            return platypusApplication;
        }

        public UninstallApplicationDetails UninstallApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationRepository = _applicationRepository;
            env.ApplicationGuid = applicationGuid;

            application.Uninstall(env);

            _applicationRepository.RemoveApplication(applicationGuid);

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
    }
}
