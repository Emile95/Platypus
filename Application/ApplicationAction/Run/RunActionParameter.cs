namespace Application.ApplicationAction.Run
{
    public class RunActionParameter
    {
        public string Guid { get; set; }
        public Dictionary<string, object> ActionParameters { get; set; }
        public bool Async { get; set; }
    }
}
