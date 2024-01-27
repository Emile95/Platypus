namespace PlatypusNetwork.Exceptions
{
    public class ExceptionFactoryProfile<ExceptionTypeEnum>
        where ExceptionTypeEnum : Enum
    {
        public Dictionary<ExceptionTypeEnum, Func<object[], FactorisableException<ExceptionTypeEnum>>> FactorisableExecptions;

        public ExceptionFactoryProfile()
        {
            FactorisableExecptions = new Dictionary<ExceptionTypeEnum, Func<object[], FactorisableException<ExceptionTypeEnum>>>();
        }

        public void MapExceptionConstructor(ExceptionTypeEnum exceptionType, Func<object[], FactorisableException<ExceptionTypeEnum>> construcor)
        {
            FactorisableExecptions.Add(exceptionType, construcor);
        }
    }
}
