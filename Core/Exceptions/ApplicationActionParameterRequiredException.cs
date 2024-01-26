namespace Core.Exceptions
{
    public class ApplicationActionParameterRequiredException : Exception
    {
        public ApplicationActionParameterRequiredException(string actionName)
            : base(PlatypusNetwork.Utils.GetString("ParameterRequieredForApplicationAction", actionName))
        { }
    }
}
