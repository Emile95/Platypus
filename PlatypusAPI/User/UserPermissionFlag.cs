namespace PlatypusAPI.User
{
    public enum UserPermissionFlag
    {
        None,
        Admin,
        InstallAndUninstallApplication,
        AddUser,
        CancelRunningAction,
        GetActionsInfo,
        GetRunningActions,
        RunAction
    }
}
