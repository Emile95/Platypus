namespace Core.Persistance.Entity
{
    public class RunningApplicationActionEntity
    {
        public string Guid { get; set; }
        public string ActionGuid { get; set; }
        public int RunNumber { get; set; }
        public Dictionary<string, object> ActionParameters { get; set; }
    }
}
