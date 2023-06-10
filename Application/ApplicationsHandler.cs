using Application.Action;
using Persistance;
using PlatypusApplicationFramework;
using Utils.GuidGeneratorHelper;

namespace Application
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
            _applicationResolver = new ApplicationResolver(applicationActionsHandler);
            _applicationInstaller = new ApplicationInstaller(
                _applicationRepository,
                applicationActionRepository,
                _applicationResolver
             );
            _applications = new Dictionary<string, PlatypusApplicationBase>();
        }

        public void LoadApplications()
        {
            List<Tuple<PlatypusApplicationBase, string>> applications = _applicationRepository.LoadApplications();
            foreach(Tuple<PlatypusApplicationBase, string> application in applications)
                LoadApplication(application.Item1, application.Item2);
        }

        public void LoadApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            _applicationResolver.ResolvePlatypusApplication(application, applicationGuid);
            _applications.Add(applicationGuid, application);
        }

        public void InstallApplication(string dllFilePath)
        {
            string newGuid = GuidGenerator.GenerateFromEnumerable(_applications.Keys);
            _applicationInstaller.InstallApplication(newGuid, dllFilePath);
        }
    }
}
