using PlatypusApplicationFramework.Confugration;

namespace PlatypusApplicationFramework.Configuration.User
{
    public abstract class UserCredentialMethod<ParameterType> : IUserCredentialMethod
        where ParameterType : class, new()
    {
        public void Login(Dictionary<string, object> parameter)
        {
            ParameterType parameterObject = ParameterEditorObjectResolver.ResolveByDictionnary<ParameterType>(parameter);
            LoginImplementation(parameterObject);
        }

        protected abstract void LoginImplementation(ParameterType parameter);
    }
}
