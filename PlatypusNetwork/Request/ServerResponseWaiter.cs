﻿namespace PlatypusNetwork.Request
{
    public class ServerResponseWaiter<ExceptionEnumType, ResponseType>
            where ResponseType : ServerResponseBase<ExceptionEnumType>
            where ExceptionEnumType : Enum
    {
        public bool Received { get; set; }
        public ResponseType Response { get; set; }
    }
}