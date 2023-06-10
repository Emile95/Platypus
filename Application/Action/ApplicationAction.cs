using Application.Exceptions;
using PlatypusApplicationFramework;
using PlatypusApplicationFramework.Action;
using System.Reflection;

namespace Application.Action
{
    public class ApplicationAction
    {
        private readonly Func<ApplicationActionEnvironmentBase, object> _action;

        public string Name { get; private set; }
        public Type EnvironmentParameterType { get; private set; }
        public Type ParameterType { get; private set; }
        public bool ParameterRequired { get; private set; }

        public ApplicationAction(PlatypusApplicationBase application, ActionDefinitionAttribute actionDefinitionAttribute, MethodInfo methodInfo)
        {
            ParameterInfo parameterInfos = methodInfo.GetParameters()[0];

            Name = actionDefinitionAttribute.Name;
            EnvironmentParameterType = parameterInfos.ParameterType;

            Type[] generics = EnvironmentParameterType.GetGenericArguments();
            ParameterType = generics.Length > 0 ? generics[0] : null;

            ParameterRequired = actionDefinitionAttribute.ParameterRequired;

            _action = (env) =>
            {
                try
                {
                    env.AssertFailed = (failedMessage) => throw new ApplicationActionRunFailedException(failedMessage);
                    object objectResult = methodInfo.Invoke(application, new object[] { env });
                    return new ApplicationActionResult()
                    {
                        Status = ApplicationActionResultStatus.Success,
                        Message = "Application run successfully",
                    }; ;
                }
                catch (TargetInvocationException ex)
                {
                    string failedMessage = "";
                    if(ex.InnerException is ApplicationActionRunFailedException)
                    {
                        ApplicationActionRunFailedException exception = (ApplicationActionRunFailedException)ex.InnerException;
                        failedMessage = exception.FailedMessage;
                    }
                    else
                    {
                        failedMessage = ex.InnerException.Message;
                    }
                    return new ApplicationActionResult()
                    {
                        Status = ApplicationActionResultStatus.Failed,
                        Message = failedMessage
                    };
                }
            };
        }

        public ApplicationActionResult RunAction(ApplicationActionEnvironmentBase env, RunActionParameter runActionParameter)
        {
            if (ParameterType != null)
            {
                if(runActionParameter.ActionParameters == null)
                {
                    if (ParameterRequired)
                        throw new ApplicationActionParameterRequiredException(Name);
                }
                else ResolveActionParameter(env, runActionParameter.ActionParameters);
            }

            object resultObject = _action.Invoke(env);

            return new ApplicationActionResult()
            {
                ResultObject = resultObject,
                Status = ApplicationActionResultStatus.Success
            };
        }

        private void ResolveActionParameter(ApplicationActionEnvironmentBase env, Dictionary<string, object> parameters)
        {
            object resolvedParam = Activator.CreateInstance(ParameterType);

            PropertyInfo[] propertyInfos = ParameterType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
                ResolveActionParameterProperty(parameters, resolvedParam, propertyInfo);

            Type environmentType = env.GetType();
            PropertyInfo parameterPropertyInfo = environmentType.GetProperty("Parameter");

            parameterPropertyInfo.SetValue(env, resolvedParam);
        }

        private void ResolveActionParameterProperty(Dictionary<string, object> parameters, object resolvedParam, PropertyInfo propertyInfo)
        {
            ActionParameterAttribute actionParameterAttribute = propertyInfo.GetCustomAttribute<ActionParameterAttribute>();
            if (actionParameterAttribute == null) return;

            if (parameters.ContainsKey(actionParameterAttribute.Name))
            {
                propertyInfo.SetValue(resolvedParam, parameters[actionParameterAttribute.Name]);
                return;
            }

            if (actionParameterAttribute.DefaultValue != null)
            {
                propertyInfo.SetValue(resolvedParam, actionParameterAttribute.DefaultValue);
                return;
            }

            if (actionParameterAttribute.Required)
                throw new ApplicationActionFieldRequired(actionParameterAttribute.Name);
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment()
        {
            if (ParameterType == null)
                return new ApplicationActionEnvironmentBase();

            return (ApplicationActionEnvironmentBase)Activator.CreateInstance(EnvironmentParameterType);
        }
    }
}
