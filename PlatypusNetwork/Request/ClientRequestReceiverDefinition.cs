using PlatypusNetwork.Exceptions;
using PlatypusUtils;

namespace PlatypusNetwork.Request
{
    public class ClientRequestReceiverDefinition<ExceptionEnumType, RequestTypeEnum, ClientRequestType, ServerResponseType> : ClientRequestReceiverDefinitionBase<ExceptionEnumType, RequestTypeEnum>
        where ExceptionEnumType : Enum
        where RequestTypeEnum : Enum
        where ClientRequestType : ClientRequestBase
        where ServerResponseType : ServerResponseBase<ExceptionEnumType>, new()
    {
        public Action<string, ClientRequestType, ServerResponseType> ServerAction {get; set;}

        public ClientRequestReceiverDefinition(RequestTypeEnum requestType)
            : base(requestType) {}

        public override bool HandleClientRequest(string clientKey, RequestData<RequestTypeEnum> clientRequestData, Action<RequestData<RequestTypeEnum>> serverResponseConsumer)
        {
            bool exceptionThrowed = false;
            RequestData<RequestTypeEnum> serverResponseData = new RequestData<RequestTypeEnum>()
            {
                RequestType = _requestType
            };
            ServerResponseType serverResponse = new ServerResponseType();
            ClientRequestType clientRequest = Utils.GetObjectFromBytes<ClientRequestType>(clientRequestData.Data);
            serverResponse.RequestKey = clientRequest.RequestKey;
            try
            {
                ServerAction(clientKey, clientRequest, serverResponse);
            }
            catch (FactorisableException<ExceptionEnumType> e)
            {
                serverResponse.ExceptionThrowed = true;
                serverResponse.FactorisableExceptionType = e.FactorisableExceptionType;
                serverResponse.FactorisableExceptionParameters = e.GetParameters();
                exceptionThrowed = true;
            }
            serverResponseData.Data = Utils.GetBytesFromObject(serverResponse);
            serverResponseConsumer(serverResponseData);
            return exceptionThrowed;
        }
    }
}
