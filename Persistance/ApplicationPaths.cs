﻿namespace Persistance
{
    public static class ApplicationPaths
    {
        public static string CONFIGFILEPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public static string APPLICATIONSDIRECTORYPATHS { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applications");
        public static string APPLICATIONCONFIGFILENAME { get; private set; } = "config.json";
        public static string APPLICATIONDLLFILENAME { get; private set; } = "application.dll";
        public static string ACTIONSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "actions");
        public static string ACTIONRUNSDIRECTORYNAME { get; private set; } = "runs";
        public static string ACTIONLASTRUNNUMBERFILENIME { get; private set; } = "lastRunNumber";
        public static string ACTIONLOGFILENAME { get; private set; } = "action.log";
        public static string USERSDIRECTORYPATH { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users");
        public static string PLATYPUSUSERSDIRECTORYPATH { get; private set; } = Path.Combine(USERSDIRECTORYPATH, "platypusUsers");
        public static string LASTPLATYPUSUSERIDFILEPATH { get; private set; } = Path.Combine(PLATYPUSUSERSDIRECTORYPATH, "lastID");
        public static string USERFILENAME { get; private set; } = "user.json";
        public static string USERCREDENTIALMETHODSDIRECTORYPATH { get; private set; } = Path.Combine(USERSDIRECTORYPATH, "connectionMethods");
        public static string DEFINITIONFILENAME { get; private set; } = "definition.json";

        public static string GetApplicationDirectoryPath(string applicationGuid)
        {
            return Path.Combine(APPLICATIONSDIRECTORYPATHS, applicationGuid);
        }

        public static string GetApplicationDllFilePath(string applicationGuid)
        {
            return Path.Combine(GetApplicationDirectoryPath(applicationGuid), APPLICATIONDLLFILENAME);
        }

        public static string GetApplicationDllFilePathByBasePath(string basePath)
        {
            return Path.Combine(basePath, APPLICATIONDLLFILENAME);
        }

        public static string GetApplicationConfigFilePath(string applicationGuid)
        {
            return Path.Combine(GetApplicationDirectoryPath(applicationGuid), APPLICATIONCONFIGFILENAME);
        }

        public static string GetApplicationConfigFilePathByBasePath(string basePath)
        {
            return Path.Combine(basePath, APPLICATIONCONFIGFILENAME);
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

        public static string GetActionRunLogFilePath(string actionGuid, int buildNumber)
        {
            return Path.Combine(GetActionRunsDirectoryPath(actionGuid), buildNumber.ToString(), ACTIONLOGFILENAME);
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

        public static string GetUserDirectoryPath(int userID)
        {
            return Path.Combine(Path.Combine(USERSDIRECTORYPATH, userID.ToString()));
        }

        public static string GetUserFilePath(int userID)
        {
            return Path.Combine(Path.Combine(USERSDIRECTORYPATH, userID.ToString(), USERFILENAME));
        }

        public static string GetUserFilePathByBasePath(string basePath)
        {
            return Path.Combine(Path.Combine(basePath, USERFILENAME));
        }

        public static string GetPlatypusUserDirectoryPath(int userID)
        {
            return Path.Combine(PLATYPUSUSERSDIRECTORYPATH, userID.ToString());
        }
    }
}
