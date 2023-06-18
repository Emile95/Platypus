namespace PlatypusApplicationFramework.Configuration.User
{
    public abstract class UserCredentialMethod<ParameterType> : IUserCredentialMethod
        where ParameterType : class
    {
        protected ParameterType Parameter { get; set; }

        public void Login(Dictionary<string, object> Parameter)
        {
            LoginImplementation(Parameter as ParameterType);
        }

        protected abstract object LoginImplementation(ParameterType parameter);
    }
}
