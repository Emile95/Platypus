using Core.Event.Abstract;
using Core.Persistance.Entity;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Core.Event;
using PlatypusRepository;

namespace Core.ApplicationAction
{
    public class RunningApplicationActionsHandler :
        IRepositoryRemoveOperator<ApplicationActionRun, string>,
        IRepositoryConsumeOperator<ApplicationActionRunInfo>,
        IRepositoryConsumeOperator<ApplicationActionRun>,
        IRepositoryAddOperator<ApplicationActionRun>
    {
        private readonly Dictionary<string, ApplicationActionRun> _applicationActionRuns;
        private readonly IRepositoryRemoveOperator<RunningApplicationActionEntity, string> _runningApplicationActionEntityRemoveOperator;
        private readonly IRepositoryAddOperator<RunningApplicationActionEntity> _runningApplicationActionEntityAddOperator;
        private readonly IEventHandlerRunner _eventhHandlerRunner;

        public RunningApplicationActionsHandler(
            IRepositoryRemoveOperator<RunningApplicationActionEntity, string> runningApplicationActionEntityRemoveOperator, 
            IRepositoryAddOperator<RunningApplicationActionEntity> runningApplicationActionEntityAddOperator,
            IEventHandlerRunner eventhHandlerRunner
        )
        {
            _applicationActionRuns = new Dictionary<string, ApplicationActionRun>();
            _runningApplicationActionEntityRemoveOperator = runningApplicationActionEntityRemoveOperator;
            _runningApplicationActionEntityAddOperator = runningApplicationActionEntityAddOperator;
            _eventhHandlerRunner = eventhHandlerRunner;
        }

        public ApplicationActionRun Add(ApplicationActionRun actionRun)
        {
            string runningActionGuid = actionRun.GetRunningActionGuid();
            RunningApplicationActionEntity entity = _runningApplicationActionEntityAddOperator.Add(new RunningApplicationActionEntity()
            {
                Guid = runningActionGuid,
                ActionGuid = actionRun.ActionGuid,
                ActionRunGuid = actionRun.Guid,
                Parameters = actionRun.Parameters
            });
            _applicationActionRuns.Add(runningActionGuid, actionRun);
            return actionRun;
        }

        public void Consume(Action<ApplicationActionRunInfo> consumer, Predicate<ApplicationActionRunInfo> condition = null)
        {
            foreach(ApplicationActionRun run in _applicationActionRuns.Values) 
            {
                ApplicationActionRunInfo applicationActionRunInfo = new ApplicationActionRunInfo()
                {
                    Guid = run.Guid,
                };
                if (condition == null) consumer(applicationActionRunInfo);
                else
                    if (condition(applicationActionRunInfo))
                        consumer(applicationActionRunInfo);
            };
        }

        public void Consume(Action<ApplicationActionRun> consumer, Predicate<ApplicationActionRun> condition = null)
        {
            foreach (ApplicationActionRun run in _applicationActionRuns.Values)
            {
                if (condition == null) consumer(run);
                else
                    if (condition(run))
                        consumer(run);
            };
        }

        public void Remove(string id)
        {
            if (_applicationActionRuns.ContainsKey(id) == false) throw new Exception($"No action with guid '{id}' is running");

            CancelRunningActionEventHandlerEnvironment eventEnv = new CancelRunningActionEventHandlerEnvironment()
            {
                RunningActionGuid = id
            };

            _eventhHandlerRunner.Run<object>(EventHandlerType.BeforeCancelApplicationRun, eventEnv, (exception) => throw exception);

            ApplicationActionRun run = _applicationActionRuns[id];
            run.Cancel();

            _applicationActionRuns.Remove(id);
            _runningApplicationActionEntityRemoveOperator.Remove(id);

            _eventhHandlerRunner.Run<object>(EventHandlerType.AfterCancelApplicationRun, eventEnv, (exception) => throw exception);
        }
    }
}
