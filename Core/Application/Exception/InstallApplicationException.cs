namespace Core.Application.Exception
{
    internal class InstallApplicationException : ApplicationException
    {
        public InstallApplicationException(string message) 
            : base(message) { }
    }
}
