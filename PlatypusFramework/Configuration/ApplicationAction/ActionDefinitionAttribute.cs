namespace PlatypusFramework.Configuration.ApplicationAction
{
    public class ActionDefinitionAttribute : Attribute
    {
        public string Name { get; set; }
        public bool ParameterRequired { get; set; }
        public Type ReturnType { get; set; }
    }
}
