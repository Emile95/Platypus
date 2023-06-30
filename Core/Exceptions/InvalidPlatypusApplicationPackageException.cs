namespace Core.Exceptions
{
    public class InvalidPlatypusApplicationPackageException : Exception
    {
        public InvalidPlatypusApplicationPackageException(string packageFileName, string errorMessage)
            : base($"error with package {packageFileName}, message : '{errorMessage}'")
        {}
    }
}
