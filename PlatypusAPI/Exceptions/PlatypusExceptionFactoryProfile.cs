using PlatypusAPI.User;
using PlatypusNetwork.Exceptions;

namespace PlatypusAPI.Exceptions
{
    public class PlatypusExceptionFactoryProfile : ExceptionFactoryProfile<FactorisableExceptionType>
    {
        public PlatypusExceptionFactoryProfile()
        {
            MapExceptionConstructor(FactorisableExceptionType.UserNotPermitted, (parameters) =>
            {
                return new UserPermissionException(parameters[0] as UserAccount);
            });

            MapExceptionConstructor(FactorisableExceptionType.UserConnectionFailed, (parameters) =>
            {
                return new UserConnectionFailedException(parameters[0] as string);
            });

            MapExceptionConstructor(FactorisableExceptionType.InvalidUserConnectionMethodGuid, (parameters) =>
            {
                return new InvalidUserConnectionMethodGuidException(parameters[0] as string);
            });

            MapExceptionConstructor(FactorisableExceptionType.ParameterEditorFieldRequired, (parameters) =>
            {
                return new ParameterEditorFieldRequiredException(parameters[0] as string);
            });
        }
    }
}
