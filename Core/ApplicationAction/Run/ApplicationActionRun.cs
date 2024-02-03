using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.ApplicationAction;

namespace Core.ApplicationAction.Run
{
    public class ApplicationActionRun
    {
        public string ActionGuid { get; set; }
        public string Guid { get; set; }
        public ApplicationActionEnvironmentBase Env { get; set; }
        public ApplicationActionRunInfoStatus Status { get; private set; }
        public ApplicationActionRunResult Result { get; private set; }
        public Dictionary<string, object> Parameters;
        public Task Task { get; private set; }

        public void StartRun(ApplicationAction action, ApplicationActionRunParameter parameter, Action runCallBack)
        {
            Status = ApplicationActionRunInfoStatus.Running;
            Task = new Task(() => RunAction(() => action.Exec(Env), runCallBack));
            Task.Start();

            Result = new ApplicationActionRunResult();
            Parameters = parameter.ActionParameters;
        }

        public void Cancel()
        {
            Env.ActionCancelled = true;
        }

        private void RunAction(Func<ApplicationActionRunResult> action, Action runCallBack)
        {
            Result = action();
            switch(Result.Status)
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
