namespace Core.Exceptions
{
    public class ApplicationActionParameterRequiredException : Exception
    {
        public ApplicationActionParameterRequiredException(string actionName)
            : base(Common.Utils.GetString("ParameterRequieredForApplicationActionException", actionName))
        { }
    }
}
