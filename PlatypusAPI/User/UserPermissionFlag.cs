namespace PlatypusAPI.User
{
    [Flags]
    public enum UserPermissionFlag
    {
        Admin = 0,
        InstallAndUninstallApplication = 1,
        UserCRUD = 2,
        CancelRunningAction = 4,
        GetActionsInfo = 8,
        GetRunningActions = 16,
        RunAction = 32,
        GetApplicationInfos = 64
    }
}
