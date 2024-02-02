namespace Core.Application.Abstract
{
    internal interface IApplicationPackageUninstaller<ApplicationIdentifier>
    {
        void Uninstall(ApplicationIdentifier identifier);
    }
}
