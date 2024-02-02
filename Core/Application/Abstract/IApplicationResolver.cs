namespace Core.Application.Abstract
{
    internal interface IApplicationResolver<ApplicationType>
        where ApplicationType : class
    {
        void Resolve(ApplicationType application);
    }
}
