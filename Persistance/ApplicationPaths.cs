namespace Persistance
{
    public static class ApplicationPaths
    {
        public static string CONFIGFILEPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public static string APPLICATIONSDIRECTORYPATHS { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applications");
        public static string APPLICATIONDLLFILENAME { get; private set; } = "application.dll";
        public static string ACTIONSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "actions");
        public static string ACTIONRUNSDIRECTORYNAME { get; private set; } = "runs";
        public static string ACTIONLASTRUNNUMBERFILENIME { get; private set; } = "lastRunNumber";
        public static string ACTIONLOGFILENAME { get; private set; } = "action.log";

        public static string GetApplicationDllFilePath(string applicationGuid)
        {
            return Path.Combine(APPLICATIONSDIRECTORYPATHS, applicationGuid, APPLICATIONDLLFILENAME);
        }

        public static string GetActionDirectoryPath(string actionGuid)
        {
            return Path.Combine(ACTIONSDIRECTORYPATH, actionGuid);
        }

        public static string GetActionDirectoryPathByBasePath(string basePath, string actionGuid)
        {
            return Path.Combine(basePath, actionGuid);
        }

        public static string GetActionRunsDirectoryPath(string actionGuid)
        {
            return Path.Combine(GetActionDirectoryPath(actionGuid), ACTIONRUNSDIRECTORYNAME);
        }

        public static string GetActionRunsDirectoryPathByBasePath(string basePath)
        {
            return Path.Combine(basePath, ACTIONRUNSDIRECTORYNAME);
        }

        public static string GetActionRunDirectoryPath(string actionGuid, int buildNumber)
        {
            return Path.Combine(GetActionRunsDirectoryPath(actionGuid), buildNumber.ToString());
        }

        public static string GetActionRunDirectoryPathByBasePath(string basePath, string actionGuid, int buildNumber)
        {
            return Path.Combine(basePath, actionGuid, buildNumber.ToString());
        }

        public static string GetActionLastRunNumberFilePath(string actionGuid)
        {
            return Path.Combine(GetActionRunsDirectoryPath(actionGuid), ACTIONLASTRUNNUMBERFILENIME);
        }

        public static string GetActionLastRunNumberFilePathByBasePath(string basePath)
        {
            return Path.Combine(basePath, ACTIONLASTRUNNUMBERFILENIME);
        }
    }
}
