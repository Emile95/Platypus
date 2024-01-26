namespace Core.Exceptions
{
    public class ApplicationInexistantException : Exception
    {
        public ApplicationInexistantException(string applicationGuid)
            : base(PlatypusNetwork.Utils.GetString("NoApplicationWithGuid", applicationGuid))
        { }
    }
}
