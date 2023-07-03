using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    public static class ExceptionFactory
    {
        public static FactorisableException CreateException(FactorisableExceptionType type, params object[] parameters)
        {
            switch(type)
            {
                case FactorisableExceptionType.UserConnectionFailed: return new UserConnectionFailedException(parameters[0] as string);
                case FactorisableExceptionType.InvalidUserConnectionMethodGuid: return new InvalidUserConnectionMethodGuidException(parameters[0] as string);
                case FactorisableExceptionType.ParameterEditorFieldRequired: return new ParameterEditorFieldRequiredException(parameters[0] as string);
            }

            return null;
        }
    }
}
