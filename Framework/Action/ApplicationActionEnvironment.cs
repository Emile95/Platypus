﻿namespace PlatypusApplicationFramework.Action
{
    public class ApplicationActionEnvironment<ParameterType> : ApplicationActionEnvironmentBase
    {
        public ParameterType Parameter { get; set; }
    }
}
