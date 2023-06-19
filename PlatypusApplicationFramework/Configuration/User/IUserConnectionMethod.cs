namespace PlatypusApplicationFramework.Configuration.User
{
    public interface IUserConnectionMethod
    {
        void Login(Dictionary<string, object> credential);
        string GetName();

        string GetDescription();
    }
}
