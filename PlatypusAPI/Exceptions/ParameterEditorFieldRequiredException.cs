using PlatypusAPI.Ressources;
using PlatypusNetwork.Exceptions;
using PlatypusUtils;

namespace PlatypusAPI.Exceptions
{
    [Serializable]
    public class ParameterEditorFieldRequiredException : FactorisableException<FactorisableExceptionType>
    {
        public string FieldName { get; private set; }

        public ParameterEditorFieldRequiredException(string fieldName)
            : base(FactorisableExceptionType.ParameterEditorFieldRequired, Utils.GetString(Strings.ResourceManager, "ParameterEditorFieldRequired", fieldName))
        {
            FieldName = fieldName;
        }

        public override object[] GetParameters()
        {
            return new object[] { FieldName };
        }
    }
}
