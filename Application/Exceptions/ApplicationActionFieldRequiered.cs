namespace Application.Exceptions
{
    public class ApplicationActionFieldRequired : Exception
    {
        public ApplicationActionFieldRequired(string fieldName)
            : base($"the field '{fieldName}' is required in the action parameter")
        { }
    }
}
