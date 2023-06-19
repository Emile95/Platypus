namespace Core.Exceptions
{
    public class ApplicationInexistantException : Exception
    {
        public ApplicationInexistantException(string applicationGuid)
            : base($"there is no application with guid '{applicationGuid}'")
        { }
    }
}
