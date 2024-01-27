namespace PlatypusNetwork
{
    public class RequestData<RequestEnumType>
        where RequestEnumType : Enum
    {
        public RequestEnumType RequestType { get; set; }
        public byte[] Data { get; set; }
    }
}
