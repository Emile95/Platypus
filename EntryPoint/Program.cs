using Core.Persistance;
using Core.Persistance.Repository;
using Microsoft.Extensions.Hosting;
using PlatypusRepository.Json;
using PlatypusRepository;
using Core.Persistance.Entity;
using Microsoft.Extensions.DependencyInjection;
using Core.User.Abstract;
using Core.User;
using Core.Application.Abstract;
using Core.Application;
using Core.Abstract;
using PlatypusFramework.Configuration.Application;
using Core.ApplicationAction;
using PlatypusAPI.ApplicationAction;
using PlatypusFramework.Configuration.ApplicationAction;
using Core.ApplicationAction.Abstract;
using Core.ApplicationAction.Run;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusFramework.Configuration.User;
using PlatypusAPI.ServerFunctionParameter;
using Core.Event;
using PlatypusFramework.Configuration.Event;
using Core;
using Core.Event.Abstract;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((configure) => {
    configure.AddHostedService<ServerInstance>();

    ApplicationRepository applicationRepository = new ApplicationRepository();
    configure.AddSingleton<IRepositoryAddOperator<ApplicationEntity>>(applicationRepository);
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationEntity>>(applicationRepository);
    configure.AddSingleton<IRepositoryRemoveOperator<ApplicationEntity, string>>(applicationRepository);

    ApplicationActionRepository applicationActionRepository = new ApplicationActionRepository();
    configure.AddSingleton<IRepositoryAddOperator<ApplicationActionEntity>>(applicationActionRepository);
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionEntity>>(applicationActionRepository);
    configure.AddSingleton<IRepositoryRemoveOperator<ApplicationActionEntity, string>>(applicationActionRepository);

    RunningApplicationActionRepository runningApplicationActionRepository = new RunningApplicationActionRepository();
    configure.AddSingleton<IRepositoryAddOperator<RunningApplicationActionEntity>>(runningApplicationActionRepository);
    configure.AddSingleton<IRepositoryConsumeOperator<RunningApplicationActionEntity>>(runningApplicationActionRepository);
    configure.AddSingleton<IRepositoryRemoveOperator<RunningApplicationActionEntity, string>>(runningApplicationActionRepository);

    IRepository<UserEntity, string> userRepository = new JsonRepository<UserEntity>(ApplicationPaths.USERSDIRECTORYPATH);
    configure.AddSingleton(userRepository);
    configure.AddSingleton<IRepositoryAddOperator<UserEntity>>(userRepository);
    configure.AddSingleton<IRepositoryConsumeOperator<UserEntity>>(userRepository);
    configure.AddSingleton<IRepositoryUpdateOperator<UserEntity>>(userRepository);
    configure.AddSingleton<IRepositoryRemoveOperator<UserEntity, string>>(userRepository);

    configure.AddSingleton<IApplicationAttributeMethodResolver<EventHandlerAttribute>, EventsHandler>();
    configure.AddSingleton<IEventHandlerRunner, EventsHandler>();

    configure.AddSingleton<IApplicationResolver<PlatypusApplicationBase>, ApplicationResolver>();
    configure.AddSingleton<IApplicationUninstaller, ApplicationUninstaller>();
    configure.AddSingleton<IApplicationInstaller, ApplicationInstaller>();

    configure.AddSingleton<IRepositoryRemoveOperator<PlatypusApplicationBase, string>, ApplicationsHandler>();
    configure.AddSingleton<IRepositoryAddOperator<PlatypusApplicationBase>, ApplicationsHandler>();
    configure.AddSingleton<IGuidValidator<PlatypusApplicationBase>, ApplicationsHandler>();
    configure.AddSingleton<IGetterByGuid<PlatypusApplicationBase>, ApplicationsHandler>();

    configure.AddSingleton<IApplicationAttributeMethodResolver<ActionDefinitionAttribute>, ApplicationActionsHandler>();
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionInfo>, ApplicationActionsHandler>();
    configure.AddSingleton<IRepositoryRemoveOperator<ApplicationAction, string>, ApplicationActionsHandler>();
    configure.AddSingleton<IApplicationActionRunner, ApplicationActionsHandler>();

    configure.AddSingleton<IRepositoryRemoveOperator<ApplicationActionRun, string>, RunningApplicationActionsHandler>();
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionRun>, RunningApplicationActionsHandler>();
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionRunInfo>, RunningApplicationActionsHandler>();
    configure.AddSingleton<IRepositoryAddOperator<ApplicationActionRun>, RunningApplicationActionsHandler>();

    configure.AddSingleton<IUserAuthentificator, UserAuthentificationHandler>();
    configure.AddSingleton<IApplicationAttributeMethodResolver<UserConnectionMethodCreatorAttribute>, UserAuthentificationHandler>();
    configure.AddSingleton<IUserValidator, UserAuthentificationHandler>();

    configure.AddSingleton<IRepositoryAddOperator<UserCreationParameter>, UsersHandler>();
    configure.AddSingleton<IRepositoryUpdateOperator<UserUpdateParameter>, UsersHandler>();
    configure.AddSingleton<IRepositoryRemoveOperator<RemoveUserParameter, string>, UsersHandler>();
    configure.AddSingleton<IRepositoryConsumeOperator<UserEntity>, UsersHandler>();

    configure.AddSingleton<IServerStarter<ApplicationsHandler>, ApplicationsHandler>();
    configure.AddSingleton<IServerStarter<RunningApplicationActionsHandler>, RunningApplicationActionsHandler>();
});


IHost host = builder.Build();
host.Run();