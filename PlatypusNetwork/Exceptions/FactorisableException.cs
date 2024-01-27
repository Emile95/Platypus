namespace PlatypusNetwork.Exceptions
{
    public abstract class FactorisableException<ExceptionTypeEnum> : Exception
        where ExceptionTypeEnum : Enum
    {
        public ExceptionTypeEnum FactorisableExceptionType { get; private set; }

        public FactorisableException(ExceptionTypeEnum factorisableExceptionType, string message)
            : base(message)
        {
            FactorisableExceptionType = factorisableExceptionType;
        }

        public abstract object[] GetParameters();
    }
}
