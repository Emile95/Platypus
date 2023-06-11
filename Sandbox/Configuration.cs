using PlatypusApplicationFramework.Configuration;

namespace Sandbox
{
    public class Configuration
    {
        [ParameterEditor(
            Name = "Text",
            DefaultValue = "J'ai peur dur noir")]
        public string Text { get; set; }
    }
}
