using PlatypusApplicationFramework.ApplicationAction;

namespace Persistance
{
    public class ApplicationActionRepository
    {
        public void SaveActionResult(ApplicationActionResult result)
        {

        }

        public void SaveAction(string actionGuid)
        {
            /*string actionDirectory = ApplicationPaths.GetActionDirectoryPath(actionGuid);
            if (Directory.Exists(actionDirectory)) return;
            Directory.CreateDirectory(actionDirectory);*/
        }
    }
}
