namespace Core.User.Abstract
{
    public interface IUserValidator
    {
        bool Validate(string connectionMethod, Dictionary<string, object> userData);
    }
}
