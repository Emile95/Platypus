using PlatypusNetwork.Request.Data;

namespace PlatypusNetwork.Request.Definition
{
    public abstract class ClientRequestReceiverDefinitionBase<ExceptionEnumType, RequestTypeEnum> : RequestDefinitionBase<ExceptionEnumType, RequestTypeEnum>
        where ExceptionEnumType : Enum
        where RequestTypeEnum : Enum
    {
        public ClientRequestReceiverDefinitionBase(RequestTypeEnum requestType)
            : base(requestType) { }

        public abstract bool HandleClientRequest(string clientKey, RequestData<RequestTypeEnum> clientRequestData, Action<RequestData<RequestTypeEnum>> serverResponseConsumer);
    }
}
