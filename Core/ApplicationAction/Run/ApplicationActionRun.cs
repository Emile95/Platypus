using PlatypusAPI.ApplicationAction;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;

namespace Core.ApplicationAction.Run
{
    public class ApplicationActionRun
    {
        public string ActionGuid { get; set; }
        public string Guid { get; set; }
        public int RunNumber { get; set; }
        public ApplicationActionEnvironmentBase Env { get; set; }
        public ApplicationActionRunInfoStatus Status { get; private set; }
        public ApplicationActionResult Result { get; private set; }
        public Task Task { get; private set; }

        public void StartRun(ApplicationAction action, ApplicationActionRunParameter parameter, Action<string> runCallBack)
        {
            Status = ApplicationActionRunInfoStatus.Running;
            Task = new Task(() => RunAction(() => action.RunAction(Env, parameter), runCallBack));
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

        private void RunAction(Func<ApplicationActionResult> action, Action<string> runCallBack)
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
            runCallBack(Guid);
        }
    }
}
