using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    [Serializable]
    public class ParameterEditorFieldRequiredException : FactorisableException
    {
        public string FieldName { get; private set; }

        public ParameterEditorFieldRequiredException(string fieldName)
            : base(FactorisableExceptionType.ParameterEditorFieldRequired, Common.Utils.GetString("ParameterEditorFieldRequired", fieldName))
        {
            FieldName = fieldName;
        }

        public override object[] GetParameters()
        {
            return new object[] { FieldName };
        }
    }
}
