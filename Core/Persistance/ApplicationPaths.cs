namespace Core.Persistance
{
    public static class ApplicationPaths
    {
        public static string CONFIGFILEPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public static string APPLICATIONSDIRECTORYPATHS { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applications");
        public static string ACTIONSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "actions");
        public static string ACTIONRUNSDIRECTORYNAME { get; private set; } = "runs";
        public static string RUNNINGACTIONSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runnings");
        public static string USERSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users");


        public static string GetActionDirectoryPath(string actionGuid)
        {
            return Path.Combine(ACTIONSDIRECTORYPATH, actionGuid);
        }

        public static string GetActionRunsDirectoryPath(string actionGuid)
        {
            return Path.Combine(GetActionDirectoryPath(actionGuid), ACTIONRUNSDIRECTORYNAME);
        }
    }
}
