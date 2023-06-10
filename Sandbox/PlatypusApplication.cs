using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using Sandbox.ActionParameter;

namespace Sandbox
{
    internal class PlatypusApplication : ConfigurablePlatypusApplication<Configuration>
    {
        [ActionDefinition(
            Name = "Some action")]
        public object SomeAction(ApplicationActionEnvironment<SomeActionParameter> env)
        {
            env.AssertFailed("FAIIIIIIIIIIILEEEEEEEEEEEDDD");
            Task.Delay(50000).Wait();
            Console.WriteLine(env.Parameter.Text + " : " + env.Parameter.Number + " : " + env.Parameter.Double + " : " + env.Parameter.Boolean + " chibougamo gonzo");
            return "Some action success you fool";
        }

        public override bool ValidateConfiguration(Configuration configuration)
        {
            throw new NotImplementedException();
        }

        protected override void OnConfigurationUpdate(Configuration previousConfiguration)
        {
            throw new NotImplementedException();
        }
    }
}
