using Persistance.Repository;
using Persistance.Entity;
using PlatypusApplicationFramework.Configuration.Application;
using Utils;
using Utils.GuidGeneratorHelper;
using Persistance;
using Core.Exceptions;

namespace Core.Application
{
    public class ApplicationsHandler
    {
        private readonly ApplicationRepository _applicationRepository;
        private readonly ApplicationInstaller _applicationInstaller;
        private readonly ApplicationResolver _applicationResolver;
        private readonly Dictionary<string, PlatypusApplicationBase> _applications;

        public ApplicationsHandler(
            ApplicationRepository applicationRepository,
            ApplicationActionRepository applicationActionRepository,
            ApplicationResolver applicationResolver
        )
        {
            _applicationRepository = applicationRepository;

            _applicationResolver = applicationResolver;

            _applicationInstaller = new ApplicationInstaller(
                _applicationRepository,
                applicationActionRepository
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
            PlatypusApplicationBase application = _applicationInstaller.InstallApplication(newGuid, dllFilePath);
            LoadApplication(application, newGuid);
        }

        public List<string> UninstallApplication(string applicationGuid)
        {
            if (_applications.ContainsKey(applicationGuid) == false)
                throw new ApplicationInexistantException(applicationGuid);

            PlatypusApplicationBase application = _applications[applicationGuid];
            _applications.Remove(applicationGuid);
            return _applicationInstaller.UninstallApplication(application, applicationGuid);
        }
    }
}
