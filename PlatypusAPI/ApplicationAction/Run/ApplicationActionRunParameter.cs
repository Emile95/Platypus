namespace PlatypusAPI.ApplicationAction.Run
{
    public class ApplicationActionRunParameter
    {
        public string Guid { get; set; }
        public Dictionary<string, object> ActionParameters { get; set; }
        public bool Async { get; set; }
    }
}
