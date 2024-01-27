using PlatypusAPI.Exceptions;
using PlatypusAPI.Network.ServerResponse;
using PlatypusNetwork.Request;


namespace PlatypusAPI.Network
{
    public class PlatypusServerResponseWaiter<ServerResponseType> : ServerResponseWaiter<FactorisableExceptionType, ServerResponseType>
        where ServerResponseType : PlatypusServerResponse
    {}
}
