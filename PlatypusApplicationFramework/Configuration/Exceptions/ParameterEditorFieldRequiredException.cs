namespace PlatypusApplicationFramework.Configuration.Exceptions
{
    public class ParameterEditorFieldRequiredException : Exception
    {
        public ParameterEditorFieldRequiredException(string fieldName)
            : base(Common.Utils.GetString("ParameterEditorFieldRequired", fieldName))
        { }
    }
}
