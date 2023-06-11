using Persistance;

namespace Common.Application
{
    public class ApplicationInstallEnvironment
    {
        public ApplicationRepository ApplicationRepository { get; set; }
        public string ApplicationGuid { get; set; }
    }
}
