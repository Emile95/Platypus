namespace PlatypusApplicationFramework.Configuration.Exceptions
{
    public class ParameterEditorFieldRequiredException : Exception
    {
        public ParameterEditorFieldRequiredException(string fieldName)
            : base($"the field '{fieldName}' is required")
        { }
    }
}
