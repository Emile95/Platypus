using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using Sandbox.ActionParameter;

namespace Sandbox
{
    internal class PlatypusApplication : PlatypusApplicationBase
    {
        [ActionDefinition(
            Name = "Some action")]
        public object SomeAction(ApplicationActionEnvironment<SomeActionParameter> env)
        {
            Task.Delay(50000).Wait();
            Console.WriteLine(env.Parameter.Text + " : " + env.Parameter.Number + " : " + env.Parameter.Double + " : " + env.Parameter.Boolean + " chibougamo gonzo");
            return "Some action success you fool";
        }
    }
}
