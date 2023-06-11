using PlatypusApplicationFramework.Configuration.ApplicationAction;
using PlatypusApplicationFramework.ApplicationAction;
using PlatypusApplicationFramework.ApplicationAction.Logger;
using Sandbox.ActionParameter;
using PlatypusApplicationFramework.Configuration.Application;

namespace Sandbox
{
    internal class PlatypusApplication : ConfigurablePlatypusApplication<Configuration>
    {
        [ActionDefinition(
            Name = "Some action")]
        public object SomeAction(ApplicationActionEnvironment<SomeActionParameter> env)
        {
            env.ActionLoggers.Log<ApplicationActionRunFileLogger>("j'aime ca logger");
            env.ActionLoggers.Log<ApplicationActionRunFileLogger>("oh que oui");

            Console.WriteLine("phase 1");
            Task.Delay(5000).Wait();
            env.AssertCanceled("phase 2 canceled", () => {
                Console.WriteLine("phase 2 has ben canceled");
            });
            Console.WriteLine("phase 2");

            Task.Delay(5000).Wait();
            env.AssertCanceled("phase 3 canceled", () => {
                Console.WriteLine("phase 3 has ben canceled");
            });
            Console.WriteLine("phase 3");
            Task.Delay(5000).Wait();
            Console.WriteLine(env.Parameter.Text + " : " + env.Parameter.Number + " : " + env.Parameter.Double + " : " + env.Parameter.Boolean + " chibougamo gonzo");
            Console.WriteLine(Configuration.Text);
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
