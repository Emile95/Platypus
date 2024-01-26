namespace PlatypusAPI.Exceptions
{
    [Serializable]
    public class ParameterEditorFieldRequiredException : FactorisableException
    {
        public string FieldName { get; private set; }

        public ParameterEditorFieldRequiredException(string fieldName)
            : base(FactorisableExceptionType.ParameterEditorFieldRequired, PlatypusNetwork.Utils.GetString("ParameterEditorFieldRequired", fieldName))
        {
            FieldName = fieldName;
        }

        public override object[] GetParameters()
        {
            return new object[] { FieldName };
        }
    }
}
