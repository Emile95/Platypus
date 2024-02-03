using PlatypusAPI.ApplicationAction.Run;

namespace Core.ApplicationAction.Abstract
{
    internal interface IApplicationActionRunner
    {
        ApplicationActionRunResult Run(ApplicationActionRunParameter runActionParameter);
    }
}
