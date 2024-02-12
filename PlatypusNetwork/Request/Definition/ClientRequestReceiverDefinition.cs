using PlatypusNetwork.Exceptions;
using PlatypusNetwork.Request.Data;
using PlatypusUtils;

namespace PlatypusNetwork.Request.Definition
{
    public class ClientRequestReceiverDefinition<ExceptionEnumType, RequestTypeEnum, ClientRequestType, ServerResponseType> : ClientRequestReceiverDefinitionBase<ExceptionEnumType, RequestTypeEnum>
        where ExceptionEnumType : Enum
        where RequestTypeEnum : Enum
        where ClientRequestType : ClientRequestBase
        where ServerResponseType : ServerResponseBase<ExceptionEnumType>, new()
    {
        public Action<string, ClientRequestType, ServerResponseType> ServerAction { get; set; }
        public Action<string, FactorisableException<ExceptionEnumType>> OnExceptionThrowedCallBack { get; set; }

        public ClientRequestReceiverDefinition(RequestTypeEnum requestType)
            : base(requestType) { }

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
            FactorisableException<ExceptionEnumType> exception = null;
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
                exception = e;
            }
            serverResponseData.Data = Utils.GetBytesFromObject(serverResponse);
            serverResponseConsumer(serverResponseData);
            if (exceptionThrowed) OnExceptionThrowedCallBack(clientKey, exception);
            return exceptionThrowed;
        }
    }
}
