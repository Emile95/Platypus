using PlatypusAPI.Application;

namespace PlatypusAPI.Network.ServerResponse
{
    public class GetApplicationInfosServerResponse : PlatypusServerResponse
    {
        public List<ApplicationInfo> ApplicationInfos { get; set; }
    }
}
