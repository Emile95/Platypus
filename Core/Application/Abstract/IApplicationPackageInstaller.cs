namespace Core.Application.Abstract
{
    internal interface IApplicationPackageInstaller<ApplicationType>
        where ApplicationType : class
    {
        ApplicationType Install(string sourcePath);
        
    }
}
