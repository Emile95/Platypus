namespace Application.Exceptions
{
    public class ApplicationActionInexistantException : Exception
    {
        public ApplicationActionInexistantException(string actionGuid)
            : base($"there is no action with guid '{actionGuid}'")
        {}
    }
}
