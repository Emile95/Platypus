using PlatypusApplicationFramework.Configuration;

namespace Sandbox.ActionParameter
{
    public class SomeActionParameter
    {
        [ParameterEditor(
            Name = "Text",
            DefaultValue = "Guacamole")]
        public string Text { get; set; }

        [ParameterEditor(
            Name = "Number",
            IsRequired = true)]
        public int Number { get; set; }

        [ParameterEditor(
            Name = "Double",
            IsRequired = true)]
        public double Double { get; set; }

        [ParameterEditor(
            Name = "Boolean",
            IsRequired = true)]
        public bool Boolean { get; set; }
    }

    public class Param2
    {
        [ParameterEditor(
            Name = "Text",
            DefaultValue = "Guacamole")]
        public string Text { get; set; }

        [ParameterEditor(
            Name = "Number",
            IsRequired = true)]
        public int Number { get; set; }
    }
}
