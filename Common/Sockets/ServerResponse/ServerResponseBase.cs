﻿using Common.Exceptions;

namespace Common.Sockets.ServerResponse
{
    public class ServerResponseBase
    {
        public string RequestKey { get; set; }
        public FactorisableExceptionType FactorisableExceptionType { get; set; }
        public object[] FactorisableExceptionParameters { get; set; }
    }
}