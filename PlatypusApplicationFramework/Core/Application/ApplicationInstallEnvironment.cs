using Persistance.Repository;

namespace PlatypusApplicationFramework.Core.Application
{
    public class ApplicationInstallEnvironment
    {
        public ApplicationRepository ApplicationRepository { get; set; }
        public string ApplicationGuid { get; set; }
    }
}
