using PlatypusApplicationFramework.Configuration.ApplicationAction;

namespace Sandbox.ActionParameter
{
    public class SomeActionParameter
    {
        [ActionParameter(
            Name = "Text",
            DefaultValue = "Guacamole")]
        public string Text { get; set; }

        [ActionParameter(
            Name = "Number",
            Required = true)]
        public int Number { get; set; }

        [ActionParameter(
            Name = "Double",
            Required = true)]
        public double Double { get; set; }

        [ActionParameter(
            Name = "Boolean",
            Required = true)]
        public bool Boolean { get; set; }
    }

    public class Param2
    {
        [ActionParameter(
            Name = "Text",
            DefaultValue = "Guacamole")]
        public string Text { get; set; }

        [ActionParameter(
            Name = "Number",
            Required = true)]
        public int Number { get; set; }
    }
}
