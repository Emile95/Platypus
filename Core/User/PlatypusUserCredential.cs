using PlatypusApplicationFramework.Configuration;

namespace Core.User
{
    public class PlatypusUserCredential
    {
        [ParameterEditor(
            Name = "User",
            IsRequired = true)]
        public string User { get; set; }

        [ParameterEditor(
            Name = "Password",
            IsRequired = true)]
        public string Password { get; set; }
    }
}
