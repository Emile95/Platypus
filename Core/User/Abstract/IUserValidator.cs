namespace Core.User.Abstract
{
    internal interface IUserValidator
    {
        bool Validate(string connectionMethod, Dictionary<string, object> userData);
    }
}
