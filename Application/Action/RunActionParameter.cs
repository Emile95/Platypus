namespace Application.Action
{
    public class RunActionParameter
    {
        public string Name { get; set; }
        public Dictionary<string, object> ActionParameters { get; set; }
        public bool Async { get; set; }
    }
}
