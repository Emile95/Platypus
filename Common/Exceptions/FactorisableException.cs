namespace Common.Exceptions
{
    public abstract class FactorisableException : Exception
    {
        public FactorisableExceptionType FactorisableExceptionType { get; private set; }

        public FactorisableException(FactorisableExceptionType factorisableExceptionType, string message)
            : base(message)
        {
            FactorisableExceptionType = factorisableExceptionType;
        }

        public abstract object[] GetParameters();
    }
}
