using Persistance.Repository;
using Persistance.Entity;
using PlatypusApplicationFramework.Core.Application;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Utils;
using PlatypusApplicationFramework.Configuration.User;

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

        public PlatypusApplicationBase InstallApplication(string newGuid, string dllFilePath)
        {
            string newDllFilePath = _applicationRepository.SaveApplication(new ApplicationEntity()
            {
                Guid = newGuid,
                DllFilePath = dllFilePath
            });

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

        public List<string> UninstallApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationRepository = _applicationRepository;
            env.ApplicationGuid = applicationGuid;

            application.Uninstall(env);

            _applicationRepository.RemoveApplication(applicationGuid);
            return _applicationActionRepository.RemoveActionsOfApplication(applicationGuid);
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
            _userRepository.SaveUserCredentialMethod(new UserCredentialMethodEntity()
            {
                Description = connectionMethod.GetDescription(),
                Name = connectionMethodName,
                Guid = connectionMethodName+applicationGuid
            });

            return true;
        }
    }
}
