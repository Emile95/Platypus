using Core.ApplicationAction;
using Persistance;
using Persistance.Entity;
using PlatypusApplicationFramework.Configuration.Application;
using Utils;
using Utils.GuidGeneratorHelper;

namespace Core.Application
{
    public class ApplicationsHandler
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        public Dictionary<string, PlatypusApplicationBase> _applications { get; private set; }

        public ApplicationsHandler(
            ApplicationActionRepository applicationActionRepository,
            ApplicationActionsHandler applicationActionsHandler
        )
        {
            _applicationRepository = new ApplicationRepository();
            _applicationResolver = new ApplicationResolver(
                applicationActionsHandler,
                _applicationRepository
            );
            _applicationInstaller = new ApplicationInstaller(
                _applicationRepository,
                applicationActionRepository,
                _applicationResolver
             );
            _applications = new Dictionary<string, PlatypusApplicationBase>();
        }

        public void LoadApplications()
        {
            List<ApplicationEntity> applications = _applicationRepository.LoadApplications();
            foreach(ApplicationEntity application in applications)
            {
                PlatypusApplicationBase applicationBase = PluginResolver.InstanciateImplementationFromDll<PlatypusApplicationBase>(application.DllFilePath);
                LoadApplication(applicationBase, application.Guid);
            }
        }

        public void LoadApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            _applicationResolver.ResolvePlatypusApplication(application, applicationGuid);
            application.ApplicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(applicationGuid);
            _applications.Add(applicationGuid, application);
        }

        public void InstallApplication(string dllFilePath)
        {
            string newGuid = GuidGenerator.GenerateFromEnumerable(_applications.Keys);
            _applicationInstaller.InstallApplication(newGuid, dllFilePath);
        }
    }
}
