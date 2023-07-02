using Common.Exceptions;

namespace PlatypusAPI.Exceptions
{
    public static class ExceptionFactory
    {
        public static PlatypusException CreateException(PlatypusExceptionType type, string parameter)
        {
            switch(type)
            {
                case PlatypusExceptionType.UserConnectionFailed: return new UserConnectionFailedException(parameter);
                case PlatypusExceptionType.InvalidUserConnectionMethodGuid: return new InvalidUserConnectionMethodGuidException(parameter);
                case PlatypusExceptionType.ParameterEditorFieldRequired: return new ParameterEditorFieldRequiredException(parameter);
            }

            return null;
        }
    }
}
