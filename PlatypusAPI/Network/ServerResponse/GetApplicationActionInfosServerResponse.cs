using PlatypusAPI.ApplicationAction;

namespace PlatypusAPI.Network.ServerResponse
{
    public class GetApplicationActionInfosServerResponse : ServerResponseBase
    {
        public List<ApplicationActionInfo> ApplicationActionInfos { get; set; }
    }
}
