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

    configure.AddSingleton<IApplicationResolver<PlatypusApplicationBase>, ApplicationResolver>();
    configure.AddSingleton<IApplicationUninstaller, ApplicationUninstaller>();
    configure.AddSingleton<IApplicationInstaller, ApplicationInstaller>();

    configure.AddSingleton<EventsHandler>();
    configure.AddSingleton<IApplicationAttributeMethodResolver<EventHandlerAttribute>>(provider => provider.GetRequiredService<EventsHandler>());
    configure.AddSingleton<IEventHandlerRunner>(provider => provider.GetRequiredService<EventsHandler>());

    configure.AddSingleton<ApplicationsHandler>();
    configure.AddSingleton<IRepositoryRemoveOperator<PlatypusApplicationBase, string>>(provider => provider.GetRequiredService<ApplicationsHandler>());
    configure.AddSingleton<IRepositoryAddOperator<PlatypusApplicationBase>>(provider => provider.GetRequiredService<ApplicationsHandler>());
    configure.AddSingleton<IGuidValidator<PlatypusApplicationBase>>(provider => provider.GetRequiredService<ApplicationsHandler>());
    configure.AddSingleton<IGetterByGuid<PlatypusApplicationBase>>(provider => provider.GetRequiredService<ApplicationsHandler>());
    configure.AddSingleton<IServerStarter<ApplicationsHandler>>(provider => provider.GetRequiredService<ApplicationsHandler>());

    configure.AddSingleton<ApplicationActionsHandler>();
    configure.AddSingleton<IApplicationAttributeMethodResolver<ActionDefinitionAttribute>>(provider => provider.GetRequiredService<ApplicationActionsHandler>());
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionInfo>>(provider => provider.GetRequiredService<ApplicationActionsHandler>());
    configure.AddSingleton<IRepositoryRemoveOperator<ApplicationAction, string>>(provider => provider.GetRequiredService<ApplicationActionsHandler>());
    configure.AddSingleton<IApplicationActionRunner>(provider => provider.GetRequiredService<ApplicationActionsHandler>());

    configure.AddSingleton<RunningApplicationActionsHandler>();
    configure.AddSingleton<IRepositoryRemoveOperator<ApplicationActionRun, string>>(provider => provider.GetRequiredService<RunningApplicationActionsHandler>());
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionRun>>(provider => provider.GetRequiredService<RunningApplicationActionsHandler>());
    configure.AddSingleton<IRepositoryConsumeOperator<ApplicationActionRunInfo>>(provider => provider.GetRequiredService<RunningApplicationActionsHandler>());
    configure.AddSingleton<IRepositoryAddOperator<ApplicationActionRun>>(provider => provider.GetRequiredService<RunningApplicationActionsHandler>());
    configure.AddSingleton<IServerStarter<RunningApplicationActionsHandler>>(provider => provider.GetRequiredService<RunningApplicationActionsHandler>());

    configure.AddSingleton<UserAuthentificationHandler>();
    configure.AddSingleton<IUserAuthentificator>(provider => provider.GetRequiredService<UserAuthentificationHandler>());
    configure.AddSingleton<IApplicationAttributeMethodResolver<UserConnectionMethodCreatorAttribute>>(provider => provider.GetRequiredService<UserAuthentificationHandler>());
    configure.AddSingleton<IUserValidator>(provider => provider.GetRequiredService<UserAuthentificationHandler>());

    configure.AddSingleton<UsersHandler>();
    configure.AddSingleton<IRepositoryAddOperator<UserCreationParameter>>(provider => provider.GetRequiredService<UsersHandler>());
    configure.AddSingleton<IRepositoryUpdateOperator<UserUpdateParameter>>(provider => provider.GetRequiredService<UsersHandler>());
    configure.AddSingleton<IRepositoryRemoveOperator<RemoveUserParameter, string>>(provider => provider.GetRequiredService<UsersHandler>());
    configure.AddSingleton<IRepositoryConsumeOperator<UserEntity>>(provider => provider.GetRequiredService<UsersHandler>());
});


IHost host = builder.Build();
host.Run();