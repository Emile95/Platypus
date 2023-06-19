using PlatypusApplicationFramework.Confugration;

namespace PlatypusApplicationFramework.Configuration.User
{
    public abstract class UserConnectionMethod<ParameterType> : IUserConnectionMethod
        where ParameterType : class, new()
    {
        public void Login(Dictionary<string, object> credential)
        {
            ParameterType parameterObject = ParameterEditorObjectResolver.ResolveByDictionnary<ParameterType>(credential);
            LoginImplementation(parameterObject);
        }

        public abstract string GetName();

        public virtual string GetDescription() { return ""; }

        protected abstract void LoginImplementation(ParameterType credential);
    }
}
