namespace Core.Exceptions
{
    public class InvalidPlatypusApplicationPackageException : Exception
    {
        public InvalidPlatypusApplicationPackageException(string packageFileName, string errorMessage)
            : base(PlatypusNetwork.Utils.GetString("ErrorWithPlatypusApplicationPackage", packageFileName, errorMessage))
        {}
    }
}
