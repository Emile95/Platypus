namespace EntryPoint.Model
{
    public class UserConnection
    {
        public string ConnectionMethodGuid { get; set; }
        public Dictionary<string, object> Credential { get; set; }
    }
}
