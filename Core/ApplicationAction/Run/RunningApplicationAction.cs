using Logging;
using Persistance;
using Persistance.Entity;
using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.ApplicationAction;
using PlatypusApplicationFramework.ApplicationAction.Logger;

namespace Core.ApplicationAction.Run
{
    public class RunningApplicationAction
    {
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly Dictionary<string, RunningApplicationAction> _runningApplicationActions;

        public string ActionGuid { get; set; }
        public string Guid { get; set; }
        public int RunNumber { get; set; }
        public Task Task { get; private set; }
        public ApplicationActionEnvironmentBase Env { get; set; }
        public RunningApplicationActionStatus Status { get; private set; }
        public ApplicationActionResult Result { get; private set; }

        public RunningApplicationAction(
            string actionGuid,
            string guid,
            int runNumber,
            ApplicationActionRepository applicationActionRepository, 
            ApplicationAction action,
            RunActionParameter runActionParameter, 
            ApplicationActionEnvironmentBase env,
            Dictionary<string, RunningApplicationAction> runningApplicationActions
        )
        {
            ActionGuid = actionGuid;
            Guid = guid;
            RunNumber = runNumber;
            Env = env;
            _applicationActionRepository = applicationActionRepository;
            _runningApplicationActions = runningApplicationActions;

            SetLoggerManager();

            Status = RunningApplicationActionStatus.Running;
            Task = new Task(() => RunAction(() => action.RunAction(Env, runActionParameter)));
            Task.Start();
        }

        public RunningApplicationActionInfo GetInfo()
        {
            return new RunningApplicationActionInfo()
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
                    Status = RunningApplicationActionStatus.Finish;
                    break;
                case ApplicationActionResultStatus.Failed:
                case ApplicationActionResultStatus.Canceled:
                    Status = RunningApplicationActionStatus.Aborted;
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
            _runningApplicationActions.Remove(Guid);
        }

        private void SetLoggerManager()
        {
            string configFilePath = _applicationActionRepository.GetRunActionLogFilePath(ActionGuid, RunNumber);
            Env.ActionLoggers.CreateLogger<ApplicationActionRunFileLogger>(configFilePath);
        }
    }
}
