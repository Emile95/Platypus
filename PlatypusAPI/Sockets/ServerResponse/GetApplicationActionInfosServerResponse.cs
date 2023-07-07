using PaltypusAPI.Sockets.ServerResponse;
using PlatypusAPI.ApplicationAction;

namespace PlatypusAPI.Sockets.ServerResponse
{
    public class GetApplicationActionInfosServerResponse : ServerResponseBase
    {
        public List<ApplicationActionInfo> ApplicationActionInfos { get; set; }
    }
}
