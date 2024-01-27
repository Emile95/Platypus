using PlatypusAPI.ApplicationAction;

namespace PlatypusAPI.Network.ServerResponse
{
    public class GetApplicationActionInfosServerResponse : PlatypusServerResponse
    {
        public List<ApplicationActionInfo> ApplicationActionInfos { get; set; }
    }
}
