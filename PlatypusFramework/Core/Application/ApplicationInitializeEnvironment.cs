using Persistance.Repository;

namespace PlatypusFramework.Core.Application
{
    public class ApplicationInitializeEnvironment
    {
        public ApplicationRepository ApplicationRepository { get; set; }
        public string ApplicationGuid { get; set; }
    }
}
