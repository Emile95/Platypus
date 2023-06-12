﻿using Logging;
using Persistance;

namespace PlatypusApplicationFramework.ApplicationAction
{
    public class ApplicationActionEnvironmentBase
    {
        public bool ActionCancelled { get; set; }
        public Action<string> AssertFailed { get; set; }
        public Action<string, Action> AssertCanceled { get; set; }
        public LoggerManager ActionLoggers { get; set; }
        public ApplicationRepository ApplicationRepository { get; set; }
    }
}
