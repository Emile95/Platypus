namespace PlatypusApplicationFramework.Configuration.User
{
    public interface IUserCredentialMethod
    {
        void Login(Dictionary<string, object> credential);
    }
}
