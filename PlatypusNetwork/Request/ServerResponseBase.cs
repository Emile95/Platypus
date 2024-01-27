﻿namespace PlatypusNetwork.ServerResponse
{
    public class ServerResponseBase<ExceptionEnumType>
        where ExceptionEnumType : Enum
    {
        public string RequestKey { get; set; }
        public ExceptionEnumType FactorisableExceptionType { get; set; }
        public object[] FactorisableExceptionParameters { get; set; }
    }
}
