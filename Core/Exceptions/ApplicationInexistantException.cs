namespace Core.Exceptions
{
    public class ApplicationInexistantException : Exception
    {
        public ApplicationInexistantException(string applicationGuid)
            : base(Common.Utils.GetString("NoApplicationWithGuid", applicationGuid))
        { }
    }
}
