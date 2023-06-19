using Core.ApplicationAction.Run;
using Core.Exceptions;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Configuration;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusAPI.ApplicationAction;
using PlatypusApplicationFramework.Confugration;
using PlatypusApplicationFramework.Configuration.Exceptions;

namespace Core.ApplicationAction
{
    public class ApplicationAction
    {
        private readonly Func<ApplicationActionEnvironmentBase, ApplicationActionRunResult> _action;

        public string Guid { get; private set; }
        public string Name { get; private set; }
        public Type EnvironmentParameterType { get; private set; }
        public Type ParameterType { get; private set; }
        public bool ParameterRequired { get; private set; }

        public ApplicationAction(PlatypusApplicationBase application, ActionDefinitionAttribute actionDefinitionAttribute, MethodInfo methodInfo, string guid)
        {
            ParameterInfo parameterInfos = methodInfo.GetParameters()[0];

            Guid = guid;
            Name = actionDefinitionAttribute.Name;
            EnvironmentParameterType = parameterInfos.ParameterType;

            Type[] generics = EnvironmentParameterType.GetGenericArguments();
            ParameterType = generics.Length > 0 ? generics[0] : null;

            ParameterRequired = actionDefinitionAttribute.ParameterRequired;

            _action = (env) => BuildAction(application, env, methodInfo);
        }

        public ApplicationActionRunResult RunAction(ApplicationActionEnvironmentBase env, ApplicationActionRunParameter runActionParameter)
        {
            if (ParameterType != null)
            {
                if (runActionParameter.ActionParameters == null)
                {
                    if (ParameterRequired)
                        throw new ApplicationActionParameterRequiredException(Name);
                }
                else
                {
                    try {
                        ResolveActionParameter(env, runActionParameter.ActionParameters);
                    } catch(ParameterEditorFieldRequiredException exception)
                    {
                        return new ApplicationActionRunResult()
                        {
                            Message = exception.Message,
                            Status = ApplicationActionRunResultStatus.Failed
                        };
                    }
                }
            }

            return _action.Invoke(env);
        }

        public ApplicationActionInfo GetInfo()
        {
            return new ApplicationActionInfo()
            {
                Name = this.Name,
                Guid = this.Guid
            };
        }

        private ApplicationActionRunResult BuildAction(PlatypusApplicationBase application, ApplicationActionEnvironmentBase env, MethodInfo methodInfo)
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

                string message = Strings.ResourceManager.GetString("ApplicationActionRunSuccess");

                return new ApplicationActionRunResult()
                {
                    Status = ApplicationActionRunResultStatus.Success,
                    Message = message,
                    ResultObject = objectResult
                }; ;
            }
            catch (TargetInvocationException ex)
            {
                string message = "";
                ApplicationActionRunResultStatus status = ApplicationActionRunResultStatus.Failed;
                if (ex.InnerException is ApplicationActionRunFailedException)
                {
                    ApplicationActionRunFailedException exception = (ApplicationActionRunFailedException)ex.InnerException;
                    message = exception.FailedMessage;
                }
                else if (ex.InnerException is ApplicationActionCanceledException)
                {
                    ApplicationActionCanceledException exception = (ApplicationActionCanceledException)ex.InnerException;
                    status = ApplicationActionRunResultStatus.Canceled;
                    message = exception.CancelMessage;
                }
                else
                {
                    message = ex.InnerException.Message;
                }
                return new ApplicationActionRunResult()
                {
                    Status = status,
                    Message = message
                };
            } catch (Exception ex)
            {
                return new ApplicationActionRunResult()
                {
                    Status = ApplicationActionRunResultStatus.Failed,
                    Message = ex.Message
                };
            }
        }

        private void ResolveActionParameter(ApplicationActionEnvironmentBase env, Dictionary<string, object> parameters)
        {
            object resolvedParam = ParameterEditorObjectResolver.ResolveByDictionnary(ParameterType, parameters);

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
