using Core.Ressource;
using PlatypusUtils;

namespace Core.Exceptions
{
    public class InvalidPlatypusApplicationPackageException : Exception
    {
        public InvalidPlatypusApplicationPackageException(string packageFileName, string errorMessage)
            : base(Utils.GetString(Strings.ResourceManager, "ErrorWithPlatypusApplicationPackage", packageFileName, errorMessage))
        {}
    }
}
