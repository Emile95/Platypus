using PlatypusFramework.Configuration;

namespace Core.User
{
    public class PlatypusUserCredential
    {
        [ParameterEditor(
            Name = "UserName",
            IsRequired = true)]
        public string UserName { get; set; }

        [ParameterEditor(
            Name = "Password",
            IsRequired = true)]
        public string Password { get; set; }
    }
}
