using Persistance;

namespace Common.Application
{
    public class ApplicationInitializeEnvironment
    {
        public ApplicationRepository ApplicationRepository { get; set; }
        public string ApplicationGuid { get; set; }
    }
}
