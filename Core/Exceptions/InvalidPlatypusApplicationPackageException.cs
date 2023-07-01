namespace Core.Exceptions
{
    public class InvalidPlatypusApplicationPackageException : Exception
    {
        public InvalidPlatypusApplicationPackageException(string packageFileName, string errorMessage)
            : base(Common.Utils.GetString("ErrorWithPlatypusApplicationPackage", packageFileName, errorMessage))
        {}
    }
}
