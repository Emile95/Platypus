namespace Core.Exceptions
{
    public class ApplicationActionFieldRequired : Exception
    {
        public ApplicationActionFieldRequired(string fieldName)
            : base(Common.Utils.GetString("ApplicationActionFieldRequired", fieldName))
        { }
    }
}
