using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.ApplicationAction;

namespace Core.ApplicationAction
{
    public class ApplicationActionRun
    {
        public string ActionGuid { get; set; }
        public string Guid { get; set; }
        public ApplicationActionEnvironmentBase Env { get; set; }
        public ApplicationActionRunInfoStatus Status { get; private set; }
        public ApplicationActionRunResult Result { get; private set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Task Task { get; private set; }
        public Func<ApplicationActionEnvironmentBase, ApplicationActionRunResult> Action { get; set; }

        public string GetRunningActionGuid()
        {
            return ActionGuid + "-" + Guid;
        }

        public void StartRun(Action runCallBack)
        {
            Status = ApplicationActionRunInfoStatus.Running;
            Task = new Task(() => RunAction(() => Action.Invoke(Env), runCallBack));
            Task.Start();
            Result = new ApplicationActionRunResult();
        }

        public void Cancel()
        {
            Env.ActionCancelled = true;
        }

        private void RunAction(Func<ApplicationActionRunResult> action, Action runCallBack)
        {
            Result = action();
            switch (Result.Status)
            {
                case ApplicationActionRunResultStatus.Success:
                    Status = ApplicationActionRunInfoStatus.Finish;
                    break;
                case ApplicationActionRunResultStatus.Failed:
                case ApplicationActionRunResultStatus.Canceled:
                    Status = ApplicationActionRunInfoStatus.Aborted;
                    break;
            }
            runCallBack();
        }
    }
}
