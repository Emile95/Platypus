using PlatypusAPI.ApplicationAction.Run;

namespace Core.ApplicationAction.Abstract
{
    public interface IApplicationActionRunner
    {
        ApplicationActionRunResult Run(ApplicationActionRunParameter runActionParameter);
    }
}
