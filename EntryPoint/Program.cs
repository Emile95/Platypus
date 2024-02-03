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

    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance(configure, new ApplicationRepository());
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance(configure, new ApplicationActionRepository());
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance(configure, new RunningApplicationActionRepository());
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance(configure, new JsonRepository<UserEntity>(ApplicationPaths.USERSDIRECTORYPATH));
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<ApplicationResolver>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<ApplicationUninstaller>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<ApplicationInstaller>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<EventsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<ApplicationsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<ApplicationActionsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<RunningApplicationActionsHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<UserAuthentificationHandler>(configure);
    DepencenyInjectionHelper.InjectAllInterfacesFromTypeWithSameInstance<UsersHandler>(configure);
});


IHost host = builder.Build();
host.Run();