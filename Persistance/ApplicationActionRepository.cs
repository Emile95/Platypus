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
            string actionDirectoryPath = ApplicationPaths.GetActionDirectoryPath(actionGuid);
            if (Directory.Exists(actionDirectoryPath)) return;
            Directory.CreateDirectory(actionDirectoryPath);

            string actionRunsDirectoryPath = ApplicationPaths.GetActionRunsDirectoryPathByBasePath(actionDirectoryPath);
            Directory.CreateDirectory(actionRunsDirectoryPath);
            File.WriteAllText(
                ApplicationPaths.GetActionRunNumberFilePathByBasePath(actionRunsDirectoryPath),
                "1"
            );


        }
    }
}
