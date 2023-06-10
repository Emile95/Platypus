namespace Application.Exceptions
{
    public class ApplicationActionInexistantException : Exception
    {
        public ApplicationActionInexistantException(string actionName)
            : base($"there is no action named '{actionName}'")
        {}
    }
}
