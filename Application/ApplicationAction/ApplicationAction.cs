using Core.ApplicationAction.Run;
using Core.Exceptions;
using PlatypusAPI.ApplicationAction;
using PlatypusApplicationFramework.ApplicationAction;
using PlatypusApplicationFramework.Configuration;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;

namespace Core.ApplicationAction
{
    public class ApplicationAction
    {
        private readonly Func<ApplicationActionEnvironmentBase, ApplicationActionResult> _action;

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

            _action = (env) => BuildAction(application, env, methodInfo);
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

            return _action.Invoke(env);
        }

        private ApplicationActionResult BuildAction(PlatypusApplicationBase application, ApplicationActionEnvironmentBase env, MethodInfo methodInfo)
        {
            try
            {
                env.AssertFailed = (failedMessage) => throw new ApplicationActionRunFailedException(failedMessage);
                env.AssertCanceled = (cancelMessage, callback) =>
                {
                    if(env.ActionCancelled)
                    {
                        callback();
                        throw new ApplicationActionCanceledException(cancelMessage);
                    }
                };

                object objectResult = methodInfo.Invoke(application, new object[] { env });
                return new ApplicationActionResult()
                {
                    Status = ApplicationActionResultStatus.Success,
                    Message = "Application run successfully",
                    ResultObject = objectResult
                }; ;
            }
            catch (TargetInvocationException ex)
            {
                string message = "";
                ApplicationActionResultStatus status = ApplicationActionResultStatus.Failed;
                if (ex.InnerException is ApplicationActionRunFailedException)
                {
                    ApplicationActionRunFailedException exception = (ApplicationActionRunFailedException)ex.InnerException;
                    message = exception.FailedMessage;
                }
                else if (ex.InnerException is ApplicationActionCanceledException)
                {
                    ApplicationActionCanceledException exception = (ApplicationActionCanceledException)ex.InnerException;
                    status = ApplicationActionResultStatus.Canceled;
                    message = exception.CancelMessage;
                }
                else
                {
                    message = ex.InnerException.Message;
                }
                return new ApplicationActionResult()
                {
                    Status = status,
                    Message = message
                };
            }
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
            ParameterEditorAttribute parameterEditorAttribute = propertyInfo.GetCustomAttribute<ParameterEditorAttribute>();
            if (parameterEditorAttribute == null) return;

            if (parameters.ContainsKey(parameterEditorAttribute.Name))
            {
                propertyInfo.SetValue(resolvedParam, parameters[parameterEditorAttribute.Name]);
                return;
            }

            if (parameterEditorAttribute.DefaultValue != null)
            {
                propertyInfo.SetValue(resolvedParam, parameterEditorAttribute.DefaultValue);
                return;
            }

            if (parameterEditorAttribute.IsRequired)
                throw new ApplicationActionFieldRequired(parameterEditorAttribute.Name);
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment()
        {
            if (ParameterType == null)
                return new ApplicationActionEnvironmentBase();

            return (ApplicationActionEnvironmentBase)Activator.CreateInstance(EnvironmentParameterType);
        }
    }
}
