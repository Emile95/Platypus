﻿using Core.ApplicationAction.Run;
using Persistance;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Core.ApplicationAction.Logger;
using PlatypusApplicationFramework.Configuration.Application;
using PlatypusApplicationFramework.Configuration.ApplicationAction;
using System.Reflection;
using Utils.GuidGeneratorHelper;
using Core.Event;
using PlatypusApplicationFramework.Core.Event;
using Persistance.Entity;
using Core.Exceptions;

namespace Core.ApplicationAction
{
    public class ApplicationActionsHandler
    {
        public Dictionary<string, ApplicationAction> ApplicationActions { get; private set; }
        public Dictionary<string, ApplicationActionRun> ApplicationActionRuns { get; private set; }

        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly EventsHandler _eventsHandler;

        public ApplicationActionsHandler(
            ApplicationActionRepository applicationActionRepository,
            EventsHandler eventsHandler
        )
        {
            ApplicationActions = new Dictionary<string, ApplicationAction>();
            ApplicationActionRuns = new Dictionary<string, ApplicationActionRun>();

            _applicationActionRepository = applicationActionRepository;
            _eventsHandler = eventsHandler;
        }

        public void AddAction(PlatypusApplicationBase application, string applicationGuid, ActionDefinitionAttribute actionDefinition, MethodInfo methodInfo)
        {
            string actionGuid = actionDefinition.Name+applicationGuid;
            if (ApplicationActions.ContainsKey(actionGuid)) return;

            ApplicationAction applicationAction = new ApplicationAction(application,actionDefinition,methodInfo);
            ApplicationActions.Add(actionGuid, applicationAction);
        }

        public ApplicationActionResult RunAction(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            ApplicationActionResult result = BeforeApplicationActionRun();
            if (result is not null) return result;

            ApplicationActionRun applicationActionRun = CreateApplicationActionRun(runActionParameter, env);

            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(runActionParameter.Guid, applicationActionRun.RunNumber);
            env.ActionLoggers.CreateLogger<ApplicationActionRunFileLogger>(configFilePath);

            ApplicationAction applicationAction = ApplicationActions[runActionParameter.Guid];

            applicationActionRun.StartRun(applicationAction, runActionParameter, (guid) => {
                ApplicationActionRunCallBack(applicationActionRun, guid);
            });

            if (runActionParameter.Async)
                return new ApplicationActionResult() { 
                    Message = $"new action has been started",
                    Status = ApplicationActionResultStatus.Started 
                };

            applicationActionRun.Task.Wait();

            return applicationActionRun.Result;
        }

        public void CancelRunningAction(string guid)
        {
            if (ApplicationActionRuns.ContainsKey(guid) == false)
                return;
            ApplicationActionRuns[guid].Cancel();
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActionInfos()
        {
            return ApplicationActionRuns.Values.Select(x => x.GetInfo());
                
        }

        public ApplicationActionEnvironmentBase CreateStartActionEnvironment(string actionName)
        {
            return ApplicationActions[actionName].CreateStartActionEnvironment();
        }

        private int CreateNewActionRunNumber(string applicationRunGuid)
        {
            string actionRunsFilePath = ApplicationPaths.GetActionRunsDirectoryPath(applicationRunGuid);
            int runNumber = _applicationActionRepository.GetAndIncrementActionRunNumberByBasePath(actionRunsFilePath);
            _applicationActionRepository.SaveActionRunByBasePath(actionRunsFilePath, runNumber);
            return runNumber;
        }

        private ApplicationActionRun CreateApplicationActionRun(ApplicationActionRunParameter runActionParameter, ApplicationActionEnvironmentBase env)
        {
            string applicationActionRunGUID = GuidGenerator.GenerateFromEnumerable(ApplicationActionRuns.Keys);
            int runNumber = CreateNewActionRunNumber(runActionParameter.Guid);

            ApplicationActionRun applicationActionRun = new ApplicationActionRun()
            {
                ActionGuid = runActionParameter.Guid,
                Guid = applicationActionRunGUID,
                RunNumber = runNumber,
                Env = env
            };

            ApplicationActionRuns.Add(
                applicationActionRunGUID,
                applicationActionRun
            );

            return applicationActionRun;
        }

        private void ApplicationActionRunCallBack(ApplicationActionRun run, string applicationRunGuid)
        {
            ApplicationActionRuns.Remove(applicationRunGuid);

            if(run.Result.Status != ApplicationActionResultStatus.Failed &&
               run.Result.Status != ApplicationActionResultStatus.Canceled)
            {
                EventHandlerEnvironment eventEnv = new EventHandlerEnvironment();
                try
                {
                    _eventsHandler.RunEventHandlers(EventHandlerType.AfterApplicationActionRun, eventEnv);
                }
                catch (EventHandlerException ex)
                {
                    run.Result.Status = ApplicationActionResultStatus.Failed;
                    run.Result.Message = ex.Message;
                }
            }

            _applicationActionRepository.SaveActionRunResult(
                run.ActionGuid,
                run.RunNumber,
                new ApplicationActionResultEntity()
                {
                    Status = run.Result.Status.ToString(),
                    Message = run.Result.Message
                }
            );
        }

        private ApplicationActionResult BeforeApplicationActionRun()
        {
            EventHandlerEnvironment eventEnv = new EventHandlerEnvironment();
            try
            {
                _eventsHandler.RunEventHandlers(EventHandlerType.BeforeApplicationActionRun, eventEnv);
            }
            catch (EventHandlerException ex)
            {
                return new ApplicationActionResult()
                {
                    Status = ApplicationActionResultStatus.Failed,
                    Message = ex.Message,
                };
            }

            return null;
        }
    }
}
