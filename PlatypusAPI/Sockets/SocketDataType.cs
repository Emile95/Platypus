namespace PlatypusAPI.Sockets
{
    public enum SocketDataType
    {
        UserConnection,
        RunApplicationAction,
        AddUser,
        UpdateUser,
        RemoveUser,
        CancelRunningAction,
        GetRunningActions,
        GetActionInfos
    }
}
