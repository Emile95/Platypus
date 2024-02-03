using Core.Persistance;
using Core.Persistance.Repository;
using Microsoft.Extensions.Hosting;
using PlatypusRepository.Json;
using Core.Persistance.Entity;
using Microsoft.Extensions.DependencyInjection;
using Core.User;
using Core.Application;
using Core.ApplicationAction;
using Core.Event;
using Core;
using EntryPoint;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((configure) => {
    configure.AddHostedService<ServerInstance>();

    DepencenyInjectionHelper.InjectAllInterfacesFromType(configure, new ApplicationRepository());
    DepencenyInjectionHelper.InjectAllInterfacesFromType(configure, new ApplicationActionRepository());
    DepencenyInjectionHelper.InjectAllInterfacesFromType(configure, new RunningApplicationActionRepository());
    DepencenyInjectionHelper.InjectAllInterfacesFromType(configure, new JsonRepository<UserEntity>(ApplicationPaths.USERSDIRECTORYPATH));
    DepencenyInjectionHelper.InjectAllInterfacesFromType<ApplicationResolver>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<ApplicationUninstaller>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<ApplicationInstaller>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<EventsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<ApplicationsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<ApplicationActionsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<RunningApplicationActionsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<UserAuthentificationHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromType<UsersHandler>(configure);
});


IHost host = builder.Build();
host.Run();