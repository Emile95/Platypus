namespace Core.Exceptions
{
    public class ApplicationActionInexistantException : Exception
    {
        public ApplicationActionInexistantException(string actionGuid)
            : base(Common.Utils.GetString("NoApplicationActionWithGuidException", actionGuid))
        {}
    }
}
