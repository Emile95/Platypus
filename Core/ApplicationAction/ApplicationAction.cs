using Core.Exceptions;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.ApplicationAction;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusAPI.ApplicationAction;
using PlatypusFramework.Confugration;
using PlatypusUtils;
using Core.Ressource;
using Core.ApplicationAction.Run;

namespace Core.ApplicationAction
{
    public class ApplicationAction
    {
        public Func<ApplicationActionEnvironmentBase, ApplicationActionRunResult> Exec { get; private set; }
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

            Exec = (env) => BuildAction(application, env, methodInfo);
        }

        public void ResolveActionParameter(ApplicationActionEnvironmentBase env, Dictionary<string, object> parameters)
        {
            if (ParameterType != null)
            {
                if (parameters == null)
                {
                    if (ParameterRequired)
                        throw new ApplicationActionParameterRequiredException(Name);
                }
                else
                {
                    object resolvedParam = ParameterEditorObjectResolver.ResolveByDictionnary(ParameterType, parameters);

                    Type environmentType = env.GetType();
                    PropertyInfo parameterPropertyInfo = environmentType.GetProperty("Parameter");

                    parameterPropertyInfo.SetValue(env, resolvedParam);
                }
            }   
        }

        public ApplicationActionInfo GetInfo()
        {
            return new ApplicationActionInfo()
            {
                Name = this.Name,
                Guid = this.Guid,
                ParentApplicationGuid = GetParentApplicationGuid()
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

                string message = Utils.GetString(Strings.ResourceManager, "ApplicationActionRunSuccess");

                return new ApplicationActionRunResult()
                {
                    Status = ApplicationActionRunResultStatus.Success,
                    Message = message,
                    ResultObject = objectResult
                };
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

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment()
        {
            if (ParameterType == null)
                return new ApplicationActionEnvironmentBase();

            return (ApplicationActionEnvironmentBase)Activator.CreateInstance(EnvironmentParameterType);
        }

        public ApplicationActionRun CreateApplicationActionRun(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            string applicationActionRunGUID = Utils.GenerateGuidFromEnumerable(_applicationActionRuns.Keys);
            int runNumber = 5;

            ApplicationActionRun applicationActionRun = new ApplicationActionRun()
            {
                ActionGuid = runActionParameter.Guid,
                Guid = applicationActionRunGUID,
                RunNumber = runNumber,
                Env = env
            };

            return applicationActionRun;
        }

        private string GetParentApplicationGuid()
        {
            return Guid.Replace(Name, "");
        }
    }
}
