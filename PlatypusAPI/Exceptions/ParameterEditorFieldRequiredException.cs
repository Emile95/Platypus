using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    [Serializable]
    public class ParameterEditorFieldRequiredException : PlatypusException
    {
        public ParameterEditorFieldRequiredException(string fieldName)
            : base(PlatypusExceptionType.ParameterEditorFieldRequired, Common.Utils.GetString("ParameterEditorFieldRequired", fieldName))
        { }
    }
}
