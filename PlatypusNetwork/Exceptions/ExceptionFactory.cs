namespace PlatypusNetwork.Exceptions
{
    public class ExceptionFactory<ExceptionTypeEnum>
        where ExceptionTypeEnum : Enum
    {
        private Dictionary<ExceptionTypeEnum, Func<object[], FactorisableException<ExceptionTypeEnum>>> _factorisableExcptions;

        public ExceptionFactory()
        {
            _factorisableExcptions = new Dictionary<ExceptionTypeEnum, Func<object[], FactorisableException<ExceptionTypeEnum>>>();
        }

        public FactorisableException<ExceptionTypeEnum> CreateException(ExceptionTypeEnum type, params object[] parameters)
        {
            return _factorisableExcptions[type](parameters);
        }
    }
}
