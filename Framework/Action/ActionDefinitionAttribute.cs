﻿namespace PlatypusApplicationFramework.Action
{
    public class ActionDefinitionAttribute : Attribute
    {
        public string Name { get; set; }
        public bool ParameterRequired { get; set; }
    }
}
