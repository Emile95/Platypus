using Core;

ServerInstance serverInstance = new ServerInstance();
serverInstance.LoadConfiguration();
serverInstance.LoadApplications();
serverInstance.InitializeServerSocketHandlers();
serverInstance.InitializeRestAPIHandler(args);