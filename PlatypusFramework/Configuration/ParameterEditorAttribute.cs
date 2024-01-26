namespace PlatypusFramework.Configuration
{
    public class ParameterEditorAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public bool IsRequired { get; set; }
    }
}
