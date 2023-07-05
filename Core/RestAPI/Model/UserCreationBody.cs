namespace Core.RestAPI.Model
{
    public class UserCreationBody
    {
        public string ConnectionMethodGUID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
