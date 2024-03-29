﻿using Core.Persistance;
using Core.Persistance.Repository;
using PlatypusRepository.FolderPath.Json;
using Core.Persistance.Entity;
using Core.User;
using Core.Application;
using Core.ApplicationAction;
using Core.Event;
using Core;
using EntryPoint;
using PlatypusContainer;

IContainerBuilder builder = new ContainerBuilder();

builder.ConfigureServices((configure) => {
    configure.AddHostedService<ServerInstance>();

    DepencenyInjectionHelper.AddServiceInterfacesSharedReference(configure, new ApplicationRepository());
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference(configure, new ApplicationActionRepository());
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference(configure, new RunningApplicationActionRepository());
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference(configure, new JsonRepository<UserEntity>(ApplicationPaths.USERSDIRECTORYPATH));
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<ApplicationResolver>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<ApplicationUninstaller>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<ApplicationInstaller>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<EventsHandler>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<ApplicationsHandler>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<ApplicationActionsHandler>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<RunningApplicationActionsHandler>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<UserAuthentificationHandler>(configure);
    DepencenyInjectionHelper.AddServiceInterfacesSharedReference<UsersHandler>(configure);
});


IContainer container = builder.Build();
container.Run();