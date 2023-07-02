namespace Common.Exceptions
{
    public class PlatypusException : Exception
    {
        public PlatypusExceptionType ExceptionType { get; private set; }

        public PlatypusException(PlatypusExceptionType exceptionType, string message)
            : base(message)
        {
            ExceptionType = exceptionType;
        }
    }
}
