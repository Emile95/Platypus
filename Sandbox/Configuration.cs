using PlatypusApplicationFramework.Configuration;

namespace Sandbox
{
    public class Configuration
    {
        [ParameterEditor(
            Name = "Text",
            IsRequired = true)]
        public string Text { get; set; }
    }
}
