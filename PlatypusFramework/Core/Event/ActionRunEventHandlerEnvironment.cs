﻿using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;

namespace PlatypusFramework.Core.Event
{
    public class ActionRunEventHandlerEnvironment : EventHandlerEnvironment
    {
        public ApplicationActionRunResult ApplicationActionResult { get; set; }
        public ApplicationActionInfo ApplicationActionInfo { get; set; }
        public ApplicationActionRunInfo ApplicationActionRunInfo { get; set; }
    }
}
