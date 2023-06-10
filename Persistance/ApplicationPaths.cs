namespace Persistance
{
    public static class ApplicationPaths
    {
        public static string CONFIGFILEPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static string PLUGINSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        public static string APPLICATIONSDIRECTORYPATHS { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applications");
        public static string APPLICATIONDLLFILENAME { get; private set; } = "application.dll";
        public static string ACTIONSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "actions");

        public static string GetApplicationDllFilePath(string applicationGuid)
        {
            return Path.Combine(APPLICATIONSDIRECTORYPATHS, applicationGuid, APPLICATIONDLLFILENAME);
        }

        public static string GetActionDirectoryPath(string actionName)
        {
            return Path.Combine(ACTIONSDIRECTORYPATH, actionName);
        }

        public static string GetActionResultDirectoryPath(string actionName)
        {
            return Path.Combine(GetActionDirectoryPath(actionName), "results");
        }
    }
}
