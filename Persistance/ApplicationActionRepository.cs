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
                ApplicationPaths.GetActionLastRunNumberFilePathByBasePath(actionRunsDirectoryPath),
                "0"
            );
        }

        public void SaveActionRun(string actionGuid, int runNumber)
        {
            string actionRunsDirectoryPath = ApplicationPaths.GetActionRunsDirectoryPath(actionGuid);
            SaveActionRunByBasePath(actionRunsDirectoryPath, runNumber);
        }

        public void SaveActionRunByBasePath(string basePath, int runNumber)
        {
            string actionRunDirectoryPath = Path.Combine(basePath, runNumber.ToString());
            Directory.CreateDirectory(actionRunDirectoryPath);
            FileStream stream = File.Create(Path.Combine(actionRunDirectoryPath, ApplicationPaths.ACTIONLOGFILENAME));
            stream.Close();
        }

        public int GetAndIncrementActionRunNumber(string actionGuid)
        {
            string actionLastRunNumberFilePath = ApplicationPaths.GetActionLastRunNumberFilePath(actionGuid);
            return GetAndIncrementActionRunNumberByBasePath(actionLastRunNumberFilePath);
        }

        public int GetAndIncrementActionRunNumberByBasePath(string basePath)
        {
            string actionLastRunNumberFilePath = ApplicationPaths.GetActionLastRunNumberFilePathByBasePath(basePath);
            string lastRunNumberStr = File.ReadAllText(actionLastRunNumberFilePath);
            int lastRunNumber = Convert.ToInt32(lastRunNumberStr);
            File.WriteAllText(actionLastRunNumberFilePath, (++lastRunNumber).ToString());
            return lastRunNumber;
        }
    }
}
