﻿using Persistance;
using Persistance.Entity;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;
using PlatypusApplicationFramework.Core.ApplicationAction.Logger;

namespace Core.ApplicationAction.Run
{
    public class ApplicationActionRun
    {
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly Dictionary<string, ApplicationActionRun> _applicationActionRuns;

        public string ActionGuid { get; set; }
        public string Guid { get; set; }
        public int RunNumber { get; set; }
        public Task Task { get; private set; }
        public ApplicationActionEnvironmentBase Env { get; set; }
        public ApplicationActionRunInfoStatus Status { get; private set; }
        public ApplicationActionResult Result { get; private set; }

        public ApplicationActionRun(
            ApplicationActionRepository applicationActionRepository, 
            Dictionary<string, ApplicationActionRun> runningApplicationActions
        )
        {
            _applicationActionRepository = applicationActionRepository;
            _applicationActionRuns = runningApplicationActions;
        }

        public void StartRun(ApplicationAction action, ApplicationActionRunParameter parameter)
        {
            SetLoggerManager();
            Status = ApplicationActionRunInfoStatus.Running;
            Task = new Task(() => RunAction(() => action.RunAction(Env, parameter)));
            Task.Start();
        }

        public ApplicationActionRunInfo GetInfo()
        {
            return new ApplicationActionRunInfo()
            {
                Guid = this.Guid,
                Status = this.Status,
                RunNumber = this.RunNumber,
            };
        }

        public void Cancel()
        {
            Env.ActionCancelled = true;
        }

        private void RunAction(Func<ApplicationActionResult> action)
        {
            Result = action();
            switch(Result.Status)
            {
                case ApplicationActionResultStatus.Success:
                    Status = ApplicationActionRunInfoStatus.Finish;
                    break;
                case ApplicationActionResultStatus.Failed:
                case ApplicationActionResultStatus.Canceled:
                    Status = ApplicationActionRunInfoStatus.Aborted;
                    break;
            }

            _applicationActionRepository.SaveActionRunResult(
                ActionGuid,
                RunNumber,
                new ApplicationActionResultEntity()
                {
                    Status = Result.Status.ToString(),
                    Message = Result.Message
                }
             );

            _applicationActionRuns.Remove(Guid);
        }

        private void SetLoggerManager()
        {
            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(ActionGuid, RunNumber);
            Env.ActionLoggers.CreateLogger<ApplicationActionRunFileLogger>(configFilePath);
        }
    }
}