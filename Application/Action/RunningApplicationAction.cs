using Persistance;
using PlatypusApplicationFramework.Action;

namespace Application.Action
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

        public void Cancel()
        {
            Status = RunningApplicationActionStatus.Cancelled;
            Env.ActionCancelled = true;
        }

        private void RunAction(Func<ApplicationActionResult> action)
        {
            try
            {
                Result = action();
                if(Status != RunningApplicationActionStatus.Cancelled)
                    Status = RunningApplicationActionStatus.Finish;
            }
            catch (Exception ex)
            {
                Result = new ApplicationActionResult()
                {
                    Status = ApplicationActionResultStatus.Failed,
                    Message = ex.Message
                };
                Status = RunningApplicationActionStatus.Aborted;
            }

            _applicationActionRepository.SaveActionResult(Result);
            _runningApplicationActions.Remove(Guid);
        }
    }
}
