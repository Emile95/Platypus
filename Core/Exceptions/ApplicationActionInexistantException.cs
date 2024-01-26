namespace Core.Exceptions
{
    public class ApplicationActionInexistantException : Exception
    {
        public ApplicationActionInexistantException(string actionGuid)
            : base(PlatypusNetwork.Utils.GetString("NoApplicationActionWithGuid", actionGuid))
        {}
    }
}
