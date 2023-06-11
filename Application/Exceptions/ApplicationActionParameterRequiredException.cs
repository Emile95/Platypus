namespace Core.Exceptions
{
    public class ApplicationActionParameterRequiredException : Exception
    {
        public ApplicationActionParameterRequiredException(string actionName)
            : base($"parameter are required for the action '{actionName}'")
        { }
    }
}
