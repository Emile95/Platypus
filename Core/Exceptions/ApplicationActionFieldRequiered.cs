namespace Core.Exceptions
{
    public class ApplicationActionFieldRequired : Exception
    {
        public ApplicationActionFieldRequired(string fieldName)
            : base(PlatypusNetwork.Utils.GetString("ApplicationActionFieldRequired", fieldName))
        { }
    }
}
