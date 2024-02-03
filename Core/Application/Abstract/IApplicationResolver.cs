namespace Core.Application.Abstract
{
    public interface IApplicationResolver<ApplicationType>
        where ApplicationType : class
    {
        void Resolve(ApplicationType application);
    }
}
