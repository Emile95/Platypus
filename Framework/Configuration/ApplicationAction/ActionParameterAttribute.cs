namespace PlatypusApplicationFramework.Configuration.ApplicationAction
{
    public class ActionParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public bool Required { get; set; }
    }
}
