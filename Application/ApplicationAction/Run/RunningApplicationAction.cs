using Persistance;
using PlatypusApplicationFramework.ApplicationAction;

namespace Application.ApplicationAction.Run
{
    public class RunningApplicationAction
    {
        private readonly ApplicationActionRepository _applicationActionRepository;
        private readonly Dictionary<string, RunningApplicationAction> _runningApplicationActions;

        public string Guid { get; set; }
        public Task Task { get; private set; }
        public ApplicationActionEnvironmentBase Env { get; set; }
        public RunningApplicationActionStatus Status { get; private set; }
        public ApplicationActionResult Result { get; private set; }

        public RunningApplicationAction(
            string guid,
            ApplicationActionRepository applicationActionRepository, 
            ApplicationAction action,
            RunActionParameter runActionParameter, 
            ApplicationActionEnvironmentBase env,
            Dictionary<string, RunningApplicationAction> runningApplicationActions
        )
        {
            Guid = guid;
            _applicationActionRepository = applicationActionRepository;
            Status = RunningApplicationActionStatus.Running;
            _runningApplicationActions = runningApplicationActions;
            Env = env;
            Task = new Task(() => RunAction(() => action.RunAction(env, runActionParameter)));
            Task.Start();
        }

        public RunningApplicationActionInfo GetInfo()
        {
            return new RunningApplicationActionInfo()
            {
                Guid = this.Guid,
                Status = this.Status
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
            _applicationActionRepository.SaveActionResult(Result);
            _runningApplicationActions.Remove(Guid);
        }
    }
}
