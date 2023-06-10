using PlatypusApplicationFramework.Action;

namespace Application.Action
{
    public class ApplicationActionRepository
    {
        public void SaveActionResult(ApplicationActionResult result)
        {

        }

        public void CreateActionRepository(string actionName)
        {
            string actionDirectory = ApplicationPaths.GetActionDirectoryPath(actionName);
            if (Directory.Exists(actionDirectory)) return;
            Directory.CreateDirectory(actionDirectory);
        }
    }
}
