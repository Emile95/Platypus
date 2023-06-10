namespace PlatypusApplicationFramework.ApplicationAction.Configuration
{
    public class ActionParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public bool Required { get; set; }
    }
}
