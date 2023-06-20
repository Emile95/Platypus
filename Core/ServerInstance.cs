using Common.Logger;
using Core.Application;
using Core.ApplicationAction;
using Core.ApplicationAction.Run;
using Core.Event;
using Core.SocketHandler;
using Core.User;
using Logging;
using Persistance.Repository;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusApplicationFramework.Core.ApplicationAction;

namespace Core
{
    public class ServerInstance
    {
        private readonly ApplicationsHandler _applicationsHandler;
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly EventsHandler _eventsHandlers;
        private readonly UsersHandler _usersHandler;

        private readonly ApplicationRepository _applicationRepository;
        private readonly LoggerManager _loggerManager;

        private readonly PlatypusSocketsHandler _socketsHandler;


        public ServerInstance()
        {
            ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();

            _eventsHandlers = new EventsHandler();

            _applicationActionsHandler = new ApplicationActionsHandler(
                applicationActionRepository,
                _eventsHandlers
            );

            UserRepository userRepository = new UserRepository();
            _usersHandler = new UsersHandler(
                userRepository
             );

            _usersHandler.AddDefaultCredentialMethod(new PlatypusUserConnectionMethod(userRepository), "Default-PlatypusUser");

            _applicationRepository = new ApplicationRepository();

            ApplicationInstaller applicationInstaller = new ApplicationInstaller(
                _applicationRepository,
                applicationActionRepository,
                userRepository
            );

            ApplicationResolver applicationResolver = new ApplicationResolver(
                _applicationRepository,
                _applicationActionsHandler,
                _eventsHandlers,
                _usersHandler
            );

            _applicationsHandler = new ApplicationsHandler(
                _applicationRepository,
                applicationResolver,
                applicationInstaller
            );

            _loggerManager = new LoggerManager();
            _loggerManager.CreateLogger<PlatypusServerConsoleLogger>();

            _socketsHandler = new PlatypusSocketsHandler(
                _applicationActionsHandler
            );
        }

        public void LoadConfiguration()
        {

        }

        public void LoadApplications()
        {
            _applicationsHandler.LoadApplications();
        }

        public void InstallApplication(string dllFilePath)
        {
            _applicationsHandler.InstallApplication(dllFilePath);
        }

        public void UninstalApplication(string applicationGuid)
        {
            UninstallApplicationDetails details = _applicationsHandler.UninstallApplication(applicationGuid);
            foreach(string actionGuid in details.ActionGuids)
                _applicationActionsHandler.RemoveAction(actionGuid);
            foreach (string userConnectionMethodGuid in details.UserConnectionMethodGuids)
                _usersHandler.RemoveCredentialMethod(userConnectionMethodGuid);
        }

        public void InitializeServerSocketHandlers()
        {
            _socketsHandler.InitializeSocketHandlers();
        }

        public ApplicationActionRunResult RunAction(ApplicationActionRunParameter runActionParameter)
        {
            
            if (_applicationActionsHandler.HasActionWithGuid(runActionParameter.Guid) == false)
            {
                string message = String.Format(Strings.ResourceManager.GetString("ApplicationActionNotFound"), runActionParameter.Guid);
                return new ApplicationActionRunResult()
                {
                    Message = message,
                    Status = ApplicationActionRunResultStatus.Failed,
                };
            }
                
            
            ApplicationActionEnvironmentBase env = _applicationActionsHandler.CreateStartActionEnvironment(runActionParameter.Guid);
            env.ApplicationRepository = _applicationRepository;
            env.ActionLoggers = new LoggerManager();

            return _applicationActionsHandler.RunAction(runActionParameter, env);
        }

        public void CancelRunningApplicationAction(string guid)
        {
            _applicationActionsHandler.CancelRunningAction(guid);
        }

        public IEnumerable<ApplicationActionRunInfo> GetRunningApplicationActions()
        {
            return _applicationActionsHandler.GetRunningApplicationActionInfos();
        }
    }
}
