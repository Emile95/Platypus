using Persistance;
using PlatypusApplicationFramework;

namespace Application
{
    public class ApplicationsHandler
    {
        private readonly ApplicationRepository applicationRepository;
        public Dictionary<string, PlatypusApplicationBase> Applications { get; private set; }

        public ApplicationsHandler()
        {
            applicationRepository = new ApplicationRepository();
            Applications = new Dictionary<string, PlatypusApplicationBase>();
        }

        public void AddApplication(string guid, PlatypusApplicationBase platypusApplicationBase)
        {
            Applications.Add(guid, platypusApplicationBase);
        }
    }
}
