﻿using PlatypusApplicationFramework.Configuration.ApplicationAction;
using PlatypusApplicationFramework.Core.ApplicationAction;
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
            env.ActionLoggers.Log("orion");

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
    }
}
