using PlatypusAPI.ApplicationAction.Run;

namespace Core.ApplicationAction.Abstract
{
    public interface IApplicationActionStarter
    {
        ApplicationActionRunResult Start(StartApplicationActionParameter parameter);
    }
}
